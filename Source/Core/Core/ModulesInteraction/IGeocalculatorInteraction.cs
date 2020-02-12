using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Core.ModulesInteraction
{
    public interface IGeocalculatorInteraction
    {
         Dictionary<int, IPoint> GetPoints();
    }
}
