using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.AddDem.ReliefProcessing
{
    public interface IPrepareDemViewGenerateTile
    {
        string SentinelNetadataDb { get; set; }
        string TileLatitude { get; }

        string TileLongtitude { get; }

    }
}
