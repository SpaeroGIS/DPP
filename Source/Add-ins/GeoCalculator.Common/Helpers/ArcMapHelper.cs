using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;
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
            int number,
            bool showNums,
            string textName,
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

            if(showNums)
            {
                var point = geom as IPoint;
                DrawText(point, number, eprop.Name, textName);
            }

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

        public static void RemoveAllGeometryFromMap(string name)
        {
            var activeView = (ArcMap.Application.Document as IMxDocument)?.FocusMap as IActiveView;

            var graphicsContainer = activeView?.GraphicsContainer;
            if(graphicsContainer == null)
                return;

            graphicsContainer.Reset();
            var element = graphicsContainer.Next();

            while(element != null)
            {
                if((element as IElementProperties).Name.StartsWith(name))
                {
                    graphicsContainer.DeleteElement(element);
                }
                element = graphicsContainer.Next();
            }
            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        public static void RemovePoint(string name)
        {
            var activeView = (ArcMap.Application.Document as IMxDocument)?.FocusMap as IActiveView;

            var graphicsContainer = activeView?.GraphicsContainer;
            if(graphicsContainer == null)
                return;

            graphicsContainer.Reset();
            var element = graphicsContainer.Next();

            while(element != null)
            {
                if((element as IElementProperties).Name.EndsWith(name))
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

        public static void AddLineToMap(Dictionary<string, IPoint> points, string name)
        {
            var activeView = (ArcMap.Application.Document as IMxDocument)?.FocusMap as IActiveView;
            var prevPoint = points.First();

            foreach(var point in points)
            {
                if(point.Key == points.First().Key)
                {
                    continue;
                }

                AddLineSegmentToMap(prevPoint.Value, point.Value, name, prevPoint.Key);
                prevPoint = point;
            }

            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        public static void AddLineSegmentToMap(IPoint pointFrom, IPoint pointTo, string name, string fromPointGuid)
        {
            var color = (IColor)new RgbColorClass() { Green = 255 };
            var polyline = EsriTools.CreatePolylineFromPoints(pointFrom, pointTo);
            var lineElement = new LineElementClass();

            ICartographicLineSymbol cartographicLineSymbol = new CartographicLineSymbolClass();
            cartographicLineSymbol.Color = color;
            cartographicLineSymbol.Width = 3;

            lineElement.Symbol = cartographicLineSymbol;

            IElement elem = (IElement)lineElement;
            elem.Geometry = polyline;

            var segmentName = name + "_" + fromPointGuid; 
            var eprop = (IElementProperties)elem;
            eprop.Name = segmentName;

            var mxdoc = ArcMap.Application.Document as IMxDocument;
            var map = (IMap)mxdoc.FocusMap;
            var gc = (IGraphicsContainer)map;
            
            gc.AddElement(elem, 0);
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

        public static void DrawText(IPoint point, int number, string pointGuid, string textName)
        {
            var activeView = (ArcMap.Application.Document as IMxDocument)?.FocusMap as IActiveView;
            var units = GetLengthInMapUnits(activeView, 5);
            var textPoint = new Point() { X = point.X + units, Y = point.Y + units, SpatialReference = point.SpatialReference };

            var textColor = (IColor)new RgbColorClass() { Red = 255 };

            ITextElement textElement = new TextElementClass();
            textElement.Text = number.ToString();
            var textSymbol = new TextSymbol();
            textSymbol.Color = textColor;
            textElement.Symbol = textSymbol;
            IElement textElementEl = (IElement)textElement;
            textElementEl.Geometry = textPoint;

            var textPropr = (IElementProperties)textElementEl;
            textPropr.Name = textName + pointGuid;

            var map = (IMap)activeView.FocusMap;
            var gc = (IGraphicsContainer)map;

            gc.AddElement(textElementEl, 0);
            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        private static double GetLengthInMapUnits(ESRI.ArcGIS.Carto.IActiveView activeView, double mm)
        {
            if(activeView == null)
            {
                return -1;
            }
            
            IScreenDisplay screenDisplay = activeView.ScreenDisplay;
            IDisplayTransformation displayTransformation = screenDisplay.DisplayTransformation;
            var dpi = displayTransformation.Resolution;
            var inches = mm / 25.4;

            tagRECT deviceRect = displayTransformation.get_DeviceFrame();
            int pixelExtent = (deviceRect.right - deviceRect.left);

            var pixels = dpi * inches;
            IDistanceConverter distanceConverter = new DistanceConverter();

            IEnvelope envelope = displayTransformation.VisibleBounds;
            double realWorldDisplayExtent = envelope.Width;

            if(pixelExtent == 0)
            {
                return -1;
            }

            var sizeOfOnePixel = (realWorldDisplayExtent / pixelExtent);

            return (pixels * sizeOfOnePixel);
        }
    }
}
