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

                    var sentinel = (groupSection.Sections[SentinelStorageSection.SectionName] as SentinelStorageSection);
                    demStorages = new DemStorages.DemStorages
                    {
                        SrtmStorage = (groupSection.Sections[SrtmStorageSection.SectionName] as SrtmStorageSection).RootFolder,
                        SentinelStorage = sentinel.RootFolder,
                        SentinelDownloadStorage = sentinel.DownloadFolder,
                        ScihubMetadataApi = sentinel.ScihubMetadataApi,
                        ScihubProductsApi = sentinel.ScihubProductsApi,
                        ScihubPassword = sentinel.Password,
                        ScihubUserName = sentinel.UserName,
                        GptCommandsPath = sentinel.GptCommandsPath,
                        GptExecPath = sentinel.GptExecPath
                    };
                }
                return demStorages;
            }
        }
    }
}
