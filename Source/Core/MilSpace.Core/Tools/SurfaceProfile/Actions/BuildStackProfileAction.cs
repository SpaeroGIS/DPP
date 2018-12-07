using MilSpace.Core.Actions.ActionResults;
using MilSpace.Core.Actions.Base;
using MilSpace.Core.Actions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using A = MilSpace.Core.Actions.Base;

namespace MilSpace.Core.Tools.SurfaceProfile.Actions
{
    class BuildStackProfileAction : A.Action<StringActionResult>
    {

        public BuildStackProfileAction() : base()
        {
        }

        public BuildStackProfileAction(IActionProcessor parameters)
                : base(parameters)
        {

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
                    new ActionParam<string>() { ParamName = ActionParameters.OutGraphName, Value = string.Empty},
                    new ActionParam<string>() { ParamName = ActionParameters.OutGraphFileName, Value = string.Empty},
               };
            }
        }


        public override StringActionResult GetResult()
        {
            throw new NotImplementedException();
        }

        public override void Process()
        {
            throw new NotImplementedException();
        }
    }
}
