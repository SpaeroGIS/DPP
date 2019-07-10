using ESRI.ArcGIS.Geometry;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MilSpace.DataAccess.DataTransfer
{
    public class ProfileLine
    {
        public ProfilePoint PointFrom;
        public ProfilePoint PointTo;
        public int Id;
        public double Angel;
        public double Azimuth;
        public double Length;
        public bool Visible;
        public ProfilePoint[] PointCollection;

        [XmlIgnore]
        public IPolyline Line;

        [XmlIgnore]
        public ISpatialReference SpatialReference;

        [XmlIgnore]
        public int SessionId;

        [XmlIgnore]
        public IEnumerable<IPoint> Vertices
        {
            get
            {
                IPointCollection collection = Line as IPointCollection;
                IPoint[] vertices = new IPoint[collection.PointCount];

                for ( int i = 0; i < collection.PointCount; i++ )
                {
                    vertices[i] = collection.Point[i];
                }

                return vertices;
            }
        }
    }
}
