using MilSpace.Core;
using MilSpace.Core.Actions;
using MilSpace.Core.Actions.ActionResults;
using MilSpace.Core.Actions.Base;
using MilSpace.Core.Actions.Interfaces;
using MilSpace.Core.Tools.SurfaceProfile.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskExecuter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("You should define at least 3 parameters");
                return;
            }

            var action = new ActionParam<string>()
            {
                ParamName = ActionParamNamesCore.Action,
                Value = ActionsEnum.bsp.ToString()
            };


            var prm = new List<IActionParam> { action,
                                new ActionParam<string>() { ParamName = ActionParameters.FeatureClass, Value = args[0] },
                                new ActionParam<string>() { ParamName = ActionParameters.ProfileSource, Value = args[1]  },
                                new ActionParam<string>() { ParamName = ActionParameters.DataWorkSpace, Value = args[2]  },
                               new ActionParam<string>() { ParamName = ActionParameters.OutGraphName, Value = args.Length > 3 ? args[3] : null },
                               new ActionParam<string>() { ParamName = ActionParameters.OutGraphFileName, Value = args.Length > 4 ? args[4] : null                              } };


            var procc = new ActionProcessor(prm);
            var res = procc.Process<StringActionResult>();
            Console.WriteLine(res.Result);
        }
    }
}
