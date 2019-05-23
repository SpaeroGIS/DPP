using System;

namespace MilSpace.DataAccess
{
    [Flags]
    public enum LayersEnum
    {
        Vegetation = 1,
        Buildings = 2,
        Hydrography = 4,
        Roads = 8,
        NotIntersect = 16
    }
}
