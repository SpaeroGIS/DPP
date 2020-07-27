using ESRI.ArcGIS.esriSystem;
using MilSpace.Configurations;
using MilSpace.Core;
using MilSpace.DataAccess.DataTransfer.Sentinel;
using MilSpace.DataAccess.Facade;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Tools.Sentinel
{
    internal enum SentinelProcesessEnun
    {
        Coherence
    }

    public class ProperiesManager
    {
        private static Dictionary<SentinelProcesessEnun, string> PropertyFileName = new Dictionary<SentinelProcesessEnun, string>
        {
            { SentinelProcesessEnun.Coherence, "Milspace_Estimate_Coherence.parametres"}
        };

        internal static string RootProcessingFolder = "Process";
        internal static string ScriptFolder = "Scripts";

        private static Logger logger = Logger.GetLoggerEx("ProperiesManager");

        public ProcessDefinition ComposeCohherence(SentinelPairCoherence pair)
        {
            var processingFolder = pair.ProcessingFolder;
            var mgr = new DemPreparationFacade();
            var tile = mgr.GetSentinelProductByName(pair.IdSceneBase)?.RelatedTile.Name;

            string folder = $"{tile}_{processingFolder}";

            var processingPath = Path.Combine(MilSpaceConfiguration.DemStorages.SentinelStorage, RootProcessingFolder, folder);

            var baseProductPath = Path.Combine(MilSpaceConfiguration.DemStorages.SentinelDownloadStorage, pair.IdSceneBase + ".zip").Replace("\\", "\\\\");
            var slaveProductPath = Path.Combine(MilSpaceConfiguration.DemStorages.SentinelDownloadStorage, pair.IdScentSlave + ".zip").Replace("\\", "\\\\");

            var coherenceRes = Path.Combine(processingPath, "Coherence");
            var text = new StringBuilder();
            text.AppendLine($"SCENE1={baseProductPath}");
            text.AppendLine($"SCENE2 = {slaveProductPath}");
            text.AppendLine($"TRG = {coherenceRes.Replace("\\", "\\\\")}");
            var pathToPRocess = SaveParametersFile(SentinelProcesessEnun.Coherence, text, processingPath);

            return new ProcessDefinition
            {
                CoherenceParamFileName = pathToPRocess,
                PairPeocessingFilder = processingPath,
                CoherenceProcessingFolder = $"{coherenceRes}.data"
            };
        }

        private string SaveParametersFile(SentinelProcesessEnun processingType, StringBuilder content, string processingFilder)
        {

            var fileName = PropertyFileName[processingType];
            var propertiesFolderName = Path.Combine(processingFilder, "Parameters");
            var fullName = Path.Combine(propertiesFolderName, fileName);
            if (!Directory.Exists(propertiesFolderName))
            {
                Directory.CreateDirectory(propertiesFolderName);
            }

            try
            {
                File.WriteAllText(fullName, content.ToString());
            }
            catch (Exception ex)
            {
                logger.ErrorEx($"Saving to file {fullName} ends with exception {ex.Message}");
                return null;
            }

            return fullName;

        }
        public static void SavePropertiesData(string sourceFileName, int b1, int b2,
                                         string outFileName, string path, string fileName = "gpt_Split_Orbit.properties", string IWN = "IW1")
        {
            var extention = ".properties";
            var fullName = Path.Combine(path, fileName);
            var fileInfo = new FileInfo(fullName);

            if (!fileInfo.Extension.Equals(extention, StringComparison.InvariantCultureIgnoreCase))
            {
                fullName = Path.ChangeExtension(fileInfo.FullName, extention);
            }

            if (!File.Exists(sourceFileName))
            {
                throw new FileNotFoundException($"Source file {sourceFileName} doesn`t exist");
            }

            var outFileInfo = new FileInfo(outFileName);

            if (!Directory.Exists(outFileInfo.DirectoryName))
            {
                throw new DirectoryNotFoundException($"Directory {outFileInfo.DirectoryName} doesn`t exist");
            }

            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"Directory {path} doesn`t exist");
            }

            var text = new StringBuilder();
            text.AppendLine($"sourcefilename = {sourceFileName}");
            text.AppendLine($"IWN = {IWN}");
            text.AppendLine($"B1 = {b1}");
            text.AppendLine($"B2 = {b2}");
            text.AppendLine($"outfilename = {outFileName}");

            try
            {
                File.WriteAllText(fullName, text.ToString());
            }
            catch (Exception ex)
            {
                logger.ErrorEx($"Saving to file {fullName} ends with exception {ex.Message}");
            }
        }
    }
}
