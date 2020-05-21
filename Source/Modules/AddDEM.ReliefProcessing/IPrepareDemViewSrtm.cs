using System;
using System.Collections.Generic;
using System.IO;


namespace MilSpace.AddDem.ReliefProcessing
{
    public interface IPrepareDemViewSrtm
    {
        string SrtmSrtorage { get; set; }
        IEnumerable<FileInfo> SrtmFilesInfo { get; set; }
    }
}
