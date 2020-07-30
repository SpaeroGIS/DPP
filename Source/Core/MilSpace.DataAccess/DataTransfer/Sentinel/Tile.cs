using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.DataAccess.DataTransfer.Sentinel
{
    public class Tile
    {

        bool isEmpty = true;

        public Tile()
        { }

        public Tile(string tileName)
        {
            if (tileName.StartsWith(North.ToString(), StringComparison.InvariantCultureIgnoreCase) || tileName.StartsWith(West.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                //    var lat = tileName.Replace(North, "").Replace(West, "");

                string estOrWest = East.ToString();
                string northOrSouth = tileName.Substring(0, 1);

                int latIndex = 1;
                int lonIndex = -1;

                string lon = null;
                string lat = null;


                if (tileName.IndexOf(East) > 0)
                {
                    lonIndex = tileName.IndexOf(East);
                }
                if (tileName.IndexOf(West) > 0)
                {
                    lonIndex = tileName.IndexOf(West);
                    estOrWest = West.ToString();
                }

                lon = tileName.Substring(latIndex, lonIndex - latIndex);
                lat = tileName.Substring(lonIndex + 1);

                if (int.TryParse(lon, out Lon) && int.TryParse(lat, out Lat))
                {
                    isEmpty = false;
                }

                if (estOrWest == West.ToString())
                {
                    Lon *= -1;
                }

                if (northOrSouth == South.ToString())
                {
                    Lat *= -1;
                }
            }
        }

        private const char North = 'n';
        private const char South = 's';
        private const char East = 'e';
        private const char West = 'w';

        public int Lon;
        public int Lat;
        public string Name
        {
            get
            {

                //n50e036
                string ns = Lat > 0 ? North.ToString() : South.ToString();
                string ew = Lon > 0 ? East.ToString() : West.ToString();
                return $"{ns}{Math.Abs(Lat)}{ew}{Math.Abs(Lon)}";
            }
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is Tile tile)
            {
                return tile.GetHashCode() == GetHashCode();
            }

            return false;
        }

        public override int GetHashCode()
        {
            return (Lat * 2 * (Lat > 0 ? (int)North : (int)South)) +
                (Lon * (Lon > 0 ? (int)East : (int)West));
        }
    }
}
