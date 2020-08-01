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
        Split,
        Dem
    }

    public class ProperiesManager
    {
        private static string splitDemFilesSuffix = "split{0}VV";
        private static string splitFilesSuffix = $"{splitDemFilesSuffix}_orb_bg_if_deb_flt";
        

        private static Dictionary<SentinelProcesessEnun, string> PropertyFileName = new Dictionary<SentinelProcesessEnun, string>
        {
            { SentinelProcesessEnun.Coherence, "Milspace_Estimate_Coherence.parametres"},
            { SentinelProcesessEnun.Split, $"Milspace_{splitFilesSuffix}_SNHexp.parametres" },
            { SentinelProcesessEnun.Dem, "Milspace_{0}VVunwimp_elh_TC_selectband_nodata_tiff.parametres" }
        };

        internal static string RootProcessingFolder = SentinelPairCoherence.RootProcessingFolder;
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
                Target = $"{coherenceRes}.data"
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

        private static string ComposeQuaziTileName(int b1, int b2, int IWNumber)
        {
            return $"IW{IWNumber}B{b1.ToString().PadLeft(2, '0')}{b2.ToString().PadLeft(2, '0')}";
        }

        public ProcessDefinition ComposeDemComposeProperties(SentinelPairCoherence pair, int b1, int b2, int IWNumber)
        {
            var quaziTilePartName = ComposeQuaziTileName(b1, b2, IWNumber);
            var processingPath = Path.Combine(MilSpaceConfiguration.DemStorages.SentinelStorage, RootProcessingFolder, pair.ProcessingFolder);
            var splitName = string.Format(splitFilesSuffix, quaziTilePartName);
            var quaziTileFilder = Path.Combine(pair.SnaphuFolder, quaziTilePartName);
            if (!Directory.Exists(quaziTileFilder))
            {
                throw new DirectoryNotFoundException($"There is no quazitile filder {quaziTileFilder}");
            }

            //Get folder 
            var dirInfo = new DirectoryInfo(quaziTileFilder);
            var dirs = dirInfo.GetDirectories();
            if (!dirs.Any())
            {
                throw new DirectoryNotFoundException($"There is no any folders in {quaziTileFilder}");
            }

            dirInfo = dirs.First();
            var files = dirs.First().GetFiles("UnwPhase_*.img");
            var imgFile = files.FirstOrDefault();

            if (imgFile == null)
            {
                throw new FileNotFoundException($"There is no source image file in {dirInfo.FullName}.");
            }

            var splitDemFiles = string.Format(splitDemFilesSuffix, quaziTilePartName);
            var source1Dim = Path.Combine(pair.ProcessingFolderFullPath, $"{pair.ProcessingFolder}_{splitName}.dim").Replace("\\", "\\\\");
            var source2Image = imgFile.FullName.Replace("\\", "\\\\");
            var quaziTileName = $"{pair.ProcessingFolder}_{splitDemFiles}_DEM.tif";

            var targetDemRelativaPath = Path.Combine(pair.ProcessingFolder, quaziTileName);
            var targetDem = Path.Combine(pair.ProcessingFolderFullPath, quaziTileName).Replace("\\", "\\\\");

            //var source1Img = Path.Combine(pair.ProcessingFolderFullPath, 
            var text = new StringBuilder();
            text.AppendLine($"SRC1={source1Dim}");
            text.AppendLine($"SRC2={source2Image}");
            text.AppendLine("DEMPROJ=PROJCS[&quot;UTM Zone 37 / World Geodetic System 1984&quot;, &#xd; GEOGCS[&quot;World Geodetic System 1984&quot;, &#xd;DATUM[&quot;World Geodetic System 1984&quot;, &#xd;SPHEROID[&quot;WGS 84&quot;, 6378137.0, 298.257223563, AUTHORITY[&quot;EPSG&quot;,&quot;7030&quot;]], &#xd;AUTHORITY[&quot;EPSG&quot;,&quot;6326&quot;]], &#xd;PRIMEM[&quot;Greenwich&quot;, 0.0, AUTHORITY[&quot;EPSG&quot;,&quot;8901&quot;]], &#xd;UNIT[&quot;degree&quot;, 0.017453292519943295], &#xd;AXIS[&quot;Geodetic longitude&quot;, EAST], &#xd;AXIS[&quot;Geodetic latitude&quot;, NORTH]], &#xd;PROJECTION[&quot;Transverse_Mercator&quot;], &#xd;PARAMETER[&quot;central_meridian&quot;, 39.0], &#xd;PARAMETER[&quot;latitude_of_origin&quot;, 0.0], &#xd;PARAMETER[&quot;scale_factor&quot;, 0.9996], &#xd;PARAMETER[&quot;false_easting&quot;, 500000.0], &#xd;PARAMETER[&quot;false_northing&quot;, 0.0], &#xd;UNIT[&quot;m&quot;, 1.0], &#xd;AXIS[&quot;Easting&quot;, EAST], &#xd;AXIS[&quot;Northing&quot;, NORTH]]");
            text.AppendLine($"TRGDEM = {targetDem}");

            var fileName = string.Format(PropertyFileName[SentinelProcesessEnun.Dem], quaziTilePartName);


            var pathToPropFile = SaveParametersFile(fileName, text, processingPath);

            return new ProcessDefinition
            {
                ParamFileName = pathToPropFile,
                PairPeocessingFilder = processingPath,
                QuaziTileName = quaziTileName,
                SnapFolder = Path.Combine(pair.SnaphuFolder, quaziTilePartName),
                Target = targetDemRelativaPath,
            };
        }


        public ProcessDefinition ComposeSplitProperties(SentinelPairCoherence pair, int b1, int b2, int IWNumber)
        {
            var quasiTileName = ComposeQuaziTileName(b1, b2, IWNumber);
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
                QuaziTileName = $"{pair.ProcessingFolder}_{splitName}",
                SplitTileName = splitName,
                SnapFolder = snaphuFolder,
                Target = target
            };
        }
    }
}
