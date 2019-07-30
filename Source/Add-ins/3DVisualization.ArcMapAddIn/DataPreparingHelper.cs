using ESRI.ArcGIS.Geometry;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer;
using System.Collections.Generic;
using System.Linq;

namespace MilSpace.Visualization3D
{
    internal static class DataPreparingHelper
    {
        static DataPreparingHelper() { }

        internal static Dictionary<ProfileSurface, bool> GetLinesSegments(ProfileSession profileSession)
        {
            if(profileSession == null || profileSession.ProfileLines == null)
            {
                return null;
            }

            var lines = new Dictionary<ProfileSurface, bool>();

            foreach(var line in profileSession.ProfileLines)
            {
                var surface = profileSession.ProfileSurfaces.First(profileSurface => profileSurface.LineId == line.Id);

                var points = new List<ProfileSurfacePoint>(surface.ProfileSurfacePoints);

                while(points.Count > 0)
                {
                    var surfaceSegment = new ProfileSurface();
                    var visibility = points.First().Visible;
                    surfaceSegment.ProfileSurfacePoints = points.TakeWhile(point =>
                                                                            point.Visible == visibility
                                                                            && (!point.isVertex || point == points.First() || point == points.Last()))                                                                           .ToArray();

                    lines.Add(surfaceSegment, visibility);
                    points.RemoveRange(0, surfaceSegment.ProfileSurfacePoints.Length);
                }
            }

            return lines;
        }

        internal static Dictionary<IPolyline, bool> GetPolylinesSegments(Dictionary<IPointCollection, bool> segments)
        {
            if(segments == null)
            {
                return null;
            }

            var polylines = new Dictionary<IPolyline, bool>();

            foreach(var segment in segments)
            {
                polylines.Add(EsriTools.Create3DPolylineFromPoints(segment.Key), segment.Value);
            }

            return polylines;
        }

        internal static Dictionary<IPointCollection, bool> GetSegmentsGeoPoints(Dictionary<ProfileSurface, bool> segments)
        {
            var geoPoints = new Dictionary<IPointCollection, bool>();

            foreach(var segment in segments)
            {
                geoPoints.Add(GetPoints(segment.Key), segment.Value);
            }

            return geoPoints;
        }


        internal static IPoint GetObserverPoint(double observerHeight, ProfileSurfacePoint firstPoint)
        {
            IPoint geoPoint = new Point() { X = firstPoint.X, Y = firstPoint.Y, Z = firstPoint.Z, SpatialReference = EsriTools.Wgs84Spatialreference };
            return EsriTools.GetObserverPoint(geoPoint, observerHeight, ArcMap.Document.FocusMap.SpatialReference);
        }

        internal static Dictionary<IPolygon, bool> GetVisibilityPolygons(IPoint observerPoint, Dictionary<IPointCollection, bool> pointCollections)
        {
            var visibilityPolygons = new Dictionary<IPolygon, bool>();

            foreach(var points in pointCollections)
            {
                var polygonPoints = new PathClass();
                polygonPoints.AddPoint(observerPoint);
                polygonPoints.AddPointCollection(points.Key);

                visibilityPolygons.Add(EsriTools.GetVisilityPolygon(polygonPoints), points.Value);
            }

            return visibilityPolygons;
        }

        private static IPointCollection GetPoints(ProfileSurface surface)
        {
            var points = new PathClass();

            foreach(var surfacePoint in surface.ProfileSurfacePoints)
            {
                IPoint point = new Point() { X = surfacePoint.X, Y = surfacePoint.Y, Z = surfacePoint.Z, SpatialReference = EsriTools.Wgs84Spatialreference };
                point.Project(ArcMap.Document.FocusMap.SpatialReference);
                points.AddPoint(point);
            }

            return points;
        }
    }
}
