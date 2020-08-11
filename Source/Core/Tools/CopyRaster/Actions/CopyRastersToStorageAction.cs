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
    public class CopyRastersToStorageAction : A.Action<CopyRasterResult>
    {
        private IEnumerable<string> files;
        private bool replaceFiles;
        private ResterStorageTypesEnum resterStorageType;

        public CopyRastersToStorageAction() { }

        public CopyRastersToStorageAction(IActionProcessor parameters)
               : base(parameters)
        {
            files = parameters.GetParameterWithValidition<IEnumerable<string>>(ActionParameters.Files, null).Value;
            replaceFiles = parameters.GetParameter(ActionParameters.ReplaceOnExists, true).Value;
            resterStorageType = parameters.GetParameter(ActionParameters.ResterStorageType, ResterStorageTypesEnum.Srtm).Value;
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



        public override CopyRasterResult GetResult()
        {
            return this.returnResult;
        }

        public override void Process()
        {

            var srtmStorage = MilSpaceConfiguration.DemStorages.SrtmStorage;
            IEnumerable<string> errorMessages = new List<string>();
            CalculationLibrary.RasterToOtherFormat(files, srtmStorage, out errorMessages);


            var fileUsage = files.ToDictionary(f => new FileInfo(f), t => false);

            foreach (var fi in fileUsage.Keys.ToArray())
            {
                var flNm = fi.Name;
                fileUsage[fi] = errorMessages.Any(m => m.EndsWith(flNm, StringComparison.CurrentCultureIgnoreCase));
            }

            returnResult.Result.Log = errorMessages;
            returnResult.Result.CopoedFiles = fileUsage.Where(f => f.Value).Select(f => f.Key.FullName);
        }

        public override string Area => "CopyRaster";

        public override ActionDescription Description 
        {
            get
            {
                if (string.IsNullOrWhiteSpace(description.Description))
                {
                    description.Description = "CopyRaster";
                }

                return description;
            }
        }
    }
}
