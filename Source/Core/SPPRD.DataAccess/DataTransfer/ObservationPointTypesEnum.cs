using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.DataAccess.DataTransfer
{
    public enum ObservationPointTypesEnum 
    {
        All = 0,
        Undefined,
        Our,
        Enemy,
        Neutrality,
    }

    public enum ObservationPointMobilityTypesEnum
    {
        All = 0,
        Stationary,
        Mobile
    }
}
