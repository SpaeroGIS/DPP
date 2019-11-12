using ESRI.ArcGIS.Carto;
using MilSpace.DataAccess.DataTransfer;
using System.Collections.Generic;

namespace MilSpace.Visualization3D.Models
{
    public class ArcSceneArguments
    {
        internal double ZFactor { get; set; } = 1;
        internal string DemLayer { get; set; }
        internal string Line3DLayer { get; set; }
        internal string Point3DLayer { get; set; }
        internal string Polygon3DLayer { get; set; }
        internal List<ILayer> AdditionalLayers { get; set; }
        internal List<VisibilityResultInfo> VisibilityResultsInfo { get; set; }
    }
}
