using MilSpace.Core;
using MilSpace.Tools.Sentinel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.AddDem.ReliefProcessing.GuiData
{
    public class SentinelProductGui : SentinelProduct
    {
        private SentinelProductGui()
        { }
        public bool BaseScene;

        public bool Downloaded;
        public bool Downloading;

        public static SentinelProductGui Get(SentinelProduct baseData)
        {
            return Helper.Clone<SentinelProductGui>(baseData);
        }
    }
}
