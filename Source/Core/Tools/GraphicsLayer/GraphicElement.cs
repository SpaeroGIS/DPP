using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;

namespace MilSpace.Tools.GraphicsLayer
{
    public class GraphicElement
    {
        public IElement Element;
        public int ProfileId;
        public int ElementId;
        public int LineId;
        public string Name;
        public IGeometry Source;
    }
}
