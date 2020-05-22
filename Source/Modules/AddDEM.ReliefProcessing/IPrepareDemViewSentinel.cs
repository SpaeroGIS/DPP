using MilSpace.DataAccess.DataTransfer.Sentinel;
using System;
using System.Collections.Generic;
using System.IO;


namespace MilSpace.AddDem.ReliefProcessing
{
    public interface IPrepareDemViewSentinel
    {
        string SentinelSrtorage { get; set; }
        string TileLatitude { get; }

        string TileLongtitude { get; }

        IEnumerable<Tile> TilesToImport { get; }

    }
}
