using MilSpace.Configurations.Base;
using System.Configuration;

namespace MilSpace.Configurations
{
    public class SentinelStorageSection : ConfigurationSection
    {
        internal static string SectionName = ConfigElementNames.SentinelStorage;

        [ConfigurationProperty(ConfigElementNames.RootFolderAttribte, IsKey = true, IsRequired = true)]
        public string RootFolder
        {
            get { return (string)this[ConfigElementNames.RootFolderAttribte]; }
            set { base[ConfigElementNames.RootFolderAttribte] = value; }
        }

        [ConfigurationProperty(ConfigElementNames.DownloadFolderAttribte, IsKey = true, IsRequired = true)]
        public string DownloadFolder
        {
            get { return (string)this[ConfigElementNames.DownloadFolderAttribte]; }
            set { base[ConfigElementNames.DownloadFolderAttribte] = value; }
        }

        [ConfigurationProperty(ConfigElementNames.MetadataUrlAttribute, IsKey = true, IsRequired = true)]
        public string ScihubMetadataApi
        {
            get { return (string)this[ConfigElementNames.MetadataUrlAttribute]; }
            set { base[ConfigElementNames.MetadataUrlAttribute] = value; }
        }

        [ConfigurationProperty(ConfigElementNames.ProductUrlAttribte, IsKey = true, IsRequired = true)]
        public string ScihubProductsApi
        {
            get { return (string)this[ConfigElementNames.ProductUrlAttribte]; }
            set { base[ConfigElementNames.ProductUrlAttribte] = value; }
        }

        [ConfigurationProperty(ConfigElementNames.UserNameAttribute, IsKey = true, IsRequired = true)]
        public string UserName
        {
            get { return (string)this[ConfigElementNames.UserNameAttribute]; }
            set { base[ConfigElementNames.UserNameAttribute] = value; }
        }

        [ConfigurationProperty(ConfigElementNames.PasswordAttribute, IsKey = true, IsRequired = true)]
        public string Password
        {
            get { return (string)this[ConfigElementNames.PasswordAttribute]; }
            set { base[ConfigElementNames.PasswordAttribute] = value; }
        }

        [ConfigurationProperty(ConfigElementNames.GptExecPathAttribute, IsKey = true, IsRequired = true)]
        public string GptExecPath
        {
            get { return (string)this[ConfigElementNames.GptExecPathAttribute]; }
            set { base[ConfigElementNames.GptExecPathAttribute] = value; }
        }

        [ConfigurationProperty(ConfigElementNames.GptCommandsPathAttribute, IsKey = true, IsRequired = true)]
        public string GptCommandsPath
        {
            get { return (string)this[ConfigElementNames.GptCommandsPathAttribute]; }
            set { base[ConfigElementNames.GptCommandsPathAttribute] = value; }
        }

        [ConfigurationProperty(ConfigElementNames.SnaphuExecPathAttribute, IsKey = true, IsRequired = true)]
        public string SnaphuExecPath
        {
            get { return (string)this[ConfigElementNames.SnaphuExecPathAttribute]; }
            set { base[ConfigElementNames.SnaphuExecPathAttribute] = value; }
        }


        [ConfigurationProperty(ConfigElementNames.GdalInfoExecPathAttribute, IsKey = true, IsRequired = true)]
        public string GdalInfoExecPath
        {
            get { return (string)this[ConfigElementNames.GdalInfoExecPathAttribute]; }
            set { base[ConfigElementNames.GdalInfoExecPathAttribute] = value; }
        }
    }
}
