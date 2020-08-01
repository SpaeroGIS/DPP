using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Core.Geometry
{
    public class WktPoint : WktGeometry
    {
        public double Latitude;
        public double Longitude;

        public override WktGeometryTypesEnum GeometryType => WktGeometryTypesEnum.POINT;

        public override bool Equals(object obj)
        {
            if (obj is WktPoint point)
            {
                return point.Latitude == Latitude && point.Longitude == Longitude;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        internal override string WktGeometryDescription => ToString();
        public override string ToString()
        {
            return $"{Longitude.ToString().Replace(",", ".")} " +
                $"{Latitude.ToString().Replace(",", ",")}";
        }
    }
}
