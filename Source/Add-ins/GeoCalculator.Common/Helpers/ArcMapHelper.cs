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
                EsriTools.ZoomToGeometry(activeView, polyline);
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
            var activeView = (ArcMap.Application.Document as IMxDocument)?.ActiveView;

            if (activeView == null)
            { return new List<string>();  }

            return new MapLayersManager(activeView).GetFeatureLayersNames().ToList();
        }

        public static ILayer GetLayer(string layerName)
        {
            var activeView = (ArcMap.Application.Document as IMxDocument)?.ActiveView;

            if (activeView == null)
            { return null; }

            return new MapLayersManager(activeView).GetLayer(layerName);
        }

        public static double GetMetresInMapUnits(double metres)
        {
            var focusMap = (ArcMap.Application.Document as IMxDocument)?.FocusMap;
            var spatialReference = focusMap.SpatialReference;
            
            return EsriTools.GetMetresInMapUnits(metres, spatialReference);
        }
    }
}
