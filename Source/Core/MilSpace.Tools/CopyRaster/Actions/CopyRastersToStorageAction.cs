using MilSpace.Configurations;
using MilSpace.Core.Actions.ActionResults;
using MilSpace.Core.Actions.Base;
using MilSpace.Core.Actions.Interfaces;
using MilSpace.Tools.SurfaceProfile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using A = MilSpace.Core.Actions.Base;

namespace MilSpace.Tools.CopyRaster.Actions
{
    public class CopyRastersToStorageAction : A.Action<StringCollectionResult>
    {
        private IEnumerable<string> files;
        private bool replaceFiles;
        private ResterStorageTypesEnum resterStorageType;

        public CopyRastersToStorageAction() { }

        public CopyRastersToStorageAction(IActionProcessor parameters)
               : base(parameters)
        {
            files = parameters.GetParameterWithValidition<IEnumerable<string>>(ActionParameters.Files, null).Value;
            replaceFiles = parameters.GetParameterWithValidition(ActionParameters.ReplaceOnExists, true).Value;
            resterStorageType = parameters.GetParameterWithValidition(ActionParameters.ResterStorageType, ResterStorageTypesEnum.Srtm).Value;
        }

        public override string ActionId => ActionsEnum.demCopyRaster.ToString();

        public override IActionParam[] ParametersTemplate
        {
            get
            {
                return new IActionParam[]
               {
                   new ActionParam<IEnumerable<string>>() { ParamName = ActionParameters.Files , Value = null},
                   new ActionParam<bool>() { ParamName = ActionParameters.ReplaceOnExists, Value = false},
                   new ActionParam<ResterStorageTypesEnum>() { ParamName = ActionParameters.ResterStorageType, Value = ResterStorageTypesEnum.Srtm},
               };
            }
        }



        public override StringCollectionResult GetResult()
        {
            return this.returnResult;
        }

        public override void Process()
        {

            var srtmStorage = MilSpaceConfiguration.DemStorages.SrtmStorage;
            IEnumerable<string> errorMessages = new List<string>();
            CalculationLibrary.RasterToOtherFormat(files, srtmStorage, out errorMessages);


            var fileUsage = files.ToDictionary(f => f, t => true);
            if (!replaceFiles)
            {
                foreach (var fl in files)
                {
                    //if (File)
                }

            }

        }
    }
}
