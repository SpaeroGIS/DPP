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
        public double ObserverHeight;
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

                var result = EsriTools.CreatePolylineFromPoints(pointFrom, pointTo);
                if (l.Line == null)
                {
                    l.Line = result;
                }

                return result;
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

                    var resu = Encoding.UTF8.GetString((stream as MemoryStream).ToArray());


                    //
                    ProfileSession sessionOut = null;

                    XmlSerializer serializerOut = new XmlSerializer(typeof(ProfileSession));

                    using (var memoryStream = new MemoryStream())
                    {
                        using (var writer = new StreamWriter(memoryStream))
                        {
                            // Various for loops etc as necessary that will ultimately do this:
                            writer.Write(resu);
                            writer.Flush();

                            try
                            {
                                memoryStream.Seek(0, SeekOrigin.Begin);
                                if (serializer.Deserialize(memoryStream) is ProfileSession result)
                                {
                                    sessionOut = result;
                                }
                            }
                            catch (Exception ex)
                            {
                                //TODO: log the error
                                //throw new MilSpaceDataException("MilSp_Profile", DataOperationsEnum.Convert, ex);
                            }
                        }
                    }
                    //

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
