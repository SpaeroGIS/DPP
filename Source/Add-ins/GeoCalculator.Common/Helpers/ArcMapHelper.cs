using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
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
        public static KeyValuePair<string, IPoint> AddGraphicToMap(
            IGeometry geom,
            IColor color,
            bool IsTempGraphic = false,
            esriSimpleMarkerStyle markerStyle = esriSimpleMarkerStyle.esriSMSCircle,
            int size = 5)
        {
            string slog = "";
                
            var emptyResult = new KeyValuePair<string, IPoint>();
            var mxdoc = ArcMap.Application.Document as IMxDocument;

            if ((geom == null) 
                || (geom.GeometryType != esriGeometryType.esriGeometryPoint)
                || (geom.SpatialReference == null)
                //|| (ArcMap.Document == null) 
                //|| (ArcMap.Document.FocusMap == null)
                || (mxdoc == null)
                )
                return emptyResult;

            //geom.Project(ArcMap.Document.FocusMap.SpatialReference);

            slog = slog + "1:" + (geom as IPoint).X.ToString();

            var simpleMarkerSymbol = (ISimpleMarkerSymbol)new SimpleMarkerSymbol();
            simpleMarkerSymbol.Color = color;
            simpleMarkerSymbol.Outline = false;
            simpleMarkerSymbol.OutlineColor = color;
            simpleMarkerSymbol.Size = size;
            simpleMarkerSymbol.Style = markerStyle;

            var markerElement = (IMarkerElement)new MarkerElement();
            markerElement.Symbol = simpleMarkerSymbol;

            IElement element = null;
            element = (IElement)markerElement;
            if (element == null)
                return emptyResult;
            element.Geometry = geom;
            element.Geometry.SpatialReference = mxdoc.FocusMap.SpatialReference;

            slog = slog + " 2:" + (element.Geometry as IPoint).X.ToString();

            var av = (IActiveView)mxdoc.FocusMap;
            var am = (IMap)mxdoc.FocusMap; //nikol
            var gc = (IGraphicsContainer)am;

            // store guid
            var eprop = (IElementProperties)element;
            eprop.Name = Guid.NewGuid().ToString();

            slog = slog + " 3:" + (element.Geometry as IPoint).X.ToString();

            gc.AddElement(element, 0);

            slog = slog + " 4:" + (element.Geometry as IPoint).X.ToString();

            av.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);

            slog = slog + " 5:" + (element.Geometry as IPoint).X.ToString();

            eprop.Name = eprop.Name + " -> " + slog;

            return new KeyValuePair<string, IPoint>(eprop.Name, element.Geometry as IPoint);
        }

        public static void RemoveGraphicsFromMap(string[] pointIds)
        {
            var activeView = (ArcMap.Application.Document as IMxDocument)?.FocusMap as IActiveView;

            var graphicsContainer = activeView?.GraphicsContainer;
            if (graphicsContainer == null)
                return;

            graphicsContainer.Reset();
            var element = graphicsContainer.Next();
            
            while (element != null)
            {
                if (pointIds.Any(pointId => pointId.Equals((element as IElementProperties)?.Name)))
                {
                    graphicsContainer.DeleteElement(element);
                }
                element = graphicsContainer.Next();
            }
            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

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
    }
}
