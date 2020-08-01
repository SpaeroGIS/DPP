using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.AddDem.ReliefProcessing
{
    public interface IPrepareDemViewGenerateTile
    {
        string SentinelMetadataDb { get; set; }
        string TileDemLatitude { get; }

        string TileDemLongitude { get; }

    }
}
