using System.Configuration;
using MilSpace.Configurations.Base;

namespace MilSpace.Configurations.Python
{
    public class PythonConfiguratuinSection : ConfigurationSection
    {
        internal static string SectionName = ConfigElementNames.PythonConfiguration;

        [ConfigurationProperty(ConfigElementNames.PythonScriptsFolder, IsRequired = false, DefaultValue ="SpaeroPythonScripts")]
        public string PythonScriptsFolder
        {
            get { return this[ConfigElementNames.PythonScriptsFolder].ToString(); }
            set { this[ConfigElementNames.PythonScriptsFolder] = value; }
        }

        [ConfigurationProperty(ConfigElementNames.PythonExecutable, IsRequired = false, DefaultValue = @"C:\Python27\ArcGIS10.2\python.exe")]
        public string PythonExecutable
        {
            get { return this[ConfigElementNames.PythonExecutable].ToString(); }
            set { this[ConfigElementNames.PythonExecutable] = value; }
        }
    }
}
