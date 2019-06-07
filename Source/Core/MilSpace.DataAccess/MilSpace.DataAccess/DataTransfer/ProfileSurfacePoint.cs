using System.Xml.Serialization;

namespace MilSpace.DataAccess.DataTransfer
{
    public class ProfileSurfacePoint
    {
        public double Distance;
        public double Z;
        public double X;
        public double Y;

        public bool isVertex;
        [XmlIgnore]
        public LayersEnum Layers;

        [XmlIgnore]
        public bool IsEmpty => Distance == 0 && Z == 0 && X == 0 && Y == 0;
    }
}
