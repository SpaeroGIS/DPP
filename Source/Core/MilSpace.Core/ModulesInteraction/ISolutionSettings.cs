using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Core.ModulesInteraction
{
    internal interface ISolutionSettings
    {
        /// <summary>
        /// Returns a Raster Layer from the map as obly value for all Add-is in the solution
        /// </summary>
        string ResterLayer { get; }
    }
}
