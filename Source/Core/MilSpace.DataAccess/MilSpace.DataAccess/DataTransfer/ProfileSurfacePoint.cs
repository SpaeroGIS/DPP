using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MilSpace.DataAccess.DataTransfer
{
    public class ProfileSurfacePoint
    {
        public double Distance;
        public double Z;
        public double X;
        public double Y;

        [XmlIgnore]
        public LayersEnum Layers;
    }
}
