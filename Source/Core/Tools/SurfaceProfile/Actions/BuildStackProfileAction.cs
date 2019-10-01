using MilSpace.Core;
using MilSpace.Core.Actions.ActionResults;
using MilSpace.Core.Actions.Base;
using MilSpace.Core.Actions.Interfaces;
using MilSpace.Tools.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using A = MilSpace.Core.Actions.Base;

namespace MilSpace.Tools.SurfaceProfile.Actions
{
    class BuildStackProfileAction : A.Action<StringCollectionResult>
    {
        private string featureClass;
        private string profileSource;
        private string outGraphName;
        private string tableName;
        private StringCollectionResult result;

        public BuildStackProfileAction() : base()
        {
        }

        public BuildStackProfileAction(IActionProcessor parameters)
                : base(parameters)
        {

            featureClass = parameters.GetParameterWithValidition<string>(ActionParameters.FeatureClass, null).Value;
            profileSource = parameters.GetParameterWithValidition<string>(ActionParameters.ProfileSource, null).Value;
            outGraphName = parameters.GetParameterWithValidition<string>(ActionParameters.OutputSourceName, null).Value;
            tableName = parameters.GetParameterWithValidition<string>(ActionParameters.DataWorkSpace, null).Value;
        }

        public override string ActionId => ActionsEnum.bsp.ToString();

        public override IActionParam[] ParametersTemplate
        {
            get
            {
                return new IActionParam[]
               {
                   new ActionParam<string>() { ParamName = ActionParameters.FeatureClass, Value = string.Empty},
                   new ActionParam<string>() { ParamName = ActionParameters.ProfileSource, Value = string.Empty},
                   new ActionParam<string>() { ParamName = ActionParameters.DataWorkSpace, Value = string.Empty},
                   new ActionParam<string>() { ParamName = ActionParameters.OutputSourceName, Value = string.Empty}
                   //new ActionParam<string>() { ParamName = ActionParameters.OutGraphFileName, Value = string.Empty},
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
                IEnumerable<string> mesasges = null;
                if (!ProfileLibrary.GenerateProfileData(featureClass, profileSource, tableName, mesasges))
                {
                    result.Exception = new MilSpaceVisibilityCalcFailedException();
                }
                if (mesasges != null && mesasges.Any())
                {
                    mesasges.ToList().ForEach(m => logger.InfoEx(m));
                }

                result.Result = mesasges;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }
        }
    }
}
