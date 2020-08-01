using System.Collections.Generic;
using System.Linq;

namespace MilSpace.Core.Geometry
{
    public class WktPolygon : WktGeometry
    {
        List<WktPoint> pointsList;
        private string wktTemplateLevel1 = "POLYGON({0})";
        private string wktTemplateLevel2 = "({0})";

        public WktPolygon(string wkt)
        { this.wkt = wkt; }
        public WktPolygon() : this(new List<WktPoint>())
        { }

        public WktPolygon(IEnumerable<WktPoint> points)
        {
            pointsList = new List<WktPoint>(points);
            CheckTopology();
        }

        private void CheckTopology()

        {
            if (pointsList.Last().Equals(pointsList.First()))
            {
                return;
            }
            pointsList.Add(pointsList.First());
        }

        internal override string WktGeometryDescription => string.Format(wktTemplateLevel2, string.Join(", ", pointsList.Select(p => p.WktGeometryDescription)));

        public override WktGeometryTypesEnum GeometryType => WktGeometryTypesEnum.POLYGON;

        public override string ToString()
        {
            return
               string.IsNullOrEmpty(wkt) ?
               string.Format(wktTemplateLevel1, WktGeometryDescription) :
           wkt;
        }

    }
}
