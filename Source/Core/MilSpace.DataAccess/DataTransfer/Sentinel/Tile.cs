using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.DataAccess.DataTransfer.Sentinel
{
    public class Tile
    {
        private const string North = "n";
        private const string South = "s";
        private const string East = "e";
        private const string West = "w";

        public int Lon;
        public int Lat;
        public string Name
        {
            get {
                //n50e036
                string ns = Lat > 0 ? North : South;
                string ew = Lon > 0.0 ? East : West;
                return $"{ns}{Math.Abs(Lat)}{ew}{Math.Abs(Lon)}";
            }
        }
    }
}
