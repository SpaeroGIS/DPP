using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using MilSpace.Configurations;
using MilSpace.Core;
using MilSpace.Core.Actions;
using MilSpace.Core.Actions.ActionResults;
using MilSpace.Core.Actions.Base;
using MilSpace.Core.Actions.Interfaces;
using MilSpace.Core.DataAccess;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.Exceptions;
using MilSpace.DataAccess.Facade;
using MilSpace.Tools.Exceptions;
using MilSpace.Tools.SurfaceProfile;
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
            IMap currentMap,
            short visibilityPercent,
            bool showAllResults = false)
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
               { ParamName = ActionParameters.FeatureClass, Value = obervationPoints},
               new ActionParam<IFeatureClass>()
               { ParamName = ActionParameters.FeatureClassX, Value = obervationStations},
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
               new ActionParam<short>()
               {ParamName = ActionParameters.VisibilityPercent, Value =  visibilityPercent},
                new ActionParam<bool>()
               {ParamName = ActionParameters.ShowAllResults, Value =  showAllResults}
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

        public static List<IObserverPoint> GetObservationPointsFromAppropriateLayer(string layerName,
                                                            IActiveView activeView, string titleFieldName = null,
                                                            IFeatureClass featureClass = null)
        {
            titleFieldName = titleFieldName ?? "TitleOp";
            var points = new List<IObserverPoint>();

            if (featureClass == null)
            {
                var mapLayersManager = new MapLayersManager(activeView);
                var layer = mapLayersManager.GetLayer(layerName);

                if (!(layer is IFeatureLayer))
                {
                    return null;
                }

                var featureLayer = layer as IFeatureLayer;


                featureClass = featureLayer.FeatureClass;
            }

            var fields = featureClass.Fields;

            var oidFieldIndex = featureClass.FindField(featureClass.OIDFieldName);
            var titleFieldIndex = (featureClass.FindField(titleFieldName) == -1)? oidFieldIndex : featureClass.FindField(titleFieldName);
            var azimuthBFieldIndex = featureClass.FindField("AzimuthB");
            var azimuthEFieldIndex = featureClass.FindField("AzimuthE");
            var anglMinHFieldIndex = featureClass.FindField("AnglMinH");
            var anglMaxHFieldIndex = featureClass.FindField("AnglMaxH");
            var innerRadiusFieldIndex = featureClass.FindField("InnerRadius");
            var outerRadiusFieldIndex = featureClass.FindField("OuterRadius");
            var heightIndex = featureClass.FindField("HRel");


            IQueryFilter queryFilter = new QueryFilter
            {
                WhereClause = $"{featureClass.OIDFieldName} >= 0"
            };

            IFeatureCursor featureCursor = featureClass.Search(queryFilter, true);
            IFeature feature = featureCursor.NextFeature();

            Func<string, string, string> CreateErrorMessage = (string message, string fieldName) =>
                $"> GetObservationPointsFromAppropriateLayer {message}. Field {fieldName}";

            try
            {
                while (feature != null)
                {
                    var shape = feature.ShapeCopy;

                    var point = shape as IPoint;
                    var pointCopy = point.Clone();
                    
                    string message;

                    try
                    {
                        if (!Helper.ConvertFromFieldType(
                                        fields.Field[oidFieldIndex].Type,
                                        feature.Value[oidFieldIndex], out int id, out message))
                        {
                            throw new InvalidCastException(CreateErrorMessage(message,
                                                               fields.Field[oidFieldIndex].AliasName));
                        }

                        if (!Helper.ConvertFromFieldType(
                                       fields.Field[titleFieldIndex].Type,
                                       feature.Value[titleFieldIndex], out string title, out message))
                        {
                            throw new InvalidCastException(CreateErrorMessage(message,
                                                               fields.Field[titleFieldIndex].AliasName));
                        }

                        if (!Helper.ConvertFromFieldType(
                                       fields.Field[azimuthBFieldIndex].Type,
                                       feature.Value[azimuthBFieldIndex], out double azimuthB, out message))
                        {
                            throw new InvalidCastException(CreateErrorMessage(message,
                                                              fields.Field[azimuthBFieldIndex].AliasName));
                        }

                        if (!Helper.ConvertFromFieldType(
                                       fields.Field[azimuthEFieldIndex].Type,
                                       feature.Value[azimuthEFieldIndex], out double azimuthE, out message))
                        {
                            throw new InvalidCastException(CreateErrorMessage(message,
                                                               fields.Field[azimuthEFieldIndex].AliasName));
                        }

                        if (!Helper.ConvertFromFieldType(
                                       fields.Field[anglMaxHFieldIndex].Type,
                                       feature.Value[anglMaxHFieldIndex], out double angleMax, out message))
                        {
                            throw new InvalidCastException(CreateErrorMessage(message,
                                                                fields.Field[anglMaxHFieldIndex].AliasName));
                        }

                        if (!Helper.ConvertFromFieldType(
                                      fields.Field[anglMinHFieldIndex].Type,
                                      feature.Value[anglMinHFieldIndex], out double angleMin, out message))
                        {
                            throw new InvalidCastException(CreateErrorMessage(message,
                                                               fields.Field[anglMinHFieldIndex].AliasName));
                        }

                        if (!Helper.ConvertFromFieldType(
                                      fields.Field[innerRadiusFieldIndex].Type,
                                      feature.Value[innerRadiusFieldIndex], out double innerRadius, out message))
                        {
                            throw new InvalidCastException(CreateErrorMessage(message,
                                                               fields.Field[innerRadiusFieldIndex].AliasName));
                        }

                        if (!Helper.ConvertFromFieldType(
                                      fields.Field[outerRadiusFieldIndex].Type,
                                      feature.Value[outerRadiusFieldIndex], out double outerRadius, out message))
                        {
                            throw new InvalidCastException(CreateErrorMessage(message,
                                                               fields.Field[outerRadiusFieldIndex].AliasName));
                        }

                        if (!Helper.ConvertFromFieldType(
                                      fields.Field[heightIndex].Type,
                                      feature.Value[heightIndex], out double relativeHeight, out message))
                        {
                            throw new InvalidCastException(CreateErrorMessage(message,
                                                               fields.Field[heightIndex].AliasName));
                        }

                        points.Add(new ObservationPoint
                        {
                            X = point.X,
                            Y = point.Y,
                            Title = title,
                            Objectid = id,
                            AzimuthStart = azimuthB,
                            AzimuthEnd = azimuthE,
                            AngelMinH = angleMin,
                            AngelMaxH = angleMax,
                            InnerRadius = innerRadius,
                            OuterRadius = outerRadius,
                            RelativeHeight = relativeHeight
                        });

                        feature = featureCursor.NextFeature();
                    }
                    catch (InvalidCastException ex)
                    {
                        logger.WarnEx(ex.Message);

                        feature = featureCursor.NextFeature();
                        continue;
                    }
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

        public static List<ObservationObject> GetObservationObjectsFromFeatureClass(IFeatureClass featureClass)
        {
            var observationObjects = new List<ObservationObject>();

            var idFieldIndex = featureClass.FindField(featureClass.OIDFieldName);
            var titleFieldIndex = featureClass.FindField("sTitleOO");
            
            if(titleFieldIndex == -1)
            {
                titleFieldIndex = idFieldIndex;
            }

            if (idFieldIndex == -1)
            {
                logger.WarnEx($"> GetObservationObjectsFromFeatureClass. Warning: Cannot find fild \"{featureClass.OIDFieldName}\" in featureClass {featureClass.AliasName}");
                return null;
            }
            
            IQueryFilter queryFilter = new QueryFilter();
            queryFilter.WhereClause = $"{featureClass.OIDFieldName} >= 0";

            IFeatureCursor featureCursor = featureClass.Search(queryFilter, true);
            IFeature feature = featureCursor.NextFeature();
            try
            {
                while (feature != null)
                {
                    var shape = feature.ShapeCopy;

                    var geometry = shape as IGeometry;

                    var id = (int)feature.Value[idFieldIndex];
                    var displayedField = feature.Value[titleFieldIndex].ToString();

                    observationObjects.Add( new ObservationObject
                    {
                        Id = id.ToString(),
                        ObjectId = id,
                        Title = displayedField,
                        DTO = DateTime.Now,
                        Creator = Environment.UserName
                    });

                    feature = featureCursor.NextFeature();
                }
            }
            catch (Exception ex)
            {
                logger.ErrorEx($"> GetObservationObjectsFromFeatureClass Exception. ex.Message:{ex.Message}");
                return null;
            }
            finally
            {
                Marshal.ReleaseComObject(featureCursor);
            }

            return observationObjects;
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
