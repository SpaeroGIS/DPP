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
        private static readonly string FIRST_DIST_Field = "FIRST_DIST";
        private static readonly string FIRST_Z_Field = "FIRST_Z";
        private static readonly string LINE_ID_Field = "LINE_ID";

        private static readonly string WhereAllRecords = "OBJECTID > 0";

        public static readonly string ObservPointFeature = "MilSp_Visible_ObservPoints";
        public static readonly string ObservStationFeature = "MilSp_Visible_ObjectsObservation_R";
        public static IFeatureClass observStationFeatureClass;
        public static IFeatureClass observPointFeatureClass;

        public delegate void SessionStartGenerationDelegate(bool isNewSessionAdded = false);
        public static SessionStartGenerationDelegate OnGenerationStarted;

        private static Logger logger = Logger.GetLoggerEx("VisibilityManagerManager");


        //Visibility dataset template 
        private const string VisibilityCalcFeatureClass = "VDSR";

        public VisibilityManager()
        { }

        public static VisibilityTask Generate(
            IFeatureClass obervationPoints,
            IEnumerable<int> pointsToExport,
            IFeatureClass obervationStations,
            IEnumerable<int> stationsToExport,
            string sourceDem,
            VisibilityCalculationresultsEnum culcResults,
            string taskName,
            string taskId,
            VisibilityCalcTypeEnum calculationType)
        {
            //Target dataset name
            string nameOfTargetDataset = taskId;

            logger.InfoEx("Starting generation visiblility resuilt for session {2} using DEM {0} from observation points {1}.".InvariantFormat(sourceDem, obervationPoints, nameOfTargetDataset));


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

            calcTask = VisibilityZonesFacade.AddVisibilitySession(calcTask);
            OnGenerationStarted.Invoke(true);

            if (calcTask == null)
            {
                throw new MilSpaceVisibilityCalcFailedException("Cannot save visibility session.");
            }

            var action = new ActionParam<string>()
            {
                ParamName = ActionParamNamesCore.Action,
                Value = ActionsEnum.vblt.ToString()
            };


            var prm = new List<IActionParam>
           {
               action,
               new ActionParam<IFeatureClass>() { ParamName = ActionParameters.FeatureClass, Value = ObservationPointsFeatureClass},
               new ActionParam<IFeatureClass>() { ParamName = ActionParameters.FeatureClassX, Value = ObservationStationsFeatureClass},
               new ActionParam<int[]>() { ParamName = ActionParameters.FilteringPointsIds, Value = pointsToExport.ToArray()},
               new ActionParam<int[]>() { ParamName = ActionParameters.FilteringStationsIds, Value = stationsToExport.ToArray()},
               new ActionParam<string>() { ParamName = ActionParameters.ProfileSource, Value = sourceDem},
               new ActionParam<VisibilityCalculationresultsEnum>() { ParamName = ActionParameters.Calculationresults, Value = culcResults},
               new ActionParam<string>() { ParamName = ActionParameters.OutputSourceName, Value = nameOfTargetDataset},
               new ActionParam<VisibilityTask>() { ParamName = ActionParameters.Session, Value = calcTask},
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
                    logger.InfoEx($"The result layer {calcRes} was successfully composed in {calcTask.ReferencedGDB}");
                }
            }

            if (res.Exception != null)
            {
                throw res.Exception;
            }
            else
            {
                VisibilityZonesFacade.FinishVisibilitySession(calcTask);
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
            return $"{preffix}{MilSpace.DataAccess.Helper.GetTemporaryNameSuffix()}";
        }

        public static bool AddVisibilityPointLayer(IActiveView view)
        {
            try
            {
                var pointsLayer = GdbAccess.Instance.GetLayerFromWorkingWorkspace(ObservPointFeature);
                view.FocusMap.AddLayer(pointsLayer);
            }
            catch (MilSpaceDataException ex)
            {
                logger.ErrorEx(ex.Message);
            }
            catch (Exception ex)
            {
                logger.ErrorEx("Unexpectede error");
                logger.ErrorEx(ex.Message);
            }

            return false;
        }

        public static bool AddObservationObjectLayer(IActiveView view)
        {
            try
            {
                var objLayer = GdbAccess.Instance.GetLayerFromWorkingWorkspace(ObservStationFeature);
                view.FocusMap.AddLayer(objLayer);
            }
            catch (MilSpaceDataException ex)
            {
                logger.ErrorEx(ex.Message);
            }
            catch (Exception ex)
            {
                logger.ErrorEx("Unexpectede error");
                logger.ErrorEx(ex.Message);
            }

            return false;
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
                        logger.InfoEx($"The result layer {calcRes} was successfully composed in {session.ReferencedGDB}");
                    }
                }

                if (res.Exception != null)
                {
                    VisibilityZonesFacade.FinishVisibilitySession(session);
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
