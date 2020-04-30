using MilSpace.Core.Actions;
using MilSpace.Core.Actions.Base;
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

        public void CopySrtmRasterToStoreage(string sourceFolder)
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

            var importfiles = sourceFolde.GetFiles("*.tif").Select( fi => fi.FullName);

        }
    }
}
