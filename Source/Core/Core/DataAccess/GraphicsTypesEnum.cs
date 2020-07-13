using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Core.DataAccess
{
    public enum GraphicsTypesEnum
    {
        None = 0,
        Geocalculator = 1,
        Profile = 2,
        Visibility = 4,
        All = 8,
        Solution = 16,
        AllButSolution = 32,
    }
}
