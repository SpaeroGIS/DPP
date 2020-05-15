using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.AddDem.ReliefProcessing
{
    public interface IPrepareDemView
    {
        string SentinelSrtorage { get; set; }
        string SrtmSrtorage { get; set; }
    }
}
