using MilSpace.Configurations;
using MilSpace.DataAccess.DataTransfer.Sentinel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Tools.Sentinel
{
    public class SnaphuManager
    {
        private SentinelPairCoherence pair;
        private string quaziToleFolder;
        private string snaphuProcessingFolder;
        private string snaphuCommandArgs;
        public SnaphuManager(SentinelPairCoherence pair, string quaziToleFolder)
        {
            this.pair = pair;
            this.quaziToleFolder = quaziToleFolder;
            ReadConfigFile();
        }

        private void ReadConfigFile()
        {
            if (!Directory.Exists(quaziToleFolder))
            {
                throw new DirectoryNotFoundException(quaziToleFolder);
            }
            var dir = Directory.GetDirectories(quaziToleFolder).FirstOrDefault();
            snaphuProcessingFolder = dir ?? throw new DirectoryNotFoundException($"No folders in {quaziToleFolder}");
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            var configFile = dirInfo.GetFiles("snaphu.conf").FirstOrDefault();


            snaphuCommandArgs = string.Empty;
            if (configFile != null)
            {
                var lines = File.ReadLines(configFile.FullName);
                foreach (var line in lines)
                {
                    if (line.Contains(shaphuCommandLimeTemplate))
                    {
                        var tempale = line.Substring(1).Trim();
                        var brackedLine = tempale.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        brackedLine[0] = MilSpaceConfiguration.DemStorages.SnaphuExecPath;
                        snaphuCommandArgs = string.Join(" ", brackedLine, 1, brackedLine.Length - 1);
                    }
                }
            }
        }


        private const string shaphuCommandLimeTemplate = "snaphu -f snaphu.conf";

        public string PnaphuProcessingFolder => snaphuProcessingFolder;
        public string SnaphuCommandLineParams => snaphuCommandArgs;
        
    }
}
