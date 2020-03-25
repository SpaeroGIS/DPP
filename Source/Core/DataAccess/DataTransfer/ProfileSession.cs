using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core;
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
        static Logger logger = Logger.GetLoggerEx("ProfileSession");

        // The Azimuth value  has format "{0}:{1}" 
        static readonly char[] athimuthSeparator = new char[] { ':' };
        private double[] azimuth;


        public ProfileLine[] ProfileLines;
        public ProfileSurface[] ProfileSurfaces;
        public double ObserverHeight;
        /// <summary>
        /// Gets or sets the method how the profile limes were defined
        /// </summary>
        public ProfileSettingsTypeEnum DefinitionType;
        public int SessionId;
        public string Azimuth;
        public string SessionName;
        public string SurfaceLayerName;
        public string SurfaceLayerPath;

        /// <summary>
        /// Date and time  when the Profile session was created
        /// </summary>
        [XmlIgnore]
        public DateTime CreatedOn;

        /// <summary>
        /// Shows if the profile session is availabe for share
        /// </summary>
        [XmlIgnore]
        public bool Shared;

        /// <summary>
        /// Proifile session creator
        /// </summary>
        [XmlIgnore]
        public string CreatedBy;

        [XmlIgnore]
        public double Azimuth1
        {
            get
            {

                if (Azimuth == null)
                {
                    return double.MinValue;
                }

                if (azimuth == null)
                {
                    azimuth = ParseAzimuth(Azimuth);
                }

                return azimuth[0];
            }
        }

        [XmlIgnore]
        public double Azimuth2
        {
            get
            {

                if (Azimuth == null)
                {
                    return double.MinValue;
                }

                if (azimuth == null)
                {
                    azimuth = ParseAzimuth(Azimuth);
                }

                return azimuth[1];
            }
        }

        [XmlIgnore]
        public List<GroupedLines> Segments;

        [XmlIgnore]
        public List<string> Layers;

        public string Serialized
        {
            get
            {
                return Serialize(this);
            }
        }

        public IEnumerable<IPolyline> ConvertLinesToEsriPolypile(ISpatialReference spatialReference, int lineId = -1)
        {
            Func<ProfileLine, IPolyline> converter = (l) =>
            {
                var surface = ProfileSurfaces.FirstOrDefault(s => l.Id == s.LineId);
                IPolyline result;

                if (DefinitionType == ProfileSettingsTypeEnum.Primitives && surface != null)
                {
                    var vertices = surface.ProfileSurfacePoints.Where(point => point.isVertex).Select(p => new Point { X = p.X, Y = p.Y, Z = p.Z, SpatialReference = EsriTools.Wgs84Spatialreference });
                    result = EsriTools.CreatePolylineFromPointsArray(vertices.ToArray(), spatialReference).First();
                }
                else
                {
                    var pointFrom = new Point { X = l.PointFrom.X, Y = l.PointFrom.Y, SpatialReference = EsriTools.Wgs84Spatialreference };
                    var pointTo = new Point { X = l.PointTo.X, Y = l.PointTo.Y, SpatialReference = EsriTools.Wgs84Spatialreference };

                    pointFrom.Project(spatialReference);
                    pointTo.Project(spatialReference);

                    result = EsriTools.CreatePolylineFromPoints(pointFrom, pointTo);
                }

                if (l.Line == null)
                {
                    l.Line = result;
                }

                return result;
            };

            if (lineId <= 0 || lineId > ProfileLines.Length)
            {
                return ProfileLines.Select(l => converter(l)).ToArray();
            }

            return new IPolyline[] { converter(ProfileLines.First(l => l.Id == lineId)) };
        }

        public void SetSegments(ISpatialReference spatialReference, ProfileLine profileLine = null)
        {
            if (profileLine == null)
            {
                Segments = ProfileLines.Select(line =>
                {
                    var lines = new List<ProfileLine> { line };
                    lines[0].Visible = true;

                    var polyline = new List<IPolyline> { line.Line };

                    return new GroupedLines
                    {
                        Lines = lines,
                        LineId = line.Id,
                        Polylines = polyline,
                        InvisibleColor = new RgbColor() { Red = 255, Green = 0, Blue = 0 },
                        VisibleColor = new RgbColor() { Red = 0, Green = 255, Blue = 0 },
                    };

                }).ToList();
            }
            else
            {
                var lines = new List<ProfileLine> { profileLine };
                lines[0].Visible = true;

                var polyline = new List<IPolyline> { profileLine.Line };

                Segments.Add(new GroupedLines
                {
                    Lines = lines,
                    LineId = profileLine.Id,
                    Polylines = polyline,
                    InvisibleColor = new RgbColor() { Red = 255, Green = 0, Blue = 0 },
                    VisibleColor = new RgbColor() { Red = 0, Green = 255, Blue = 0 },
                });
            }
        }

        private static double[] ParseAzimuth(string valueToParse)
        {
            if (string.IsNullOrWhiteSpace(valueToParse))
            {
                return null;
            }

            var athimutList = valueToParse.Split(athimuthSeparator, StringSplitOptions.RemoveEmptyEntries);
            if (athimutList.Length < 2)
            {
                return null;
            }

            logger.InfoEx("Try to parse Athimuth {0}".InvariantFormat(valueToParse));


            double azm1;
            double azm2;
            if (!double.TryParse(athimutList[0], out azm1))
            {
                logger.WarnEx("Can not parse value {0}".InvariantFormat(athimutList[0]));
                return new double[] { double.MinValue, double.MinValue };
            }

            if (!double.TryParse(athimutList[1], out azm2))
            {
                logger.WarnEx("Can not parse value {0}".InvariantFormat(athimutList[1]));
                return new double[] { double.MinValue, double.MinValue };
            }

            return new double[] { azm1, azm2 };


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
                    ProfileSession sessionOut = null;

                    XmlSerializer serializerOut = new XmlSerializer(typeof(ProfileSession));

                    using (var memoryStream = new MemoryStream())
                    {
                        using (var writer = new StreamWriter(memoryStream))
                        {
                            // Various for loops etc as necessary that will ultimately do this:
                            writer.Write(resu);
                            writer.Flush();


                            memoryStream.Seek(0, SeekOrigin.Begin);
                            if (serializer.Deserialize(memoryStream) is ProfileSession result)
                            {
                                sessionOut = result;
                            }
                        }
                    }

                    return Encoding.UTF8.GetString((stream as MemoryStream).ToArray());
                }
                catch (Exception ex)
                {
                    logger.ErrorEx(ex.Message);
                }

                return null;
            }
        }

    }
}
