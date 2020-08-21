using MilSpace.Core.Actions;
using MilSpace.DataAccess.DataTransfer.Sentinel;
using System.Collections.Generic;


namespace MilSpace.AddDem.ReliefProcessing
{
    public interface IPrepareDemViewSentinelPeocess
    {
        IEnumerable<Tile> DownloadedTiles { get; }

        SentinelPairCoherence SentinelPairDem { get; set; }

        SentinelProduct SelectedProductDem { get; }

        void OnProcessing(string consoleMessage, ActironCommandLineStatesEnum state);
        
    }
}
