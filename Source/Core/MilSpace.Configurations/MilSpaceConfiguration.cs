using MilSpace.Configurations.Connection;
using MilSpace.Configurations.Python;
using System;

namespace MilSpace.Configurations
{
    public class MilSpaceConfiguration : MilSpaceRootConfiguration
    {

        private static MilSpaceConnectionProperty connectionProperty;
        public static MilSpaceConnectionProperty ConnectionProperty
        {
            get
            {
                if (connectionProperty == null)
                {
                    try
                    {
                        var searchPropsConfid = GetRootSectionGroup.Sections[ConnectionSection.SectionName] as ConnectionSection;
                    
                    

                    connectionProperty = new MilSpaceConnectionProperty()
                    {
                        ConnectionString = searchPropsConfid.ConnectionString,
                        TaskTablePrefix = searchPropsConfid.TaskTablePrefix,
                    };
                    }
                    catch (Exception ex)
                    {

                        string r = ex.Message;
                    }
                }

                return connectionProperty;
            }
        }

        private static PythonConfiguration pythonConfiguration;
        public static PythonConfiguration PythonConfigurationPoperty
        {
            get
            {
                if (pythonConfiguration == null)
                {
                    var pythonConfigurationSection = GetRootSectionGroup.Sections[PythonConfiguratuinSection.SectionName] as PythonConfiguratuinSection;
                    pythonConfiguration = new PythonConfiguration
                    {
                        PythonExecutable = pythonConfigurationSection.PythonExecutable,
                        PythonScriptsFolder = pythonConfigurationSection.PythonScriptsFolder
                    };
                }

                return pythonConfiguration;
            }
        }
        public static string ConfigurationFilePath
        {
            get { return GetCurrentConfiguration().FilePath; }
        }
    }
}
