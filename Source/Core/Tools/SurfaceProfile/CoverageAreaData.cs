using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Tools.SurfaceProfile
{
    public class CoverageAreaData
    {
        internal int PointId { get; set; }
        internal int ObjId { get; set; }
        internal IPolygon Polygon { get; set; }
    }
}
