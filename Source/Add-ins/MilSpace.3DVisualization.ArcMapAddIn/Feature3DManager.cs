using ESRI.ArcGIS.Geometry;
using MilSpace.DataAccess;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.Facade;
using MilSpace.Visualization3D.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Visualization3D
{
    internal class Feature3DManager
    {
        internal Feature3DManager() { }

        internal static ArcSceneArguments Get3DFeatures(string DemLayer, List<ProfileSession> profileSessions)
        {
            var arcSceneArguments = new ArcSceneArguments();

            var polylines = new Dictionary<IPolyline, bool>();
            var visibilityPolygons = new Dictionary<IPolygon, bool>();
            var observerPoints = new List<IPoint>();

            arcSceneArguments.DemLayer = DemLayer;

            try
            {
                foreach(var profilesSet in profileSessions)
                {
                    var setPolylines = DataPreparingHelper.GetPolylinesSegments(profilesSet);
                    foreach(var polyline in setPolylines)
                    {
                        polylines.Add(polyline.Key, polyline.Value);
                    }

                    observerPoints.Add(DataPreparingHelper.GetObserverPoint(profilesSet.ObserverHeight, profilesSet.ProfileSurfaces[0].ProfileSurfacePoints[0]));

                    var setPolygons = (DataPreparingHelper.GetVisibilityPolygons(observerPoints.Last(), polylines));
                    foreach(var polygon in setPolygons)
                    {
                        visibilityPolygons.Add(polygon.Key, polygon.Value);
                    }
                }

                arcSceneArguments.Line3DLayer = GdbAccess.Instance.AddProfileLinesTo3D(polylines);
                //  arcSceneArguments.Point3DLayer = GdbAccess.Instance.AddProfilePointsTo3D(observerPoints);
                //  arcSceneArguments.Polygon3DLayer = GdbAccess.Instance.AddPolygonTo3D(visibilityPolygons);
                arcSceneArguments.SpatialReference = ArcMapInstance.Document.FocusMap.SpatialReference;
              
                return arcSceneArguments;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}
