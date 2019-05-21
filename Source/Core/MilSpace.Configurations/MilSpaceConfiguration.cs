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
                        var searchPropsConfid = GetRootSectionGroup.SectionGroups[ConnectionsSection.SectionName] as ConnectionsSection;
                        connectionProperty = new MilSpaceConnectionProperty()
                        {
                            TemporaryGDBConnection = searchPropsConfid.TemporaryGDBConnectionSection.ConnectionString,
                            WorkingDBConnection = searchPropsConfid.WorkingDBConnectionSection.ConnectionString,
                            WorkingGDBConnection = searchPropsConfid.WorkingGDBConnectionSection.ConnectionString,
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
        public static string ConfigurationFileName
        {
            get { return GetCurrentConfiguration().FilePath; }
        }
    }
}
