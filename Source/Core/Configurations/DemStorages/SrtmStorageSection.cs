using MilSpace.Configurations.Base;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Configurations
{
    public class SrtmStorageSection : ConfigurationSection
    {
        internal static string SectionName = ConfigElementNames.SrtmStorage;

        [ConfigurationProperty(ConfigElementNames.RootFolderAttribte, IsKey = true, IsRequired = true)]
        public string RootFolder
        {
            get { return (string)this[ConfigElementNames.RootFolderAttribte]; }
            set { base[ConfigElementNames.RootFolderAttribte] = value; }
        }
        [ConfigurationProperty(ConfigElementNames.RootFolderExternalAttribte, IsKey = true, IsRequired = true)]
        public string RootFolderExternal
        {
            get { return (string)this[ConfigElementNames.RootFolderExternalAttribte]; }
            set { base[ConfigElementNames.RootFolderExternalAttribte] = value; }
        }

    }
}
