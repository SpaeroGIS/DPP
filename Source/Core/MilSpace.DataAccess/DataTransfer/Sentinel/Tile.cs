using ESRI.ArcGIS.Geometry;
using MilSpace.Core.Geometry;
using MilSpace.Core.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.DataAccess.DataTransfer.Sentinel
{
    public class Tile
    {
        IWktGeometry geometry = null;
        bool isEmpty = true;

        public Tile()
        { }

        public Tile(string tileName)
        {
            tileName = tileName.Replace("_", "");
            if (tileName.StartsWith(North.ToString(), StringComparison.InvariantCultureIgnoreCase) || tileName.StartsWith(West.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
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

                lat = tileName.Substring(latIndex, lonIndex - latIndex);
                lon = tileName.Substring(lonIndex + 1);

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

                //n50e36
                string ns = Lat > 0 ? North.ToString() : South.ToString();
                string ew = Lon > 0 ? East.ToString() : West.ToString();
                return $"{ns}{Math.Abs(Lat)}{ew}{Math.Abs(Lon)}";
            }
        }

        public string FullName
        {
            get
            {
                //n50e036
                string ns = Lat > 0 ? North.ToString() : South.ToString();
                string ew = Lon > 0 ? East.ToString() : West.ToString();

                var lon = Math.Abs(Lon).ToString().PadLeft(3, '0');
                return $"{ns}{Math.Abs(Lat)}_{ew}{lon}";
            }
        }
        public bool IsEmpty => isEmpty;

        public IWktGeometry Geometry
        {
            get
            {
                if (geometry == null)
                {
                    var points = new List<WktPoint>
                    {
                        new WktPoint { Latitude = Lat, Longitude = Lon},
                        new WktPoint { Latitude = Lat+1, Longitude = Lon},
                        new WktPoint { Latitude = Lat+1, Longitude = Lon+1},
                        new WktPoint { Latitude = Lat, Longitude = Lon+1},
                        new WktPoint { Latitude = Lat, Longitude = Lon},
                    };

                    geometry = new WktPolygon(points);
                }
                return geometry;
            }
        }

        public IEnvelope EsriGeometry
        {
            get
            {
                var pointCollection = new MultipointClass();
                pointCollection.SpatialReference = EsriTools.Wgs84Spatialreference;
                Geometry.ToPoints.ToList().ForEach(wktPoint =>
                {
                    var pnt = new Point
                    {
                        X = wktPoint.Longitude,
                        Y = wktPoint.Latitude,
                    };
                    pnt.SpatialReference = EsriTools.Wgs84Spatialreference;
                    pointCollection.AddPoint(pnt);
                   });

                return EsriTools.GetPolygonByPointCollection(pointCollection).Envelope;
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

        public static IEnumerable<Tile> GetTilesFormFile(string pathTiFile)
        {
            IEnumerable<Tile> result = null;

            if (File.Exists(pathTiFile))
            {
                var lines = File.ReadAllLines(pathTiFile);

                result = lines.Select(t => new Tile(t)).Where(t => !t.IsEmpty).ToArray();
            }

            return result;
        }
    }
}
