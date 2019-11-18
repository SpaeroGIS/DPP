using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.Facade;
using MilSpace.Visualization3D.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MilSpace.Visualization3D
{
    internal class Feature3DManager
    {
        internal Feature3DManager() { }

        internal static ArcSceneArguments Get3DFeatures(string demLayer, List<ProfileSession> profileSessions)
        {
            var arcSceneArguments = new ArcSceneArguments();

            var polylines = new Dictionary<IPolyline, bool>();
            var visibilityPolygons = new Dictionary<IPolygon, bool>();
            var observerPoints = new List<IPoint>();

            var layers = ArcMap.Document.ActiveView.FocusMap.Layers;

            var layer = layers.Next();

            while(layer.Name != demLayer)
            {
                layer = layers.Next();
            }

            var rasterLayer = layer as IRasterLayer;
            arcSceneArguments.DemLayer = rasterLayer.FilePath;

            try
            {
                foreach(var profilesSet in profileSessions)
                {
                    if(profilesSet == null || profilesSet.ProfileLines == null)
                    {
                        continue;
                    }

                    var segments = new List<ProfileSurface>();

                    if(profilesSet.DefinitionType == ProfileSettingsTypeEnum.Primitives)
                    {
                        segments = DataPreparingHelper.GetPrimitiveSegments(profilesSet);
                    }
                    else
                    {
                        segments = new List<ProfileSurface> { profilesSet.ProfileSurfaces[0] };
                    }

                    foreach(var segment in segments)
                    {
                        Dictionary<ProfileSurface, bool> segmentsSurfaces;

                        if(profilesSet.DefinitionType == ProfileSettingsTypeEnum.Primitives)
                        {
                            segmentsSurfaces = DataPreparingHelper.GetLinesSegmentsForPrimitive(segment);
                        }
                        else
                        {
                            segmentsSurfaces = DataPreparingHelper.GetLinesSegments(profilesSet);
                        }

                        var segmentsGeoPoints = DataPreparingHelper.GetSegmentsGeoPoints(segmentsSurfaces);
                        var setPolylines = DataPreparingHelper.GetPolylinesSegments(segmentsGeoPoints);

                        foreach(var polyline in setPolylines)
                        {
                            polylines.Add(polyline.Key, polyline.Value);
                        }

                        observerPoints.Add(DataPreparingHelper.GetObserverPoint(profilesSet.ObserverHeight, segment.ProfileSurfacePoints[0]));

                        var setPolygons = (DataPreparingHelper.GetVisibilityPolygons(observerPoints.Last(), segmentsGeoPoints));
                        foreach(var polygon in setPolygons)
                        {
                            visibilityPolygons.Add(polygon.Key, polygon.Value);
                        }
                    }
                }

                arcSceneArguments.Line3DLayer = GdbAccess.Instance.AddProfileLinesTo3D(polylines);
                arcSceneArguments.Point3DLayer = GdbAccess.Instance.AddProfilePointsTo3D(observerPoints);
                arcSceneArguments.Polygon3DLayer = GdbAccess.Instance.AddPolygonTo3D(visibilityPolygons);

                return arcSceneArguments;
            }
            catch(Exception ex)
            {
                //TODO: Log erros
                return null;
            }
        }
    }
}
