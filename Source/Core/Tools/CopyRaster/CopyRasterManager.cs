using MilSpace.Configurations;
using MilSpace.Core;
using MilSpace.Core.Actions;
using MilSpace.Core.Actions.ActionResults;
using MilSpace.Core.Actions.Base;
using MilSpace.Core.Actions.Interfaces;
using MilSpace.DataAccess.Facade;
using MilSpace.Tools.CopyRaster.Actions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Tools.CopyRaster
{
    public class CopyRasterManager
    {
        private static Logger log = Logger.GetLoggerEx("CopyRasterManager");

        public void CopySrtmRasterToStoreage(IEnumerable<string> rasterFiles, bool replaceExisted)
        {

            if (rasterFiles == null)
            {
                throw new FileLoadException("Srtm files were not defined");
            }

            var action = new ActionParam<string>()
            {
                ParamName = ActionParamNamesCore.Action,
                Value = ActionsEnum.demCopyRaster.ToString()
            };

            var prm = new IActionParam[]
             {
                  action,
                   new ActionParam<IEnumerable<string>>() { ParamName = ActionParameters.Files , Value = rasterFiles.ToArray()},
                   new ActionParam<bool>() { ParamName = ActionParameters.ReplaceOnExists, Value = replaceExisted},
                   new ActionParam<ResterStorageTypesEnum>() { ParamName = ActionParameters.ResterStorageType, Value = ResterStorageTypesEnum.Srtm},
             };

            var procc = new ActionProcessor(prm);
            var res = procc.Process<CopyRasterResult>();

            foreach( var line in res.Result.Log)
            {
                log.InfoEx(line);
            }

            if (res.Result.CopoedFiles.Count()  != rasterFiles.Count())
            {
                throw new FormatException("Not all SRTM files were coppied");
            }
        }
    }
}
