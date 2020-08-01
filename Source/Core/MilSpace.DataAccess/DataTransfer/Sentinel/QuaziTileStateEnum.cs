using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.DataAccess.DataTransfer.Sentinel
{
    [Flags]
    public enum QuaziTileStateEnum
    {
        SplitOngoig = 1,
        Snaphu = 2,
        Dem = 4,
        Finished = 8,
    }
}
