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
        private static Logger logger = Logger.GetLoggerEx("VisibilityManagerManager");

        //Visibility dataset template 
        private static readonly string VisibilityCalcFeatureClass = "VDSR";

        public VisibilityManager()
        { }

        public static VisibilitySession Generate(
            IFeatureClass obervationPoints,
            IEnumerable<int> pointsToExport,
            IFeatureClass obervationStations,
            IEnumerable<int> stationsToExport,
            string sourceDem,
             VisibilityCalculationresultsEnum culcResults, string sessionName)
        {
            //Target dataset name
            string nameOfTargetDataset = GenerateResultId();

            logger.InfoEx("Starting generation visiblility resuilt for session {2} using DEM {0} from observation points {1}.".InvariantFormat(sourceDem, obervationPoints, nameOfTargetDataset));


            var session = new VisibilitySession
            {
                Id = nameOfTargetDataset,
                Name = sessionName,
                ReferencedGDB = MilSpaceConfiguration.ConnectionProperty.TemporaryGDBConnection,
                CalculatedResults = (int)culcResults,
                UserName = Environment.UserName
            };

            session = VisibilityZonesFacade.AddVisibilitySession(session);

            if (session == null)
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
               new ActionParam<IFeatureClass>() { ParamName = ActionParameters.FeatureClass, Value = obervationPoints},
               new ActionParam<IFeatureClass>() { ParamName = ActionParameters.FeatureClassX, Value = obervationStations},
               new ActionParam<int[]>() { ParamName = ActionParameters.FilteringPointsIds, Value = pointsToExport.ToArray()},
               new ActionParam<int[]>() { ParamName = ActionParameters.FilteringStationsIds, Value = stationsToExport.ToArray()},
               new ActionParam<string>() { ParamName = ActionParameters.ProfileSource, Value = sourceDem},
               new ActionParam<VisibilityCalculationresultsEnum>() { ParamName = ActionParameters.Calculationresults, Value = culcResults},
               new ActionParam<string>() { ParamName = ActionParameters.OutputSourceName, Value = nameOfTargetDataset},
            };


            var procc = new ActionProcessor(prm);
            var res = procc.Process<StringCollectionResult>();

            if (res.Result.Count() > 0)
            {
                foreach (var calcRes in res.Result)
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

            return session;
        }

        public static string GenerateResultId()
        {
            return $"{VisibilityCalcFeatureClass}{MilSpace.DataAccess.Helper.GetTemporaryNameSuffix()}";
        }


    }
}
