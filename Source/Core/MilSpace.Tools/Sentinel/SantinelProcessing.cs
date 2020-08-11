using Microsoft.SqlServer.Types;
using MilSpace.Configurations;
using MilSpace.Core;
using MilSpace.Core.Actions;
using MilSpace.Core.Actions.ActionResults;
using MilSpace.Core.Actions.Base;
using MilSpace.Core.Actions.Interfaces;
using MilSpace.DataAccess.DataTransfer.Sentinel;
using MilSpace.DataAccess.Facade;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MilSpace.Tools.Sentinel
{
    public class SantinelProcessing
    {
        private static Logger logger = Logger.GetLoggerEx("SantinelProcessing");
        const string EstimateCoherenceCommand = "Milspace_Estimate_Coherence.xml";
        const string SplitProductsToBirstsCommand = "Milspace_split_orb_bg_if_deb_flt_SNHexp.xml";
        const string DemComposeCommand = "Milspace_unwimp_elh_TC_selectband_nodata_tif.xml";

        private static int[] IWValues = new int[] { 1, 2, 3 };
        private static int[][] bValues = new int[][] { new int[] { 1, 5 }, new int[] { 6, 10 } };
        private static string coherenceStatFileName = null;

        private static string gptExecFile = "gpt.exe";
        public static void EstimateCoherence(SentinelPairCoherence pair)
        {
            logger.InfoEx("EstimateCoherence. Statring..");
            var command = CheckCommandFileExistance(EstimateCoherenceCommand);
            var propMgr = new ProperiesManager();
            var prop = propMgr.ComposeCohherence(pair);
            var paramName = $"{command} -p {prop.ParamFileName}";
            DoPreProcessing(Path.Combine(MilSpaceConfiguration.DemStorages.GptExecPath, gptExecFile), paramName);

            var coherenceResFolder = prop.Target;
            var imgFile = Directory.GetFiles(coherenceResFolder, "*.img", SearchOption.TopDirectoryOnly);

            if (imgFile.Any())
            {
                var imgFileName = imgFile.First();
                logger.InfoEx($"EstimateCoherence. Image file {imgFileName} was generated.");

                var fileInfo = new FileInfo(imgFileName);
                var ext = fileInfo.Extension;
                var fileName = fileInfo.Name;

                coherenceStatFileName = Path.Combine(coherenceResFolder, $"{imgFileName}.stat");

                var gdalExec = $"-stats -hist {imgFileName}";
                logger.InfoEx($"Starting GdalInfo to get coherence: {gdalExec}");
                DoPreProcessing(MilSpaceConfiguration.DemStorages.GdalInfoExecPath, gdalExec, coherenceResFolder, OnOutputCoherenceCommandLine);

                if (File.Exists(coherenceStatFileName))
                {
                    prop.CoherenceStatFileName = coherenceStatFileName;
                    pair.ReadGDalStatFile(coherenceStatFileName);

                    var facade = new DemPreparationFacade();
                    logger.InfoEx($"Saving coherence data.");
                    facade.UpdateSentinelPairCoherence(pair);
                }
                coherenceStatFileName = null;
            }

            logger.InfoEx("EstimateCoherence. Finished.");
        }

        public static void PairProcessing(SentinelPairCoherence pair)
        {
            string command;//
            var facade = new DemPreparationFacade();
            var propMgr = new ProperiesManager();
            foreach (var iw in IWValues)
            {
                foreach (var birsts in bValues)
                {
                    var prop = propMgr.ComposeSplitProperties(pair, birsts[0], birsts[1], iw);
                    logger.InfoEx($"Processing quazitile {prop.SplitTileName}");


                    SentinelTilesCoverage tileCover = facade.AddOrUpdateTileCoverage(
                        new SentinelTilesCoverage
                        {
                            QuaziTileName = prop.QuaziTileName,
                            SceneName = pair.ProcessingFolder,
                            Status = (int)QuaziTileStateEnum.SplitOngoig
                        });


                    if (!Directory.Exists(prop.SnapFolder))
                    {
                        Directory.CreateDirectory(prop.SnapFolder);
                    }

                    command = CheckCommandFileExistance(SplitProductsToBirstsCommand);
                    //QuaziTile Generation
                    var paramName = $"{command} -p {prop.ParamFileName}";

                    logger.InfoEx($"Executing {Path.Combine(MilSpaceConfiguration.DemStorages.GptExecPath, gptExecFile)} {paramName}");
                    DoPreProcessing(Path.Combine(MilSpaceConfiguration.DemStorages.GptExecPath, gptExecFile), paramName, prop.PairPeocessingFilder);
                    var wktText = GetQauzitileWkt(prop.Target);
                    if (!string.IsNullOrWhiteSpace(wktText))
                    {
                        tileCover.Wkt = wktText;
                        tileCover = facade.AddOrUpdateTileCoverage(tileCover);
                    }

                    logger.InfoEx($"Quazitile {prop.QuaziTileName} processed.");


                    //Snaphu Processing
                    tileCover.Status = (int)QuaziTileStateEnum.Snaphu;
                    facade.AddOrUpdateTileCoverage(tileCover);
                    logger.InfoEx($"Processing Snaphu for {prop.QuaziTileName}");
                    SnaphuManager snaphuMgr = new SnaphuManager(pair, prop.SnapFolder);
                    logger.InfoEx($"Executing {MilSpaceConfiguration.DemStorages.SnaphuExecPath} {snaphuMgr.SnaphuCommandLineParams}");
                    DoPreProcessing(MilSpaceConfiguration.DemStorages.SnaphuExecPath, snaphuMgr.SnaphuCommandLineParams, snaphuMgr.PnaphuProcessingFolder);
                    logger.InfoEx($"Snaphu for {prop.QuaziTileName} processed.");

                    //DEM processing
                    try
                    {
                        prop = propMgr.ComposeDemComposeProperties(pair, birsts[0], birsts[1], iw);
                        tileCover.Status = (int)QuaziTileStateEnum.Dem;
                        facade.AddOrUpdateTileCoverage(tileCover);

                        command = CheckCommandFileExistance(DemComposeCommand);
                        paramName = $"{command} -p {prop.ParamFileName}";
                        logger.InfoEx($"Executing {Path.Combine(MilSpaceConfiguration.DemStorages.GptExecPath, gptExecFile)} {paramName}");

                        DoPreProcessing(Path.Combine(MilSpaceConfiguration.DemStorages.GptExecPath, gptExecFile), paramName, prop.PairPeocessingFilder);

                        tileCover.Status = (int)QuaziTileStateEnum.Finished;
                        tileCover.DEMFilePath = prop.Target;
                        facade.AddOrUpdateTileCoverage(tileCover);
                        logger.InfoEx($"Removing {prop.SnapFolder} ...");
                        Directory.Delete(prop.SnapFolder, true);
                    }
                    catch (DirectoryNotFoundException ex)
                    {
                        logger.ErrorEx($"{iw}B1 = {birsts[0]} B2 {birsts[1]} get error.");
                        logger.ErrorEx(ex.Message);
                    }
                    catch (FileNotFoundException ex)
                    {
                        logger.ErrorEx($"{iw}B1 = {birsts[0]} B2 {birsts[1]} get error.");
                        logger.ErrorEx(ex.Message);

                    }
                    catch (Exception ex)
                    {
                        logger.ErrorEx($"{iw}B1 = {birsts[0]} B2 {birsts[1]} get error.");
                        logger.ErrorEx($"Unexpected error {ex.Message}");
                    }

                }
            }
        }

        public static void DemCompose(SentinelPairCoherence pair)
        {
            var command = CheckCommandFileExistance(DemComposeCommand);
            var propMgr = new ProperiesManager();

            foreach (var iw in IWValues)
            {
                foreach (var birsts in bValues)
                {
                    try
                    {
                        var prop = propMgr.ComposeDemComposeProperties(pair, birsts[0], birsts[1], iw);
                        var paramName = $"{command} -p {prop.ParamFileName}";
                        DoPreProcessing(Path.Combine(MilSpaceConfiguration.DemStorages.GptExecPath, gptExecFile), paramName, prop.PairPeocessingFilder);
                    }
                    catch (DirectoryNotFoundException ex)
                    {
                        logger.ErrorEx($"{iw}B1 = {birsts[0]} B2 {birsts[1]} get error.");
                        logger.ErrorEx(ex.Message);
                    }
                    catch (FileNotFoundException ex)
                    {
                        logger.ErrorEx($"{iw}B1 = {birsts[0]} B2 {birsts[1]} get error.");
                        logger.ErrorEx(ex.Message);

                    }
                    catch (Exception ex)
                    {
                        logger.ErrorEx($"{iw}B1 = {birsts[0]} B2 {birsts[1]} get error.");
                        logger.ErrorEx($"Unexpected error {ex.Message}");
                    }
                }
            }
        }

        public static void DoPreProcessing(string commandFile, string parameters, string workingDirectory = null,
            ActionProcessCommandLineDelegate onOutputCommandLine = null,
            ActionProcessCommandLineDelegate onErrorCommandLine = null)
        {

            var action = new ActionParam<string>()
            {
                ParamName = ActionParamNamesCore.Action,
                Value = ActionsCore.RunCommandLine
            };

            if (onOutputCommandLine == null)
            {
                onOutputCommandLine = OnOutputCommandLine;
            }
            if (onErrorCommandLine == null)
            {
                onErrorCommandLine = OnErrorCommandLine;
            }

            var prm = new IActionParam[]
            {
                  action,
                    new ActionParam<string>() { ParamName = ActionParamNamesCore.PathToFile, Value = commandFile},
                    new ActionParam<string>() { ParamName = ActionParamNamesCore.WorkingDirectory, Value = workingDirectory},
                    new ActionParam<string>() { ParamName = ActionParamNamesCore.DataValue, Value = parameters},
                    new ActionParam<ActionProcessCommandLineDelegate>()
                    { ParamName = ActionParamNamesCore.OutputDataReceivedDelegate, Value = onOutputCommandLine},
                    new ActionParam<ActionProcessCommandLineDelegate>()
                    { ParamName = ActionParamNamesCore.ErrorDataReceivedDelegate, Value = onErrorCommandLine}
            };

            var procc = new ActionProcessor(prm);
            var res = procc.Process<StringActionResult>();
        }

        public static void OnErrorCoherenceCommandLine(string consoleMessage, ActironCommandLineStatesEnum state)
        {
            logger.ErrorEx(consoleMessage);
        }

        public static void OnOutputCoherenceCommandLine(string consoleMessage, ActironCommandLineStatesEnum state)
        {
            if (coherenceStatFileName != null)
            {
                StreamWriter sw = File.Exists(coherenceStatFileName) ?
                    File.AppendText(coherenceStatFileName) : File.CreateText(coherenceStatFileName);
                sw.WriteLine(consoleMessage);
                sw.Dispose();
            }
            logger.InfoEx(consoleMessage);

        }

        public static void OnErrorCommandLine(string consoleMessage, ActironCommandLineStatesEnum state)
        {
            logger.ErrorEx(consoleMessage);
        }

        public static void OnOutputCommandLine(string consoleMessage, ActironCommandLineStatesEnum state)
        {
            logger.InfoEx(consoleMessage);
        }

        private static string CheckCommandFileExistance(string commandFileName)
        {
            var command = Path.Combine(MilSpaceConfiguration.DemStorages.SentinelScriptsFolder, commandFileName);
            if (!File.Exists(command))
            {
                throw new FileNotFoundException(command);
            }

            return command;
        }

        private static string GetQauzitileWkt(string qauzitileDefinition)
        {
            if (File.Exists(qauzitileDefinition))
            {
                try
                {
                    using (var flstr = new FileStream(qauzitileDefinition, FileMode.Open))
                    {
                        var doc = XDocument.Load(flstr);
                        var DatasetSources = doc.Elements().First().Elements(XName.Get("Dataset_Sources")).FirstOrDefault();

                        if (DatasetSources != null)
                        {
                            var firs1tLat = DatasetSources.Descendants(XName.Get("MDATTR")).FirstOrDefault(e =>
                            {
                                var attr = e.Attribute(XName.Get("name"));
                                return attr == null ? false : attr.Value == "first_near_lat";
                            });
                            if (firs1tLat == null) return null;

                            var first1Long = DatasetSources.Descendants(XName.Get("MDATTR")).FirstOrDefault(e =>
                            {
                                var attr = e.Attribute(XName.Get("name"));
                                return attr == null ? false : attr.Value == "first_near_long";
                            });
                            if (first1Long == null) return null;

                            var first2Lat = DatasetSources.Descendants(XName.Get("MDATTR")).FirstOrDefault(e =>
                            {
                                var attr = e.Attribute(XName.Get("name"));
                                return attr == null ? false : attr.Value == "first_far_lat";
                            });
                            if (first2Lat == null) return null;

                            var first2Long = DatasetSources.Descendants(XName.Get("MDATTR")).FirstOrDefault(e =>
                            {
                                var attr = e.Attribute(XName.Get("name"));
                                return attr == null ? false : attr.Value == "first_far_long";
                            });
                            if (first2Long == null) return null;

                            var last1Lat = DatasetSources.Descendants(XName.Get("MDATTR")).FirstOrDefault(e =>
                            {
                                var attr = e.Attribute(XName.Get("name"));
                                return attr == null ? false : attr.Value == "last_near_lat";
                            });
                            if (last1Lat == null) return null;

                            var last1Long = DatasetSources.Descendants(XName.Get("MDATTR")).FirstOrDefault(e =>
                            {
                                var attr = e.Attribute(XName.Get("name"));
                                return attr == null ? false : attr.Value == "last_near_long";
                            });
                            if (last1Long == null) return null;

                            var last2Lat = DatasetSources.Descendants(XName.Get("MDATTR")).FirstOrDefault(e =>
                            {
                                var attr = e.Attribute(XName.Get("name"));
                                return attr == null ? false : attr.Value == "last_far_lat";
                            });
                            if (last2Lat == null) return null;

                            var last2Long = DatasetSources.Descendants(XName.Get("MDATTR")).FirstOrDefault(e =>
                            {
                                var attr = e.Attribute(XName.Get("name"));
                                return attr == null ? false : attr.Value == "last_far_long";
                            });
                            if (last2Long == null) return null;


                            string wktText = $"MULTIPOLYGON(((" +
                                $"{first1Long.Value} {firs1tLat.Value}," +
                                $"{first2Long.Value} {first2Lat.Value}," +
                                $"{last2Long.Value} {last2Lat.Value}," +
                                $"{last1Long.Value} {last1Lat.Value}," +
                                $"{first1Long.Value} {firs1tLat.Value})))";

                            SqlChars chrs = new SqlChars(new SqlString(wktText));
                            var wkt = SqlGeography.STMPolyFromText(chrs, 4326);

                            return wktText;
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.ErrorEx(ex.Message);
                }
            }
            return null;
        }
    }
}
