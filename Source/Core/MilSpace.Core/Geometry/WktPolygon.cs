using Microsoft.SqlServer.Types;
using System.Collections.Generic;
using System.Linq;

namespace MilSpace.Core.Geometry
{
    public class WktPolygon : WktGeometry
    {
        List<WktPoint> pointsList;
        private string wktTemplateLevel1 = "POLYGON({0})";
        private string wktTemplateLevel2 = "({0})";
       
        public WktPolygon(string wkt) : base(wkt)
        {  }
        public WktPolygon() : this(new List<WktPoint>())
        { }

        public WktPolygon(IEnumerable<WktPoint> points)
        {
            pointsList = new List<WktPoint>(points);
            CheckTopology();
        }

        internal override void ParceWkt()
        {
            pointsList = new List<WktPoint>();
            for (int i = 1; i <= (int)Geometry.STNumPoints(); i++)
            {
                SqlGeometry ptGeometry = Geometry.STPointN(i);
                pointsList.Add(new WktPoint { Longitude = (double)ptGeometry.STX, Latitude = (double)ptGeometry.STY});
            }
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

        public override IEnumerable<WktPoint> ToPoints => pointsList;

        public override string ToString()
        {
            return
               string.IsNullOrEmpty(wkt) ?
               string.Format(wktTemplateLevel1, WktGeometryDescription) :
           wkt;
        }

        

    }
}
