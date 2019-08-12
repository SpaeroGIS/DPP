using ESRI.ArcGIS.Carto;
using System.Collections.Generic;

namespace MilSpace.Visualization3D.Models
{
    public class ArcSceneArguments
    {
        internal string DemLayer { get; set; }
        internal string Line3DLayer { get; set; }
        internal string Point3DLayer { get; set; }
        internal string Polygon3DLayer { get; set; }
        internal List<ILayer> AdditionalLayers { get; set; }
    }
}
