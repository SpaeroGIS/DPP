using System.Collections.Generic;
using System.Linq;

namespace MilSpace.Core.Geometry
{
    public class WktMulyiPolygon : WktGeometry
    {
        List<WktPolygon> multipolygon;
        private string wktTemplateLevel1 = "MULTIPOLYGON({0})";
        private string wktTemplateLevel2 = "({0})";

        public WktMulyiPolygon(string wkt)
        {
            this.wkt = wkt;
        }
        public WktMulyiPolygon() : this(new List<WktPolygon>())
        {
        }

        public WktMulyiPolygon(List<WktPolygon> polygons)
        {
            multipolygon = polygons;
        }

        public void AddRing(WktPolygon ring)
        {
            multipolygon.Add(ring);
        }

        internal override string WktGeometryDescription => string.Format(wktTemplateLevel2, string.Join(", ", multipolygon.Select(p => p.ToString())));


        public override WktGeometryTypesEnum GeometryType => WktGeometryTypesEnum.MULTIPOLYGON;

        public override string ToString()
        {
            return
                string.IsNullOrEmpty(wkt) ?
                string.Format(wktTemplateLevel1, WktGeometryDescription) :
            wkt;
        }
    }
}
