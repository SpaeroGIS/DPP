using ESRI.ArcGIS.Geometry;
using System.Xml.Serialization;

namespace MilSpace.DataAccess.DataTransfer
{
    public class ProfilePoint
    {
        public double X;
        public double Y;
        public double Z;
        [XmlIgnore]
        public ISpatialReference SpatialReference;
    }
}
