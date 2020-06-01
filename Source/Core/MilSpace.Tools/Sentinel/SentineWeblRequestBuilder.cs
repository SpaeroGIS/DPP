using MilSpace.Configurations;
using MilSpace.Core;
using MilSpace.DataAccess.DataTransfer.Sentinel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MilSpace.Tools.Sentinel
{
    public class SentineWeblRequestBuilder
    {
        private static string wktTempleate0 = "POLYGON ({0},{1},{2},{3},{0})";
        private static string wktPointTemplate = "{0} {1}";
        private static string footPrintTemplate = "footprint:%22Intersects({0})%22";

        List<Tile> tiles = new List<Tile>();

        public IEnumerable<Tile> Tiles { get => tiles; }

        public void AddTile(Tile tile)
        {
            tiles.Add(tile);
        }

        public DateTime Position { get; set; }
        public string PlatformName { get; set; } = "Sentinel-1";

        public string FileName { get; set; } = "S1A_*";
        public string ProductType { get; set; } = "SLC";
        public string SensorOperationalMode { get; set; } = "IW";

        string urlTmpl = "?filter=({0})%20AND%20({1})%20AND%20({2})&offset=0&limit=150&sortedby=ingestiondate&order=desc";

        public string GetMetadataUrl
        {
            get
            {
                var rootUrl = MilSpaceConfiguration.DemStorages.ScihubMetadataApi;
                var prmtrs = $"{urlTmpl.InvariantFormat(GeoFootPrintParam(Tiles), GetPositionParam(Position), GetTheRestParams())}";
                return rootUrl + prmtrs;
            }
        }

        private static string GeoFootPrintParam(IEnumerable<Tile> tiles)
        {

            int maxLat = tiles.Max(t => t.Lat) + 1;
            int minLat = tiles.Min(t => t.Lat);
            int maxLon = tiles.Max(t => t.Lon) + 1;
            int minLon = tiles.Min(t => t.Lon);

            return string.Format(footPrintTemplate, Uri.EscapeUriString($"POLYGON (({minLat}.0 {minLon}.0,{minLat}.0 {maxLon}.0,{maxLat}.0 {maxLon}.0,{maxLat}.0 {minLon}.0,{minLat}.0 {minLon}.0))"));
        }

        private static string GetPositionParam(DateTime date)
        {
            var position = date.AddDays(-24).ToUniversalTime().ToString("o");
            string start = date.ToUniversalTime().ToString("o");

            //position = "2020-03-27T00:00:00.000Z";
            //start = "2020-04-08T23:59:59.999Z";
            return $"beginPosition:[{position}%20TO%20{start}]%20AND%20endPosition:[{position}%20TO%20{start}]";
        }

        private string GetTheRestParams()
        {
            return $"platformname:{PlatformName}%20AND%20filename:{FileName}%20AND%20producttype:{ProductType}%20AND%20sensoroperationalmode:{SensorOperationalMode}";
        }

    }
}
