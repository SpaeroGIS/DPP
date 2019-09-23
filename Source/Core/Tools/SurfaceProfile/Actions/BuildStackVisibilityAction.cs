using MilSpace.Core;
using MilSpace.Core.Actions.ActionResults;
using MilSpace.Core.Actions.Base;
using MilSpace.Core.Actions.Interfaces;
using MilSpace.Tools.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using A = MilSpace.Core.Actions.Base;

namespace MilSpace.Tools.SurfaceProfile.Actions
{
    class BuildStackVisibilityAction : A.Action<StringCollectionResult>
    {
        private string featureClass;
        private string profileSource;
        private string outGraphName;
        Logger logger = Logger.GetLoggerEx("BuildStackVisibilityAction");

        private StringCollectionResult result;

        public BuildStackVisibilityAction() : base()
        {
        }

        public BuildStackVisibilityAction(IActionProcessor parameters)
                : base(parameters)
        {

            featureClass = parameters.GetParameterWithValidition<string>(ActionParameters.FeatureClass, null).Value;
            profileSource = parameters.GetParameterWithValidition<string>(ActionParameters.ProfileSource, null).Value;
            outGraphName = parameters.GetParameterWithValidition<string>(ActionParameters.OutputSourceName, null).Value;
        }

        public override string ActionId => ActionsEnum.vblt.ToString();

        public override IActionParam[] ParametersTemplate
        {
            get
            {
                return new IActionParam[]
               {
                   new ActionParam<string>() { ParamName = ActionParameters.FeatureClass, Value = string.Empty},
                   new ActionParam<string>() { ParamName = ActionParameters.ProfileSource, Value = string.Empty},
                   new ActionParam<string>() { ParamName = ActionParameters.OutputSourceName, Value = string.Empty}
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
                IEnumerable<string> messages = null;
                if (ProfileLibrary.GenerateVisibilityData(profileSource, featureClass, VisibilityAnalysisTypesEnum.Frequency, outGraphName, messages))
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
