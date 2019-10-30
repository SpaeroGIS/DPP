using ESRI.ArcGIS.Geodatabase;
using MilSpace.Core;
using MilSpace.Core.Actions.ActionResults;
using MilSpace.Core.Actions.Base;
using MilSpace.Core.Actions.Interfaces;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.Facade;
using MilSpace.Tools.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using A = MilSpace.Core.Actions.Base;

namespace MilSpace.Tools.SurfaceProfile.Actions
{
    class BuildStackVisibilityAction : A.Action<VisibilityCalculationResult>
    {
        private IFeatureClass obserpPointsfeatureClass;
        private IFeatureClass obserpStationsfeatureClass;
        private string rasterSource;
        private string outputSourceName;
        private string outGraphName;
        private VisibilitySession session;

        private VisibilityCalculationresultsEnum calcResults;

        /// <summary>
        /// Ids are selected to expotr for visibility calculation
        /// </summary>
        private int[] pointSfilteringIds;
        private int[] stationsFilteringIds;
        static Logger logger = Logger.GetLoggerEx("BuildStackVisibilityAction");

        private VisibilityCalculationResult result;

        public BuildStackVisibilityAction() : base()
        {
        }

        public BuildStackVisibilityAction(IActionProcessor parameters)
                : base(parameters)
        {

            obserpPointsfeatureClass = parameters.GetParameterWithValidition<IFeatureClass>(ActionParameters.FeatureClass, null).Value;
            obserpStationsfeatureClass = parameters.GetParameterWithValidition<IFeatureClass>(ActionParameters.FeatureClassX, null).Value;
            rasterSource = parameters.GetParameterWithValidition<string>(ActionParameters.ProfileSource, null).Value;
            pointSfilteringIds = parameters.GetParameterWithValidition<int[]>(ActionParameters.FilteringPointsIds, null).Value;
            stationsFilteringIds = parameters.GetParameterWithValidition<int[]>(ActionParameters.FilteringStationsIds, null).Value;
            calcResults = parameters.GetParameterWithValidition<VisibilityCalculationresultsEnum>(ActionParameters.Calculationresults, VisibilityCalculationresultsEnum.None).Value;
            outputSourceName = parameters.GetParameterWithValidition<string>(ActionParameters.OutputSourceName, string.Empty).Value;
            session = parameters.GetParameterWithValidition<VisibilitySession>(ActionParameters.Session, null).Value;
        }

        public override string ActionId => ActionsEnum.vblt.ToString();

        public override IActionParam[] ParametersTemplate
        {
            get
            {
                return new IActionParam[]
               {
                   new ActionParam<IFeatureClass>() { ParamName = ActionParameters.FeatureClass, Value = null},
                   new ActionParam<IFeatureClass>() { ParamName = ActionParameters.FeatureClassX, Value = null},
                   new ActionParam<int[]>() { ParamName = ActionParameters.FilteringStationsIds, Value = null},
                   new ActionParam<int[]>() { ParamName = ActionParameters.FilteringPointsIds, Value = null},
                   new ActionParam<string>() { ParamName = ActionParameters.ProfileSource, Value = string.Empty},
                   new ActionParam<VisibilityCalculationresultsEnum>() { ParamName = ActionParameters.Calculationresults, Value = VisibilityCalculationresultsEnum.None},
                   new ActionParam<string>() { ParamName = ActionParameters.OutputSourceName, Value = null},
                   new ActionParam<string>() { ParamName = ActionParameters.Session, Value = null}
               };
            }
        }


        public override VisibilityCalculationResult GetResult()
        {
            return result;
        }

        public override void Process()
        {
            result = new VisibilityCalculationResult();
            result.Result.Session = session;

            var results = new List<string>();

            if (calcResults == VisibilityCalculationresultsEnum.None)
            {
                string errorMessage = $"The value {VisibilityCalculationresultsEnum.None.ToString()} is not acceptable for calculatuion results";
                logger.WarnEx(errorMessage);
                result.Exception = new MilSpaceVisibilityCalcFailedException(errorMessage);
                return;
            }

            if (!calcResults.HasFlag(VisibilityCalculationresultsEnum.ObservationPoints) && !calcResults.HasFlag(VisibilityCalculationresultsEnum.VisibilityAreaRaster))
            {
                string errorMessage = $"The values {VisibilityCalculationresultsEnum.ObservationPoints.ToString()} and {VisibilityCalculationresultsEnum.VisibilityAreaRaster.ToString()} must be in calculatuion results";
                logger.WarnEx(errorMessage);
                result.Exception = new MilSpaceVisibilityCalcFailedException(errorMessage);
                return;
            }

            try
            {
                var oservPointFeatureClassName = VisibilitySession.GetResultName(VisibilityCalculationresultsEnum.ObservationPoints, outputSourceName);
                var exportedFeatureClass = GdbAccess.Instance.ExportObservationFeatureClass(obserpPointsfeatureClass as IDataset, oservPointFeatureClassName, pointSfilteringIds);

                if (string.IsNullOrWhiteSpace(exportedFeatureClass))
                {
                    string errorMessage = $"The feature calss {oservPointFeatureClassName} was not exported";
                    result.Exception = new MilSpaceVisibilityCalcFailedException(errorMessage);
                    result.ErrorMessage = errorMessage;
                    logger.ErrorEx(errorMessage);
                    return;
                }

                results.Add(exportedFeatureClass);


                if (calcResults.HasFlag(VisibilityCalculationresultsEnum.ObservationStations))
                {
                    var oservStationsFeatureClassName = VisibilitySession.GetResultName(VisibilityCalculationresultsEnum.ObservationStations, outputSourceName);
                    exportedFeatureClass = GdbAccess.Instance.ExportObservationFeatureClass(obserpStationsfeatureClass as IDataset, oservStationsFeatureClassName, stationsFilteringIds);
                    if (!string.IsNullOrWhiteSpace(exportedFeatureClass))
                    {
                        results.Add(exportedFeatureClass);
                    }
                    else
                    {
                        string errorMessage = $"The feature calss {oservPointFeatureClassName} was not exported";
                        result.ErrorMessage = errorMessage;
                        logger.ErrorEx(errorMessage);
                    }
                }

                //Generate Visibility Raster
                string featureClass = oservPointFeatureClassName;
                outGraphName = VisibilitySession.GetResultName(VisibilityCalculationresultsEnum.VisibilityAreaRaster, outputSourceName); ;

                IEnumerable<string> messages = null;
                if (!ProfileLibrary.GenerateVisibilityData(rasterSource, featureClass, VisibilityAnalysisTypesEnum.Frequency, outGraphName, messages))
                {
                    result.Exception = new MilSpaceVisibilityCalcFailedException();
                }
                else
                {
                    results.Add(outGraphName);
                }


                //Here is the place to extend handle the rest of results 

                result.Result.CalculationMessages = results;
                if (messages != null && messages.Any())
                {
                    messages.ToList().ForEach(m => { if (result.Exception != null) logger.ErrorEx(m); else logger.InfoEx(m); });
                }

            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }
        }
    }
}
