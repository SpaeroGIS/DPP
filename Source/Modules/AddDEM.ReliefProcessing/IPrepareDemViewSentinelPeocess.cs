using MilSpace.AddDem.ReliefProcessing.GuiData;
using MilSpace.DataAccess.DataTransfer.Sentinel;
using MilSpace.Tools.Sentinel;
using System;
using System.Collections.Generic;
using System.IO;


namespace MilSpace.AddDem.ReliefProcessing
{
    public interface IPrepareDemViewSentinelPeocess
    {
        IEnumerable<Tile> DownloadedTiles { get; }

        SentinelPairCoherence SentinelPairDem { get; set; }

        SentinelProduct SelectedProductDem { get; }

    }
}
