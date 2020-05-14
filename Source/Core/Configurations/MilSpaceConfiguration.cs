using MilSpace.Configurations.Connection;
using MilSpace.Configurations.DemStorages;
using MilSpace.Configurations.Python;
using System;
using System.Diagnostics;

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
                        var searchPropsConfig = GetRootSectionGroup.SectionGroups[ConnectionsSection.SectionName] as ConnectionsSection;
                        connectionProperty = new MilSpaceConnectionProperty()
                        {
                            TemporaryGDBConnection = searchPropsConfig.TemporaryGDBConnectionSection.ConnectionString,
                            WorkingDBConnection = searchPropsConfig.WorkingDBConnectionSection.ConnectionString,
                            WorkingGDBConnection = searchPropsConfig.WorkingGDBConnectionSection.ConnectionString,
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
                    var pythonConfigurationSection =
                        GetRootSectionGroup.Sections[PythonConfiguratuinSection.SectionName] as PythonConfiguratuinSection;
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
            get
            {
                return GetCurrentConfiguration().FilePath;
            }
        }

        private static DemStorages.DemStorages demStorages;
        public static DemStorages.DemStorages DemStorages
        {
            get
            {
                if (demStorages == null)
                {
                    var groupSection = GetRootSectionGroup.SectionGroups[DemStoragesSections.SectionName] as DemStoragesSections;

                    demStorages = new DemStorages.DemStorages
                    {
                        SrtmStorage  = (groupSection.Sections[SrtmStorageSection.SectionName] as SrtmStorageSection).RootFolder,
                        SentinelStorage = (groupSection.Sections[SentinelStorageSection.SectionName] as SentinelStorageSection).RootFolder,
                        ScihubMetadataApi = (groupSection.Sections[SentinelStorageSection.SectionName] as SentinelStorageSection).ScihubMetadataApi,
                        ScihubProductsApi = (groupSection.Sections[SentinelStorageSection.SectionName] as SentinelStorageSection).ScihubProductsApi,
                        ScihubPassword = (groupSection.Sections[SentinelStorageSection.SectionName] as SentinelStorageSection).Password,
                        ScihubUserName = (groupSection.Sections[SentinelStorageSection.SectionName] as SentinelStorageSection).UserName,
                    };
                }
                return demStorages;
            }
        }
    }
}
