using ESRI.ArcGIS.Geometry;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Profile
{
    public class Test3D
    {
        static Test3D instance = null;
        MilSpaceProfileCalsController calsController;
        ProfileSession profileSession = new ProfileSession();

        internal delegate MilSpaceProfileCalsController GetControllerDelegate();
        internal event GetControllerDelegate GetController;

        private Test3D()
        { 

        }

        public static Test3D Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Test3D();
                }
                return instance;
            }
        }

        public void SetCurrentProfile()
        {
            calsController = GetController.Invoke();
            profileSession = calsController.GetSelectedSession();
        }

        public void SetSelectedProfileSetTo3D()
        {
            try
            {
                var polylines = new List<IPolyline>();

                foreach(var line in profileSession.ProfileLines)
                {
                    var surfacePoints = profileSession.ProfileSurfaces.First(profileSurface => profileSurface.LineId == line.Id).ProfileSurfacePoints;
                    var fromPoint = new Point() { X = surfacePoints.First().X, Y = surfacePoints.First().Y, Z = surfacePoints.First().Z, SpatialReference = EsriTools.Wgs84Spatialreference };
                    var toPoint = new Point() { X = surfacePoints.Last().X, Y = surfacePoints.Last().Y, Z = surfacePoints.Last().Z, SpatialReference = EsriTools.Wgs84Spatialreference };

                    fromPoint.Project(line.Line.SpatialReference);
                    toPoint.Project(line.Line.SpatialReference);

                    polylines.Add(EsriTools.Create3DPolylineFromPoints(fromPoint, toPoint));
                }

                GdbAccess.Instance.AddProfileLinesTo3D(polylines);
            }
            catch(Exception ex) { }
        }

        public void SetVisibilityPolygon()
        {
            try
            {
               
            }
            catch(Exception ex) { }
        }

        public void SetObserverPointsTo3D()
        {
            try
            {
                var point = profileSession.ProfileSurfaces[0].ProfileSurfacePoints[0];
                IPoint geoPoint = new Point() { X = point.X, Y = point.Y, Z = point.Z, SpatialReference = profileSession.ProfileLines[0].SpatialReference };
                GdbAccess.Instance.AddProfilePointsTo3D(new List<IPoint>() { EsriTools.GetObserverPoint( geoPoint, profileSession.ObserverHeight, ArcMap.Document.FocusMap.SpatialReference) });
            }
            catch(Exception ex) { }
        }
    }
}
