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
        private List<string> files;
        private bool replaceFiles;
        private ResterStorageTypesEnum resterStorageType;

        public CopyRastersToStorageAction() { }

        public CopyRastersToStorageAction(IActionProcessor parameters)
               : base(parameters)
        {
            files = parameters.GetParameterWithValidition<IEnumerable<string>>(ActionParameters.Files, null).Value?.ToList();
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

            var fileUsage = files.ToDictionary(f => new FileInfo(f), t => false);

            List<string> existen = new List<string>();
            List<string> tempLog = new List<string>();

            foreach (var fi in fileUsage)
            {
                var fiInStorage = Path.Combine(srtmStorage, fi.Key.Name);
                if (File.Exists(fiInStorage))
                {
                    try
                    {
                        if (replaceFiles)
                        {
                            File.Delete(fiInStorage);
                        }
                        else
                        {
                            files.Remove(fi.Key.FullName);
                            tempLog.Add($"File {fi.Key.FullName} was skipped");
                            existen.Add(fi.Key.FullName);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.ErrorEx(ex.Message);
                        files.Remove(fi.Key.FullName);
                        tempLog.Add($"File {fi.Key.FullName} was skipped");
                        tempLog.Add(ex.Message);
                        if (returnResult.Exception == null)
                        {
                            returnResult.Exception = ex;
                        }
                    }
                }
            }

            returnResult.Result.CopyiedFiles = existen;
            returnResult.Result.Log = tempLog;
            if (files.Any())
            {
                fileUsage = files.ToDictionary(f => new FileInfo(f), t => false);

                CalculationLibrary.RasterToOtherFormat(files, srtmStorage, out errorMessages);

                foreach (var fi in fileUsage.Keys.ToArray())
                {
                    var flNm = fi.Name;
                    var found = errorMessages.Any(m => m.EndsWith(flNm, StringComparison.CurrentCultureIgnoreCase));

                    foreach (var m in errorMessages)
                    {
                        if (m.EndsWith(flNm, StringComparison.CurrentCultureIgnoreCase))
                        {
                            fileUsage[fi] = true;
                        }
                    }

                    fileUsage[fi] = found;
                }
                returnResult.Result.Log = tempLog.Union(errorMessages);
                returnResult.Result.CopyiedFiles = existen.Union(fileUsage.Where(f => f.Value).Select(f => f.Key.FullName));
            }
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
