using ESRI.ArcGIS.Geometry;
using MilSpace.Core.Tools;
using MilSpace.DataAccess;
using MilSpace.DataAccess.DataTransfer;
using System.Collections.Generic;
using System.Linq;

namespace MilSpace.Profile
{
    public static class ProfileLinesConverter
    {
        public static List<IPolyline> ConvertLineToEsriPolyline(List<ProfileLine> profileLines, ISpatialReference spatialReference)
        {
            return profileLines.Select(l =>
            {
                var pointFrom = new Point { X = l.PointFrom.X, Y = l.PointFrom.Y, SpatialReference = EsriTools.Wgs84Spatialreference };
                var pointTo = new Point { X = l.PointTo.X, Y = l.PointTo.Y, SpatialReference = EsriTools.Wgs84Spatialreference };

                pointFrom.Project(spatialReference);
                pointTo.Project(spatialReference);

                return EsriTools.CreatePolylineFromPoints(pointFrom, pointTo);
            }
             ).ToList();
        }

        public static List<ProfileLine> ConvertEsriPolylineToLine(List<IPolyline> polylines)
        {
            var id = 0;
            var spatialReference = polylines[0].SpatialReference;

            var esriPolylines = new List<IPolyline>(polylines);

            return esriPolylines.Select(line =>
               {
                   id++;

                   line.Project(EsriTools.Wgs84Spatialreference);

                   var pointFrom = new ProfilePoint { SpatialReference = line.SpatialReference, X = line.FromPoint.X, Y = line.FromPoint.Y };
                   var pointTo = new ProfilePoint { SpatialReference = line.SpatialReference, X = line.ToPoint.X, Y = line.ToPoint.Y };

                   line.Project(spatialReference);

                   return new ProfileLine
                   {
                       Line = line,
                       Id = id,
                       PointFrom = pointFrom,
                       PointTo = pointTo,
                   };
               }
                ).ToList();
        }

        public static List<IntersectionLine> ConvertEsriPolylineToIntersectionLines(List<IPolyline> polylines, ProfilePoint pointFrom, LayersEnum layer)
        {
            var id = 0;
            var fromPoint = new Point { X = pointFrom.X, Y = pointFrom.Y, SpatialReference = EsriTools.Wgs84Spatialreference };

            return polylines.Select(line =>
            {
                id++;
                fromPoint.Project(line.SpatialReference);


                var fromLength = EsriTools.CreatePolylineFromPoints(fromPoint, line.FromPoint).Length;
                var toLength = EsriTools.CreatePolylineFromPoints(fromPoint, line.ToPoint).Length;

                double startDistance = (fromLength < toLength) ? fromLength : toLength;
                double endDistance = (fromLength > toLength) ? fromLength : toLength;

                return new IntersectionLine()
                {
                    PointFromDistance = startDistance,
                    PointToDistance = endDistance,
                    LayerType = layer
                };
            }
                ).ToList();
        }

        public static IEnumerable<IPolyline> ConvertSolidGroupedLinesToEsriPolylines(List<GroupedLines> groupedLines,
                                                                                        ISpatialReference spatialReference)
        {
            var lines = new List<ProfileLine>();

            foreach (var line in groupedLines)
            {
                lines.AddRange(line.Lines);
            }

            return ConvertLineToEsriPolyline(lines, spatialReference);
        }

        public static List<GroupedLines> GetSegmentsFromProfileLine(ProfileSurface[] profileSurfaces, ISpatialReference spatialReference)
        {
            var polylines = ConvertLineToPrimitivePolylines(profileSurfaces, spatialReference);
            var lines = new GroupedLines()
            {
                Polylines = polylines,
                Lines = ConvertEsriPolylineToLine(polylines),
                LineId = 1,
                IsPrimitive = true
            };

            return new List<GroupedLines>() { lines };

        }

        public static List<IPolyline> ConvertLineToPrimitivePolylines(ProfileSurface[] profileSurfaces, ISpatialReference spatialReference)
        {
            var polylines = new List<IPolyline>();

            foreach (var surface in profileSurfaces)
            {
                polylines.AddRange(SeparatePrimitives(surface.ProfileSurfacePoints.Where(point => point.isVertex).ToList(), spatialReference));
            }

            return polylines;
        }

        private static List<IPolyline> SeparatePrimitives(IEnumerable<ProfileSurfacePoint> vertices/*ProfileSurface[] profileSurfaces*/, ISpatialReference spatialReference)
        {
            var verticesArray = vertices.ToArray();
            var polylines = new List<IPolyline>();

            for (int i = 0; i < vertices.Count() - 1; i++)
            {
                var pointFrom = new Point { X = verticesArray[i].X, Y = verticesArray[i].Y, Z = verticesArray[i].Z, SpatialReference = EsriTools.Wgs84Spatialreference };
                var pointTo = new Point { X = verticesArray[i + 1].X, Y = verticesArray[i + 1].Y, Z = verticesArray[i + 1].Z, SpatialReference = EsriTools.Wgs84Spatialreference };

                pointFrom.Project(spatialReference);
                pointTo.Project(spatialReference);

                polylines.Add(EsriTools.CreatePolylineFromPoints(pointFrom, pointTo));
            }

            return polylines;
        }
    }
}
