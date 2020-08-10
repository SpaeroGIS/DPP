using Microsoft.SqlServer.Types;
using System.Collections.Generic;
using System.Linq;

namespace MilSpace.Core.Geometry
{
    public class WktMulyiPolygon : WktGeometry
    {
        List<WktPolygon> multipolygon;
        private string wktTemplateLevel1 = "MULTIPOLYGON({0})";
        private string wktTemplateLevel2 = "({0})";

        public WktMulyiPolygon(string wkt) :base(wkt)
        {
            
        }
        public WktMulyiPolygon() : this(new List<WktPolygon>())
        {
        }

        public WktMulyiPolygon(List<WktPolygon> polygons) :base()
        {
            multipolygon = polygons;
        }

        public void AddRing(WktPolygon ring)
        {
            multipolygon.Add(ring);
        }

        internal override string WktGeometryDescription => string.Format(wktTemplateLevel2, string.Join(", ", multipolygon.Select(p => p.ToString())));

        internal override void ParceWkt()
        {
            multipolygon = new List<WktPolygon>();
            for (int i = 1; i <= (int)Geometry.STNumGeometries(); i++)
            {
                SqlGeometry ptGeometry = Geometry.STGeometryN(i);
                WktPolygon pl = new WktPolygon(ptGeometry.ToString());
                multipolygon.Add(pl);
            }
        }
        public override WktGeometryTypesEnum GeometryType => WktGeometryTypesEnum.MULTIPOLYGON;

        public override IEnumerable<WktPoint> ToPoints => multipolygon.SelectMany( pp => pp.ToPoints);

        public override string ToString()
        {
            return
                string.IsNullOrEmpty(wkt) ?
                string.Format(wktTemplateLevel1, WktGeometryDescription) :
            wkt;
        }
    }
}
