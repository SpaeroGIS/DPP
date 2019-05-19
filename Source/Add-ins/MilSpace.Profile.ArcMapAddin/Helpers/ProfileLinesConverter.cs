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

            return polylines.Select(line =>
               {
                   id++;

                   line.Project(EsriTools.Wgs84Spatialreference);

                   var pointFrom = new ProfilePoint { SpatialReference = line.SpatialReference, X = line.FromPoint.X, Y = line.FromPoint.Y };
                   var pointTo = new ProfilePoint { SpatialReference = line.SpatialReference, X = line.ToPoint.X, Y = line.ToPoint.Y };

                   return new ProfileLine
                   {
                       Line = line,
                       Id = id,
                       PointFrom = pointFrom,
                       PointTo = pointTo
                   };
               }
                ).ToList();
        }

        public static List<IntersectionLine> ConvertEsriPolylineToIntersectionLines(List<IPolyline> polylines, IPoint fromPoint, LayersEnum layer)
        {
            var id = 0;

            return polylines.Select(line =>
            {
                id++;

                var startDistance = EsriTools.CreatePolylineFromPoints(fromPoint, line.FromPoint).Length;
                var endDistance = EsriTools.CreatePolylineFromPoints(fromPoint, line.ToPoint).Length;

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

            foreach(var line in groupedLines)
            {
                lines.AddRange(line.Lines);
            }

            return ConvertLineToEsriPolyline(lines, spatialReference);
        }
    }
}
