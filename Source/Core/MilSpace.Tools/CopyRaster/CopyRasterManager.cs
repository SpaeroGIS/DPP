using MilSpace.Configurations;
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

        public void CopySrtmRasterToStoreage(string sourceFolder, bool replaceExisted)
        {
            var action = new ActionParam<string>()
            {
                ParamName = ActionParamNamesCore.Action,
                Value = ActionsEnum.demCopyRaster.ToString()
            };

            var sourceFolde = new DirectoryInfo(sourceFolder);

            if (!sourceFolde.Exists)
            {
                throw new DirectoryNotFoundException(sourceFolder);
            }

            var importfiles = sourceFolde.GetFiles("*.tif");

            var storageDbFiles = (replaceExisted ? AddDemFacade.GetSrtmGrids() : AddDemFacade.GetNotLoadedSrtmGrids()) ?? throw new InvalidDataException("Cannot read SRTM Grig data. For more detailed infor go to the Log file");
            var storageFiles = storageDbFiles.Select(gi => new FileInfo(Path.Combine(MilSpaceConfiguration.DemStorages.SrtmStorage, gi.FileName)));

            var filtered = importfiles.Where(impf => storageDbFiles.Any(sf => impf.Name.EndsWith(sf.FileName))).Select(fi => fi.FullName);

            var prm = new IActionParam[]
             {
                  action,
                   new ActionParam<IEnumerable<string>>() { ParamName = ActionParameters.Files , Value = filtered.ToArray()},
                   new ActionParam<bool>() { ParamName = ActionParameters.ReplaceOnExists, Value = replaceExisted},
                   new ActionParam<ResterStorageTypesEnum>() { ParamName = ActionParameters.ResterStorageType, Value = ResterStorageTypesEnum.Srtm},
             };

            var procc = new ActionProcessor(prm);
            var res = procc.Process<StringCollectionResult>();

        }
    }
}
