using System;
using System.Linq;
using System.Collections.Generic;
using MilSpace.Core;
using System.IO;

namespace MilSpace.DataAccess.DataTransfer.Sentinel
{
    public class SentinelPairCoherence
    {
        private string STATISTICS_MINIMUM = "STATISTICS_MINIMUM";
        private string STATISTICS_MAXIMUM = "STATISTICS_MAXIMUM";
        private string STATISTICS_MEAN = "STATISTICS_MEAN";
        private string STATISTICS_STDDEV = "STATISTICS_STDDEV";


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
                    var parts = IdSceneBase.Split('_');
                    if (parts.Length >= 5)
                    { processingFolder = parts[5]; }
                }
                return processingFolder;// S1A_IW_SLC__1SDV_20200427T041140_20200427T041207_032308_03BD12_3419"";} }
            }
        }

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