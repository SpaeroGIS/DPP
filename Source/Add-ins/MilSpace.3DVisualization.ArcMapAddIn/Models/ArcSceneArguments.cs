using ESRI.ArcGIS.Geodatabase;
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
        internal IFeatureClass Line3DLayer { get; set; }
        internal IFeatureClass Point3DLayer { get; set; }
        internal IFeatureClass Polygon3DLayer { get; set; }
    }
}
