using ESRI.ArcGIS.Geometry;
using System.Xml.Serialization;

namespace MilSpace.DataAccess.DataTransfer
{
    public class ProfilePoint
    {
        public double X;
        public double Y;
        [XmlIgnore]
        public ISpatialReference SpatialReference;
    }
}
