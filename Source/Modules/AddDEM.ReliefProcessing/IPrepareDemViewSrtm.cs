using MilSpace.DataAccess.DataTransfer.Sentinel;
using System;
using System.Collections.Generic;
using System.IO;


namespace MilSpace.AddDem.ReliefProcessing
{
    public interface IPrepareDemViewSrtm
    {
        string SrtmSrtorage { get; set; }
        string SrtmSrtorageExternal { get; set; }

        IEnumerable<FileInfo> SrtmFilesInfo { get; set; }

        string SelectedSrtmFile { get; }

        string TileLatitudeSrtm { get; }

        string TileLongitudeSrtm { get; }

        IEnumerable<Tile> RequestedSrtmFiles { get; }

        bool ReplaceSrtmFiles { get; }

        bool OnlyRquestedSrtmFiles { get; }
    }
}
