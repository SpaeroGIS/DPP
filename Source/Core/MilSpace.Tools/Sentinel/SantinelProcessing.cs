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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Tools.Sentinel
{
    public class SantinelProcessing
    {
        private static Logger logger = Logger.GetLoggerEx("SantinelProcessing");
        const string EstimateCoherenceExecutionFile = "Milspace_Estimate_Coherence.xml";
        private static string coherenceStatFileName = null;

        private static string gptExecFile = "gpt.exe";
        public static void EstimateCoherence(SentinelPairCoherence pair)
        {
            var propMgr = new ProperiesManager();
            var command = Path.Combine(MilSpaceConfiguration.DemStorages.SentinelStorage, ProperiesManager.ScriptFolder, EstimateCoherenceExecutionFile);
            if (!File.Exists(command))
            {
                throw new FileNotFoundException(command);
            }

            var prop = propMgr.ComposeCohherence(pair);
            var paramName = $"{command} -p {prop.CoherenceParamFileName}";
            DoPreProcessing(Path.Combine(MilSpaceConfiguration.DemStorages.GptExecPath, gptExecFile), paramName);

            var coherenceResFolder = prop.CoherenceProcessingFolder;
            var imgFile = Directory.GetFiles(coherenceResFolder, "*.img", SearchOption.TopDirectoryOnly);

            if (imgFile.Any())
            {
                var imgFileName = imgFile.First();

                var fileInfo = new FileInfo(imgFileName);
                var ext = fileInfo.Extension;
                var fileName = fileInfo.Name;

                coherenceStatFileName = Path.Combine(coherenceResFolder, $"{imgFileName}.stat");

                var gdalExec = $"-stats -hist {imgFileName}";
                logger.InfoEx($"Starting GdalInfo to get coherence: {gdalExec}");
                DoPreProcessing(MilSpaceConfiguration.DemStorages.GdalInfoExecPath, gdalExec, OnOutputCoherenceCommandLine);

                if (File.Exists(coherenceStatFileName))
                {
                    prop.CoherenceStatFileName = coherenceStatFileName;
                    pair.ReadGDalStatFile(coherenceStatFileName);


                    var facade = new DemPreparationFacade();
                    facade.UpdateSentinelPairCoherence(pair);
                }
                coherenceStatFileName = null;
            }
        }

        public static void DoPreProcessing(string commandFile, string parameters,
            ActionProcessCommandLineDelegate onOutputCommandLine = null,
            ActionProcessCommandLineDelegate onErrorCommandLine = null)
        {
            //string commandFile =;//  @"E:\SourceCode\40copoka\DPP\Source\UnitTests\CommandLineAction.UnitTest\Output\CommandLineAction.UnitTest.exe";

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
    }
}
