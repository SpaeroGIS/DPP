using MilSpace.Core;
using MilSpace.DataAccess.DataTransfer.Sentinel;

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
