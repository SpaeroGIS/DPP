using ESRI.ArcGIS.Geometry;

namespace MilSpace.DataAccess.DataTransfer.Sentinel
{
    public class SentinelFootprint
    {
        public string Uuid;
        public int Id;
        public string Identifier;
        public IPolygon Footprint;
        public int RelativeOrbit;
        public string PassDirection;
    }
}
