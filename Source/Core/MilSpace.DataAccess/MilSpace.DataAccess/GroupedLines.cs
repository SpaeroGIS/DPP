using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using MilSpace.DataAccess.DataTransfer;
using System.Collections.Generic;

namespace MilSpace.DataAccess
{
    public class GroupedLines
    {
        public List<ProfileLine> Lines { get; set; }
        public List<IPolyline> Polylines { get; set; }
        public List<ProfilePoint> Vertices { get; set; }
        public int LineId { get; set; }
        public RgbColor VisibleColor { get; set; }
        public RgbColor InvisibleColor { get; set; }
        public bool IsSelected { get; set; }
        public bool IsPrimitive { get; set; }
    }
}
