using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Visualization3D.Models
{
    public class ArcSceneArguments
    {
        internal string DemLayer { get; set; }
        internal string Line3DLayer { get; set; }
        internal string Point3DLayer { get; set; }
        internal string Polygon3DLayer { get; set; }
        internal ISpatialReference SpatialReference { get; set; }
    }
}
