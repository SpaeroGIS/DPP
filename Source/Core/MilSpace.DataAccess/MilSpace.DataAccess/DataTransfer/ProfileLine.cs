using ESRI.ArcGIS.Geometry;
using System.Xml.Serialization;

namespace MilSpace.DataAccess.DataTransfer
{
    public class ProfileLine
    {
        public ProfilePoint PointFrom;
        public ProfilePoint PointTo;
        public int Id;
        public double Angel;

        [XmlIgnore]
        public ISpatialReference SpatialReference;
    }
}
