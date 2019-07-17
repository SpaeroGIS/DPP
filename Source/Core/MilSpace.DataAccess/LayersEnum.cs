using System;

namespace MilSpace.DataAccess
{
    [Flags]
    public enum LayersEnum
    {
        Vegetation = 1,
        Buildings = 2,
        Roads = 4,
        Hydrography = 8,
        NotIntersect = 16
    }
}
