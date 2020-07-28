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
        Coherence,
        Split
    }

    public class ProperiesManager
    {
        private static string splitFilesSuffix = "split{0}VV_orb_bg_if_deb_flt";

        private static Dictionary<SentinelProcesessEnun, string> PropertyFileName = new Dictionary<SentinelProcesessEnun, string>
        {
            { SentinelProcesessEnun.Coherence, "Milspace_Estimate_Coherence.parametres"},
            { SentinelProcesessEnun.Split, $"Milspace_{splitFilesSuffix}_SNHexp.parametres" }
        };

        internal static string RootProcessingFolder = SentinelPairCoherence .RootProcessingFolder;
        internal static string ScriptFolder = "Scripts";

        private static Logger logger = Logger.GetLoggerEx("ProperiesManager");

        public ProcessDefinition ComposeCohherence(SentinelPairCoherence pair)
        {
            var mgr = new DemPreparationFacade();
            var tile = mgr.GetSentinelProductByName(pair.IdSceneBase)?.RelatedTile.Name;



            var processingPath = Path.Combine(MilSpaceConfiguration.DemStorages.SentinelStorage, RootProcessingFolder, pair.ProcessingFolder);

            var baseProductPath = pair.SourceFileBase.Replace("\\", "\\\\");
            var slaveProductPath = pair.SourceFileSlave.Replace("\\", "\\\\");

            var coherenceRes = Path.Combine(processingPath, "Coherence");
            var text = new StringBuilder();
            text.AppendLine($"SCENE1={baseProductPath}");
            text.AppendLine($"SCENE2={slaveProductPath}");
            text.AppendLine($"TRG = {coherenceRes.Replace("\\", "\\\\")}");
            var fileName = PropertyFileName[SentinelProcesessEnun.Coherence];
            var pathToPRocess = SaveParametersFile(fileName, text, processingPath);

            return new ProcessDefinition
            {
                ParamFileName = pathToPRocess,
                PairPeocessingFilder = processingPath,
                CoherenceProcessingFolder = $"{coherenceRes}.data"
            };
        }

        private string SaveParametersFile(string fileName, StringBuilder content, string processingFolder)
        {

            var propertiesFolderName = Path.Combine(processingFolder, "Parameters");
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
        public ProcessDefinition ComposeSplitProperties(SentinelPairCoherence pair, int b1, int b2, int IWNumber)
        {
            var quasiTileName = $"IW{IWNumber}B{b1.ToString().PadLeft(2, '0')}{b2.ToString().PadLeft(2, '0')}";
            var processingPath = Path.Combine(MilSpaceConfiguration.DemStorages.SentinelStorage, RootProcessingFolder, pair.ProcessingFolder);
            var splitName = string.Format(splitFilesSuffix, quasiTileName);

            var baseProductPath = pair.SourceFileBase.Replace("\\", "\\\\");
            var slaveProductPath = pair.SourceFileSlave.Replace("\\", "\\\\");
            var snaphuFolder = Path.Combine(pair.SnaphuFolder, quasiTileName).Replace("\\", "\\\\");
            var target = Path.Combine(pair.ProcessingFolderFullPath, $"{pair.ProcessingFolder}_{splitName}.dim").Replace("\\", "\\\\");

            var text = new StringBuilder();
            text.AppendLine($"SCENE1={baseProductPath}");
            text.AppendLine($"SCENE2={slaveProductPath}");
            text.AppendLine($"IW = IW{IWNumber}");
            text.AppendLine($"B1 = {b1}");
            text.AppendLine($"B2 = {b2}");
            text.AppendLine($"SNAPHUCTALOG = {snaphuFolder}");
            text.AppendLine($"TRG = {target}");

            var fileName = string.Format(PropertyFileName[SentinelProcesessEnun.Split], splitName);


            var pathToPropFile = SaveParametersFile(fileName, text, processingPath);

            return new ProcessDefinition
            {
                ParamFileName = pathToPropFile,
                PairPeocessingFilder = processingPath,
                QuaziTileName = splitName,
                SnapFolder = snaphuFolder
            };
        }
    }
}
