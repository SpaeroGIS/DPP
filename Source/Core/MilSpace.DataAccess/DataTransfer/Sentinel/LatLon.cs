using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.DataAccess.DataTransfer.Sentinel
{
    public class LatLon
    {
        public double Lat;
        public double Lon;

        public override bool Equals(object obj)
        {
            if (obj == null )
            {
                return false;
            }
            if (obj is LatLon latLon)
            {
                return Math.Abs(Lat - latLon.Lat) <= 0.001 &&
                    Math.Abs(Lon - latLon.Lon) <= 0.0015;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
