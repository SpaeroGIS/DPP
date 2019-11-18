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
                var visibility = surface.ProfileSurfacePoints.First().Visible;
                var segmentPoints = new List<ProfileSurfacePoint>();

                var points = surface.ProfileSurfacePoints;

                for(int i = 1; i < points.Length; i++)
                {
                    segmentPoints.Add(points[i]);
                    if(visibility != surface.ProfileSurfacePoints[i].Visible || surface.ProfileSurfacePoints[i].isVertex || i == surface.ProfileSurfacePoints.Length - 1)
                    {
                        lines.Add(new ProfileSurface() { ProfileSurfacePoints = segmentPoints.ToArray() }, visibility);
                        visibility = !visibility;
                        segmentPoints = new List<ProfileSurfacePoint>();
                        segmentPoints.Add(points[i]);
                    }
                }
            }

            return lines;
        }

        internal static Dictionary<ProfileSurface, bool> GetLinesSegmentsForPrimitive(ProfileSurface segment)
        {
            var lines = new Dictionary<ProfileSurface, bool>();

            var visibility = segment.ProfileSurfacePoints.First().Visible;
            var segmentPoints = new List<ProfileSurfacePoint>();

            var points = segment.ProfileSurfacePoints;

            for(int i = 1; i < points.Length; i++)
            {
                segmentPoints.Add(points[i]);
                if(visibility != segment.ProfileSurfacePoints[i].Visible || i == segment.ProfileSurfacePoints.Length - 1)
                {
                    lines.Add(new ProfileSurface() { ProfileSurfacePoints = segmentPoints.ToArray() }, visibility);
                    visibility = !visibility;
                    segmentPoints = new List<ProfileSurfacePoint>();
                    segmentPoints.Add(points[i]);
                }
            }

            return lines;
        }

        internal static List<ProfileSurface> GetPrimitiveSegments(ProfileSession profileSession)
        {
            var segmentSurfaces = new List<ProfileSurface>();

            foreach(var line in profileSession.ProfileLines)
            {
                var surface = profileSession.ProfileSurfaces.First(profileSurface => profileSurface.LineId == line.Id);
                var segment = new List<ProfileSurfacePoint>();

                foreach(var point in surface.ProfileSurfacePoints)
                {
                    segment.Add(point);

                    if(point.isVertex)
                    {
                        if(point.Distance != 0)
                        {
                            segmentSurfaces.Add(new ProfileSurface
                            {
                                LineId = surface.LineId,
                                ProfileSurfacePoints = segment.ToArray()
                            });

                            segment = new List<ProfileSurfacePoint>();
                            segment.Add(point);
                        }
                    }
                }
            }

            return segmentSurfaces;

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
                var polylines = new List<IPolyline>();

                if(points.Value)
                {
                    polylines.Add(EsriTools.Create3DPolylineFromPoints(observerPoint, points.Key.Point[0]));
                    for(int i = 0; i < points.Key.PointCount - 1; i++)
                    {
                        polylines.Add(EsriTools.Create3DPolylineFromPoints(points.Key.Point[i], points.Key.Point[i + 1]));
                    }
                    polylines.Add(EsriTools.Create3DPolylineFromPoints(observerPoint, points.Key.Point[points.Key.PointCount - 1]));

                }
                else
                {
                    for(int i = 0; i < points.Key.PointCount - 1; i++)
                    {
                        polylines.Add(EsriTools.Create3DPolylineFromPoints(points.Key.Point[i], points.Key.Point[i + 1]));
                    }
                }

                visibilityPolygons.Add(EsriTools.GetVisilityPolygon(polylines), points.Value);
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
