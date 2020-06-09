using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MilSpace.Configurations.Base
{
    internal class ConfigElementNames
    {

        /// <summary>
        /// Defines assembly contained Spaero actions
        /// </summary>
        internal const string Assembly = "aassembly";

        /// <summary>
        /// Defines search section name
        /// </summary>
        internal const string SearchSeactionName = "search";

        /// <summary>
        /// Defines LoadActionAssemblies Section Name 
        /// </summary>
        internal const string LoadActionAssembliesSectionName = "loadActions";

        /// <summary>
        /// Defines a connection string
        /// </summary>
        internal const string ConnectionString = "connectionString";

        /// <summary>
        /// Defines a prefix for the task functionality tables
        /// </summary>
        internal const string TaskTablePrefix = "taskTablePrefix";

        /// <summary>temporaryGDB
        /// Defines the section where the main connection is defined 
        /// </summary>

        internal const string MilSpaceConnections = "connections";

        internal const string WorkingGDBConnectionSection = "workingGDB";
        internal const string WorkingDBConnectionSection = "workingDB";
        internal const string TemporaryGDBConnectionSection = "temporaryGDB";

        /// <summary>
        /// The section group to defibne file storages for SRTM and Santitel files
        /// </summary>
        internal const string DemStorages = "demStorages";

        /// <summary>
        /// Defines the root folder for SRTM files
        /// </summary>
        internal const string SrtmStorage = "srtmStorage";

        internal const string SentinelStorage = "sentinelStorage";
        /// <summary>
        /// Python configuration section
        /// </summary>
        internal const string PythonConfiguration = "python";

        /// <summary>
        /// The folder with python scripts
        /// </summary>
        internal const string PythonScriptsFolder = "scriptsStorage";

        /// <summary>
        /// The path to python.exe
        /// </summary>
        internal const string PythonExecutable = "runWith";

        internal const string FeatureClasses = "featureClasses";
        internal const string ObservationPoint = "observationPoint";
        internal const string NameAttribute = "name";
        internal const string UserNameAttribute = "userName";
        internal const string PasswordAttribute = "password";
        internal const string RootFolderAttribte = "rootFolder";
        internal const string DownloadFolderAttribte = "downloadFolder";
        internal const string MetadataUrlAttribute = "scihubMetadataUrl";
        internal const string ProductUrlAttribte = "scihubPrtoductsUrl";
        internal const string GptExecPathAttribute = "gptExecPath";
        internal const string GptCommandsPathAttribute = "gptCommandsPath";

    }
}