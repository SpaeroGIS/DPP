using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Core.Geometry
{
    public class WktPolygon 
    {
        List<WktPoint> pointsList;

        public WktPolygon() : this( new List<WktPoint>())
        { }

        public WktPolygon(IEnumerable<WktPoint> points)
        {
            pointsList = new List<WktPoint>(points);
        }
    }
}
