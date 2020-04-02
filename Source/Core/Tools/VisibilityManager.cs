using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using MilSpace.Configurations;
using MilSpace.Core;
using MilSpace.Core.Actions;
using MilSpace.Core.Actions.ActionResults;
using MilSpace.Core.Actions.Base;
using MilSpace.Core.Actions.Interfaces;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.Exceptions;
using MilSpace.DataAccess.Facade;
using MilSpace.Tools.Exceptions;
using MilSpace.Tools.SurfaceProfile.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace MilSpace.Tools
{
    public class VisibilityManager
    {

        public static readonly string ObservPointFeature = "MilSp_Visible_ObservPoints";
        public static readonly string ObservStationFeature = "MilSp_Visible_ObjectsObservation_R";
        public static IFeatureClass observStationFeatureClass;
        public static IFeatureClass observPointFeatureClass;
        public static IFeatureLayer observStationFeatureLayer;
        public static IFeatureLayer observPointsFeatureLayer;
        public static IMap CurrentMap;

        public delegate void SessionStartGenerationDelegate(bool isNewSessionAdded = false, string newSessionId = null);
        public static SessionStartGenerationDelegate OnGenerationStarted;

        private static Logger logger = Logger.GetLoggerEx("MilSpace.Tools.VisibilityManager");

        //Visibility dataset template 
        private const string VisibilityCalcFeatureClass = "VDSR";

        public VisibilityManager()
        {
            logger.InfoEx("> VisibilityManager Constructor");
        }

        public static VisibilityTask Generate(
            IFeatureClass obervationPoints,
            IEnumerable<int> pointsToExport,
            IFeatureClass obervationStations,
            IEnumerable<int> stationsToExport,
            string sourceDem,
            VisibilityCalculationResultsEnum culcResults,
            string taskName,
            string taskId,
            VisibilityCalcTypeEnum calculationType,
            IMap currentMap)
        {
            logger.InfoEx("> Generate START. Visiblility result {2} using DEM {0} from observation points {1}"
                .InvariantFormat(sourceDem, obervationPoints, taskId));

            CurrentMap = currentMap;
            //Target dataset name
            string nameOfTargetDataset = taskId;

            var calcTask = new VisibilityTask
            {
                Id = nameOfTargetDataset,
                Name = taskName,
                ReferencedGDB = MilSpaceConfiguration.ConnectionProperty.TemporaryGDBConnection,
                CalculatedResults = (int)culcResults,
                UserName = Environment.UserName,
                CalculationType = calculationType,
                Surface = sourceDem
            };

            calcTask = VisibilityZonesFacade.AddVisibilityTask(calcTask);
            OnGenerationStarted.Invoke(true, calcTask.Id);

            if (calcTask == null)
            {
                throw new MilSpaceVisibilityCalcFailedException("Cannot save visibility session");
            }

            var action = new ActionParam<string>()
            {
                ParamName = ActionParamNamesCore.Action,
                Value = ActionsEnum.vblt.ToString()
            };

            var prm = new List<IActionParam>
            {
               action,
               new ActionParam<IFeatureClass>()
               { ParamName = ActionParameters.FeatureClass, Value = ObservationPointsFeatureClass},
               new ActionParam<IFeatureClass>()
               { ParamName = ActionParameters.FeatureClassX, Value = ObservationStationsFeatureClass},
               new ActionParam<int[]>()
               { ParamName = ActionParameters.FilteringPointsIds, Value = pointsToExport.ToArray()},
               new ActionParam<int[]>()
               { ParamName = ActionParameters.FilteringStationsIds, Value = stationsToExport.ToArray()},
               new ActionParam<string>()
               { ParamName = ActionParameters.ProfileSource, Value = sourceDem},
               new ActionParam<VisibilityCalculationResultsEnum>()
               { ParamName = ActionParameters.Calculationresults, Value = culcResults},
               new ActionParam<string>()
               { ParamName = ActionParameters.OutputSourceName, Value = nameOfTargetDataset},
               new ActionParam<VisibilityTask>()
               { ParamName = ActionParameters.Session, Value = calcTask},
            };

            var procc = new ActionProcessor(prm);
            VisibilityZonesFacade.StarthVisibilitySession(calcTask);

            var res = procc.Process<VisibilityCalculationResult>();

            calcTask = res.Result.Session;
            if (res.Result.CalculationMessages != null && res.Result.CalculationMessages.Count() > 0)
            {
                foreach (var calcRes in res.Result.CalculationMessages)
                {
                    //Here should be checked if the results match with session.CalculatedResults
                    //logger.InfoEx($"The result layer {calcRes} was successfully composed in {calcTask.ReferencedGDB}");
                }
            }

            if (res.Exception != null)
            {
                VisibilityZonesFacade.UpdateVisibilityTask(calcTask);
                throw res.Exception;
            }
            else
            {
                VisibilityZonesFacade.FinishVisibilityTask(calcTask);
                VisibilityZonesFacade.AddVisibilityResults(calcTask.GetVisibilityResults(false));
            }

            if (!string.IsNullOrWhiteSpace(res.ErrorMessage))
            {
                throw new MilSpaceVisibilityCalcFailedException(res.ErrorMessage);
            }

            return calcTask;
        }

        public static string GenerateResultId(string preffix = VisibilityCalcFeatureClass)
        {
            logger.DebugEx("> GenerateResultId Periffics: {0}", preffix);
            return $"{preffix}{MilSpace.DataAccess.Helper.GetTemporaryNameSuffix()}";
        }

        public static bool AddVisibilityPointLayer(IActiveView view)
        {
            logger.InfoEx("> AddVisibilityPointLayer START");
            try
            {
                var pointsLayer = ObservationPointsFeatureLayer;
                view.FocusMap.AddLayer(pointsLayer);

                logger.InfoEx("> AddVisibilityPointLayer END");
                return true;
            }
            catch (MilSpaceDataException ex)
            {
                logger.ErrorEx("> AddVisibilityPointLayer Exception: {0}", ex.Message);
            }
            catch (Exception ex)
            {
                logger.ErrorEx("> AddVisibilityPointLayer Exception 2: {0}", ex.Message);
            }

            return false;
        }

        public static bool AddObservationObjectLayer(IActiveView view)
        {
            logger.InfoEx("> AddObservationObjectLayer START");
            try
            {
                var objLayer = ObservationStationsFeatureLayer;
                view.FocusMap.AddLayer(objLayer);
                logger.InfoEx("> AddObservationObjectLayer END");
                return true;
            }
            catch (MilSpaceDataException ex)
            {
                logger.ErrorEx("> AddObservationObjectLayer Exception: {0}", ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                logger.ErrorEx("> AddObservationObjectLayer Exception 2: {0}", ex.Message);
                return false;
            }
        }

        public static IFeatureClass ObservationPointsFeatureClass
        {
            get
            {
                if (observPointFeatureClass == null)
                {
                    observPointFeatureClass = GdbAccess.Instance.GetFeatureFromWorkingWorkspace(ObservPointFeature);
                }

                return observPointFeatureClass;
            }
        }

        public static IFeatureLayer ObservationPointsFeatureLayer
        {
            get
            {
                if (observPointsFeatureLayer == null)
                {
                    observPointsFeatureLayer = GdbAccess.Instance.GetLayerFromWorkingWorkspace(ObservPointFeature) as IFeatureLayer;
                }

                return observPointsFeatureLayer;
            }
        }

        public static IFeatureLayer ObservationStationsFeatureLayer
        {
            get
            {
                if(observStationFeatureLayer == null)
                {
                    observStationFeatureLayer = GdbAccess.Instance.GetLayerFromWorkingWorkspace(ObservStationFeature) as IFeatureLayer;
                }

                return observStationFeatureLayer;
            }
        }

        public static IFeatureClass ObservationStationsFeatureClass
        {
            get
            {
                if (observStationFeatureClass == null)
                {
                    observStationFeatureClass = GdbAccess.Instance.GetFeatureFromWorkingWorkspace(ObservStationFeature);
                }

                return observStationFeatureClass;
            }
        }

        public static List<ObservationPoint> GetObservationPointsFromAppropriateLayer(string layerName, IActiveView activeView)
        {
            var mapLayersManager = new MapLayersManager(activeView);
            var layer = mapLayersManager.GetLayer(layerName);

            if (!(layer is IFeatureLayer))
            {
                return null;
            }

            var featureLayer = layer as IFeatureLayer;
            var points = new List<ObservationPoint>();

            var featureClass = featureLayer.FeatureClass;

            IQueryFilter queryFilter = new QueryFilter();
            queryFilter.WhereClause = "OBJECTID > 0";

            IFeatureCursor featureCursor = featureClass.Search(queryFilter, true);
            int heightIndex = featureClass.FindField("HRel");
            IFeature feature = featureCursor.NextFeature();
            try
            {
                while (feature != null)
                {
                    var shape = feature.ShapeCopy;

                    var point = shape as IPoint;
                    var pointCopy = point.Clone();

                        points.Add(new ObservationPoint
                        {
                            X = point.X,
                            Y = point.Y,
                            Title = feature.Value[featureClass.FindField("TitleOp")].ToString(),
                            Id = feature.Value[featureClass.FindField(featureClass.OIDFieldName)].ToString(),
                            AzimuthStart = (double)feature.Value[featureClass.FindField("AzimuthB")],
                            AzimuthEnd = (double)feature.Value[featureClass.FindField("AzimuthE")],
                            AngelMinH = (double)feature.Value[featureClass.FindField("AnglMinH")],
                            AngelMaxH = (double)feature.Value[featureClass.FindField("AnglMaxH")],
                            RelativeHeight = (heightIndex == -1) ? 0 : (double)feature.Value[heightIndex]
                        });

                    feature = featureCursor.NextFeature();
                }
            }
            catch (Exception ex)
            {
                logger.ErrorEx($"> GetObservationPointsFromAppropriateLayer Exception. ex.Message:{ex.Message}");
            }
            finally
            {
                Marshal.ReleaseComObject(featureCursor);
            }

            return points;
        }


        private static void OnCalculationFinished(IActionResult message)
        {
            if (message is VisibilityCalculationResult res)
            {
                var session = res.Result.Session;

                if (res.Result.CalculationMessages.Count() > 0)
                {
                    foreach (var calcRes in res.Result.CalculationMessages)
                    {
                        //Here should be checked if the results match with session.CalculatedResults
                        logger.InfoEx("> OnCalculationFinished. The result layer {0} was composed in {1}", calcRes, session.ReferencedGDB);
                    }
                }

                if (res.Exception != null)
                {
                    VisibilityZonesFacade.FinishVisibilityTask(session);
                    throw res.Exception;
                }

                if (!string.IsNullOrWhiteSpace(res.ErrorMessage))
                {
                    throw new MilSpaceVisibilityCalcFailedException(res.ErrorMessage);
                }
            }

        }


    }
}
