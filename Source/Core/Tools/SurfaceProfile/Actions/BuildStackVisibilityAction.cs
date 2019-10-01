using ESRI.ArcGIS.Geodatabase;
using MilSpace.Core;
using MilSpace.Core.Actions.ActionResults;
using MilSpace.Core.Actions.Base;
using MilSpace.Core.Actions.Interfaces;
using MilSpace.DataAccess.Facade;
using MilSpace.Tools.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using A = MilSpace.Core.Actions.Base;

namespace MilSpace.Tools.SurfaceProfile.Actions
{
    class BuildStackVisibilityAction : A.Action<StringCollectionResult>
    {
        private IFeatureClass obserpPointsfeatureClass;
        private string rasterSource;
        private string outGraphName;
        
        /// <summary>
        /// Ids are selected to expotr for visibility calculation
        /// </summary>
        private int[] filteringIds;
        Logger logger = Logger.GetLoggerEx("BuildStackVisibilityAction");

        private StringCollectionResult result;

        public BuildStackVisibilityAction() : base()
        {
        }

        public BuildStackVisibilityAction(IActionProcessor parameters)
                : base(parameters)
        {

            obserpPointsfeatureClass = parameters.GetParameterWithValidition<IFeatureClass>(ActionParameters.FeatureClass, null).Value;
            rasterSource = parameters.GetParameterWithValidition<string>(ActionParameters.ProfileSource, null).Value;
            filteringIds = parameters.GetParameterWithValidition<int[]>(ActionParameters.FilteringIds, null).Value;
        }

        public override string ActionId => ActionsEnum.vblt.ToString();

        public override IActionParam[] ParametersTemplate
        {
            get
            {
                return new IActionParam[]
               {
                   new ActionParam<IFeatureClass>() { ParamName = ActionParameters.FeatureClass, Value = null},
                   new ActionParam<int[]>() { ParamName = ActionParameters.FilteringIds, Value = null},
                   new ActionParam<string>() { ParamName = ActionParameters.ProfileSource, Value = string.Empty}
               };
            }
        }


        public override StringCollectionResult GetResult()
        {
            return result;
        }

        public override void Process()
        {
            result = new StringCollectionResult();

            try
            {
                //
                var exportedFeatureClass = GdbAccess.Instance.ExportObservationPoints(obserpPointsfeatureClass as IDataset, filteringIds);

                if (exportedFeatureClass == null)
                {
                    //TODO: write Exception 
                    return;
                }

                //Generate Visibility Raster
                string featureClass = exportedFeatureClass;
                outGraphName = $"{featureClass}_img";

                IEnumerable<string> messages = null;
                if (ProfileLibrary.GenerateVisibilityData(rasterSource, featureClass, VisibilityAnalysisTypesEnum.Frequency, outGraphName, messages))
                {
                    result.Exception = new MilSpaceVisibilityCalcFailedException();
                }

                result.Result = messages;
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
