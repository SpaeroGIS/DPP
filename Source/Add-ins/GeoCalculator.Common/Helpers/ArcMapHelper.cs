using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.GeoCalculator
{
    public static class ArcMapHelper
    {
        /// <summary>
        /// Adds a graphic element to the map graphics container
        /// Returns GUID
        /// </summary>
        /// <param name="geom">IGeometry</param>
        /// 
        
      
        public static void FlashGeometry(
            ESRI.ArcGIS.Geometry.IGeometry geometry,
            System.Int32 delay)
        {
            if(!EsriTools.IsPointOnExtent(ArcMap.Document.ActivatedView.Extent, geometry as IPoint))
            {
                EsriTools.PanToGeometry(ArcMap.Document.ActiveView, geometry, true);
            }
            EsriTools.FlashGeometry(geometry, delay, ArcMap.Application);
        }

      
        public static void FlashLine(IEnumerable<IPoint> points)
        {
            var activeView = (ArcMap.Application.Document as IMxDocument)?.FocusMap as IActiveView;
            IGeometryBridge2 pGeoBrg = new GeometryEnvironment() as IGeometryBridge2;

            IPointCollection4 pPointColl = new PolylineClass();
            var aWKSPointBuffer = new IPoint[points.Count()];
            
            var i = 0;
            foreach(var point in points)
            {
                aWKSPointBuffer[i] = point;
                i++;
            }
            pGeoBrg.SetPoints(pPointColl, ref aWKSPointBuffer);

            var polyline = pPointColl as IPolyline;

            IGeometryCollection theGeomColl = new GeometryBagClass();
            theGeomColl.AddGeometry(polyline);

            ITopologicalOperator theTopoOp = new PolylineClass();
            theTopoOp.ConstructUnion((IEnumGeometry)theGeomColl);

            IGeometry flashGeometry = theTopoOp as IGeometry;

            if(flashGeometry != null)
            {
                EsriTools.PanToGeometry(activeView, polyline);
                EsriTools.FlashGeometry(activeView.ScreenDisplay, new IGeometry[] { polyline });
            }
        }


        public static void AddFeatureClassToMap(IFeatureClass featureClass)
        {
            var map = (ArcMap.Application.Document as IMxDocument)?.FocusMap;

            var featureLayer = new FeatureLayer();
            featureLayer.FeatureClass = featureClass;
            featureLayer.Name = featureClass.AliasName;

            map.AddLayer(featureLayer);
        }

        public static List<string> GetFeatureLayers()
        {
            var result = new List<string>();
            var map = (ArcMap.Application.Document as IMxDocument)?.FocusMap;
            var layers = map.Layers;

            var layer = layers.Next();

            while(layer != null)
            {
                if(layer is IFeatureLayer fLayer && !layer.Name.StartsWith($"GCP_{ DateTime.Now.ToString("yyyyMMdd")}"))
                {
                    var featureLayer = fLayer;
                    var featureClass = featureLayer.FeatureClass;

                    if(featureClass != null &&
                        ((featureClass.ShapeType == esriGeometryType.esriGeometryLine) ||
                        (featureClass.ShapeType == esriGeometryType.esriGeometryPolyline) ||
                        (featureClass.ShapeType == esriGeometryType.esriGeometryPoint) ||
                        (featureClass.ShapeType == esriGeometryType.esriGeometryPolygon)))
                    {
                        result.Add(fLayer.Name);
                    }
                }

                layer = layers.Next();
            }

            return result;
        }

        public static ILayer GetLayer(string layerName)
        {
            var map = (ArcMap.Application.Document as IMxDocument)?.FocusMap;
            var layers = map.Layers;
            var layer = layers.Next();

            while(layer != null && layer.Name != layerName)
            {
                layer = layers.Next() as ILayer;
            }

            return layer;
        }

        public static double GetMetresInMapUnits(double metres)
        {
            var focusMap = (ArcMap.Application.Document as IMxDocument)?.FocusMap;
            var spatialReference = focusMap.SpatialReference;

            IDistanceConverter distanceConverter = new DistanceConverter();

            return distanceConverter.GetValue($"{metres}m", spatialReference);
        }

    }
}
