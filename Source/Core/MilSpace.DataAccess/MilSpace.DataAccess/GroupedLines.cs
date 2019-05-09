using MilSpace.DataAccess.DataTransfer;
using ESRI.ArcGIS.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESRI.ArcGIS.Geometry;

namespace MilSpace.DataAccess
{
    public class GroupedLines
    {
        public List<ProfileLine> Lines { get; set; }
        public IEnumerable<IPolyline> Polylines { get; set; }
        public int LineId { get; set; }
        public RgbColor VisibleColor { get; set; }
        public RgbColor InvisibleColor { get; set; }
    }
}
