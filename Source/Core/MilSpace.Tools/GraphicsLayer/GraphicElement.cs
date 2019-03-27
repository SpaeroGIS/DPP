using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Tools.GraphicsLayer
{
    public class GraphicElement
    {
        public IElement Element;
        public int ProfileId;
        public int ElementId;
        public IPolyline Source;
    }
}
