using System;
using System.Linq;
using System.Collections.Generic;
using MilSpace.Core;
using System.IO;
using MilSpace.DataAccess.Facade;
using MilSpace.Configurations;

namespace MilSpace.DataAccess.DataTransfer.Sentinel
{
    public class SentinelPairCoherence
    {
        private string STATISTICS_MINIMUM = "STATISTICS_MINIMUM";
        private string STATISTICS_MAXIMUM = "STATISTICS_MAXIMUM";
        private string STATISTICS_MEAN = "STATISTICS_MEAN";
        private string STATISTICS_STDDEV = "STATISTICS_STDDEV";
        public static string RootProcessingFolder = "Process";

        private string processingFolder = null;
        public int IdRow;

        public string IdSceneBase;
        public string IdScentSlave;
        public double Mean;
        public double Deviation;
        public double Min;
        public double Max;

        public bool RecomendedToUse => Mean > 0.55;
        public DateTime Dto;
        public string Operator;

        public IEnumerable<string> Pair => new string[] { IdSceneBase, IdScentSlave };
        public string ProcessingFolder
        {
            get
            {
                if (processingFolder == null)
                {
                    var demFacade = new DemPreparationFacade();
                    var scene = demFacade.GetSentinelProductByName(IdSceneBase);

                    var partsBase = IdSceneBase.Split('_');
                    var partsSlave = IdScentSlave.Split('_');

                    if (partsBase.Length >= 5 && partsSlave.Length >= 5)
                    {
                        processingFolder = $"S1_{scene.OrbitNumber}_{partsBase[5]}_{partsSlave[5]}";
                    }

                }
                return processingFolder;// S1A_IW_SLC__1SDV_20200427T041140_20200427T041207_032308_03BD12_3419"";} }
            }
        }

        public string ProcessingFolderFullPath => Path.Combine(MilSpaceConfiguration.DemStorages.SentinelStorage, RootProcessingFolder, ProcessingFolder == null ? "Undefined" : ProcessingFolder);

        public string SourceFileBase => Path.Combine(MilSpaceConfiguration.DemStorages.SentinelDownloadStorageExternal, IdSceneBase + ".zip");
        public string SourceFileSlave => Path.Combine(MilSpaceConfiguration.DemStorages.SentinelDownloadStorageExternal, IdScentSlave + ".zip");

        public string SnaphuFolder => Path.Combine(ProcessingFolderFullPath, "snaphu");

        public void ReadGDalStatFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(fileName);
            }

            var content = File.ReadAllLines(fileName);
            var statMin = content.FirstOrDefault(l => l.Trim().StartsWith(STATISTICS_MINIMUM));
            var statMax = content.FirstOrDefault(l => l.Trim().StartsWith(STATISTICS_MAXIMUM));
            var statDev = content.FirstOrDefault(l => l.Trim().StartsWith(STATISTICS_STDDEV));
            var statMean = content.FirstOrDefault(l => l.Trim().StartsWith(STATISTICS_MEAN));

            Max = GetValeuFormLine(statMax);
            Min = GetValeuFormLine(statMin);
            Deviation = GetValeuFormLine(statDev);
            Mean = GetValeuFormLine(statMean);

        }
        private static double GetValeuFormLine(string line)
        {
            var res = double.MinValue;
            var lines = line.Split('=');
            if (lines.Length == 2)
            {
                res = lines[1].ParceToDouble();

                if ( double.IsNaN(res))
                {
                    res = double.MinValue;
                }
            }

            return res;
        }
    }
}