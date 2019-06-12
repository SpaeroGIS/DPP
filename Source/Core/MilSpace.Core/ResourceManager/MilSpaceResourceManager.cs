using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Core.MilSpaceResourceManager
{
    public class MilSpaceResourceManager
    {

        ResourceManager innerObject;

        public MilSpaceResourceManager(string sourceName)
        {
            var pathToAssembly = new FileInfo(Assembly.GetCallingAssembly().FullName).DirectoryName;
           innerObject = ResourceManager.CreateFileBasedResourceManager(sourceName, $"{pathToAssembly}", null);

            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("uk-UA");
        }

        public string GetLocalization(string key)
        {

            var ttt = innerObject.GetString("btnRefreshLayersToolTip_", System.Threading.Thread.CurrentThread.CurrentUICulture);

            return innerObject.GetString("btnRefreshLayersToolTip", System.Threading.Thread.CurrentThread.CurrentUICulture);
        }
    }
}
