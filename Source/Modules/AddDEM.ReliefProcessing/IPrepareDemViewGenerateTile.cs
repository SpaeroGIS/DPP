using System.Collections.Generic;

namespace MilSpace.AddDem.ReliefProcessing
{
    public interface IPrepareDemViewGenerateTile
    {
        string SentinelMetadataDb { get; set; }
        string TileDemLatitude { get; }

        string TileDemLongitude { get; }

        string SelectedTileDem { get; }

        IEnumerable<string> QuaziTilesToGenerate { get; }

        string SelectedQuaziTile { get; }

    }
}
