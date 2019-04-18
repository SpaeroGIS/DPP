using ESRI.ArcGIS.Geometry;
using MilSpace.Core.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MilSpace.DataAccess.DataTransfer
{
    public class ProfileSession
    {
        public ProfileLine[] ProfileLines;
        public ProfileSurface[] ProfileSurfaces;
        /// <summary>
        /// Gets or sets the method how the profile limes were defined
        /// </summary>
        public ProfileSettingsTypeEnum DefinitionType;
        public int SessionId;
        public string SessionName;

        [XmlIgnore]
        public string Serialized
        {
            get
            {

                return Serialize(this);
            }
        }

        public IEnumerable<IPolyline> ConvertLinesToEsriPolypile(ISpatialReference spatialReference)
        {
           return ProfileLines.Select(l =>
            {
                var pointFrom = new Point { X = l.PointFrom.X, Y = l.PointFrom.Y, SpatialReference = EsriTools.Wgs84Spatialreference};
                var pointTo = new Point { X = l.PointTo.X, Y = l.PointTo.Y, SpatialReference = EsriTools.Wgs84Spatialreference };

                pointFrom.Project(spatialReference);
                pointTo.Project(spatialReference);

                return EsriTools.CreatePolylineFromPoints(pointFrom, pointTo);
            }
            ).ToArray();
        }

        private static string Serialize(ProfileSession session)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ProfileSession));


            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, session);
                try
                {
                    return Encoding.UTF8.GetString((stream as MemoryStream).ToArray());
                }
                catch (Exception ex)
                {
                    //TODO: log the error
                }

                return null;
            }
        }



    }
}
