using ESRI.ArcGIS.Geometry;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer;
using System.Collections.Generic;
using System.Linq;

namespace MilSpace.Visualization3D
{
    internal static class DataPreparingHelper
    {
         static DataPreparingHelper(){ }

        internal static Dictionary<IPolyline, bool> GetPolylinesSegments(ProfileSession profileSession)
        {
            if(profileSession == null || profileSession.ProfileLines == null)
            {
                return null;
            }

            var polylines = new Dictionary<IPolyline, bool>();

            foreach(var line in profileSession.ProfileLines)
            {
                var segmentFromPoint = new ProfileSurfacePoint();
                var surfacePoints = profileSession.ProfileSurfaces.First(profileSurface => profileSurface.LineId == line.Id).ProfileSurfacePoints;
                segmentFromPoint = surfacePoints.First();

                for(int i = 1; i < surfacePoints.Length; i++)
                {
                    if(segmentFromPoint.Visible != surfacePoints[i].Visible || surfacePoints[i].isVertex || i == surfacePoints.Length - 1)
                    {
                        var fromPoint = new Point() { X = segmentFromPoint.X, Y = segmentFromPoint.Y, Z = segmentFromPoint.Z, SpatialReference = EsriTools.Wgs84Spatialreference };
                        var toPoint = new Point() { X = surfacePoints[i].X, Y = surfacePoints[i].Y, Z = surfacePoints[i].Z, SpatialReference = EsriTools.Wgs84Spatialreference };

                        fromPoint.Project(line.Line.SpatialReference);
                        toPoint.Project(line.Line.SpatialReference);

                        polylines.Add(EsriTools.Create3DPolylineFromPoints(fromPoint, toPoint), segmentFromPoint.Visible);

                        segmentFromPoint = surfacePoints[i];
                    }
                }
            }

            return polylines;
        }

        internal static IPoint GetObserverPoint(double observerHeight, ProfileSurfacePoint firstPoint)
        {
            IPoint geoPoint = new Point() { X = firstPoint.X, Y = firstPoint.Y, Z = firstPoint.Z, SpatialReference = EsriTools.Wgs84Spatialreference };
            return EsriTools.GetObserverPoint(geoPoint, observerHeight, ArcMap.Document.FocusMap.SpatialReference);
        }
    }
}
