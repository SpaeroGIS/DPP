using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
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
        public static KeyValuePair<string, IPoint> AddGraphicToMap(
            IGeometry geom,
            IColor color,
            bool IsTempGraphic = false,
            esriSimpleMarkerStyle markerStyle = esriSimpleMarkerStyle.esriSMSCircle,
            int size = 5)
        {
            var emptyResult = new KeyValuePair<string, IPoint>();

            if ((geom == null) || (ArcMap.Document == null) || (ArcMap.Document.FocusMap == null)
                || (ArcMap.Document.FocusMap.SpatialReference == null))
                return emptyResult;

            IElement element = null;

            geom.Project(ArcMap.Document.FocusMap.SpatialReference);

            if (geom.GeometryType != esriGeometryType.esriGeometryPoint) return emptyResult;


            var simpleMarkerSymbol = (ISimpleMarkerSymbol)new SimpleMarkerSymbol();
            simpleMarkerSymbol.Color = color;
            simpleMarkerSymbol.Outline = false;
            simpleMarkerSymbol.OutlineColor = color;
            simpleMarkerSymbol.Size = size;
            simpleMarkerSymbol.Style = markerStyle;

            var markerElement = (IMarkerElement)new MarkerElement();
            markerElement.Symbol = simpleMarkerSymbol;
            element = (IElement)markerElement;


            if (element == null)
                return emptyResult;

            element.Geometry = geom;

            var mxdoc = ArcMap.Application.Document as IMxDocument;
            if (mxdoc == null)
                return emptyResult;

            var av = (IActiveView)mxdoc.FocusMap;
            var gc = (IGraphicsContainer)av;

            // store guid
            var eprop = (IElementProperties)element;
            eprop.Name = Guid.NewGuid().ToString();

            gc.AddElement(element, 0);

            av.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);

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

        public static void FlashGeometry(ESRI.ArcGIS.Geometry.IGeometry geometry,
            System.Int32 delay)
        {
            var mxdoc = ArcMap.Application.Document as IMxDocument;
            if (mxdoc == null)
                return;

            var av = (IActiveView)mxdoc.FocusMap;
            var display = av.ScreenDisplay;
            var envelope = av.Extent.Envelope;

            IRgbColor color = new RgbColorClass();
            color.Green = 255;
            color.Red = 0;
            color.Blue = 0;

            if ((geometry == null) || (color == null) || (display == null) || (envelope == null) || (delay < 0))
            {
                return;
            }

            display.StartDrawing(display.hDC, (System.Int16)ESRI.ArcGIS.Display.esriScreenCache.esriNoScreenCache); // Explicit Cast

            if (geometry.GeometryType != ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint) return;

            //Set the flash geometry's symbol.
            ESRI.ArcGIS.Display.ISimpleMarkerSymbol simpleMarkerSymbol = new ESRI.ArcGIS.Display.SimpleMarkerSymbolClass();
            simpleMarkerSymbol.Style = ESRI.ArcGIS.Display.esriSimpleMarkerStyle.esriSMSCircle;
            simpleMarkerSymbol.Size = 12;
            simpleMarkerSymbol.Color = color;
            ESRI.ArcGIS.Display.ISymbol markerSymbol = (ESRI.ArcGIS.Display.ISymbol)simpleMarkerSymbol;
            markerSymbol.ROP2 = ESRI.ArcGIS.Display.esriRasterOpCode.esriROPNotXOrPen;

            ESRI.ArcGIS.Display.ISimpleLineSymbol simpleLineSymbol = new ESRI.ArcGIS.Display.SimpleLineSymbolClass();
            simpleLineSymbol.Width = 1;
            simpleLineSymbol.Color = color;
            ESRI.ArcGIS.Display.ISymbol lineSymbol = (ESRI.ArcGIS.Display.ISymbol)simpleLineSymbol;
            lineSymbol.ROP2 = ESRI.ArcGIS.Display.esriRasterOpCode.esriROPNotXOrPen;

            DrawCrossHair(geometry, display, envelope, markerSymbol, lineSymbol);

            //Flash the input point geometry.
            display.SetSymbol(markerSymbol);
            display.DrawPoint(geometry);
            System.Threading.Thread.Sleep(delay);
            display.DrawPoint(geometry);
            display.FinishDrawing();
        }

        private static void DrawCrossHair(
            ESRI.ArcGIS.Geometry.IGeometry geometry,
            ESRI.ArcGIS.Display.IDisplay display,
            IEnvelope extent,
            ISymbol markerSymbol,
            ISymbol lineSymbol)
        {
            try
            {
                var point = geometry as IPoint;

                if ((point == null) || (display == null) || (extent == null) || (markerSymbol == null) ||
                    (lineSymbol == null) || (ArcMap.Application == null))
                    return;

                var numSegments = 10;

                var latitudeMid = point.Y;//envelope.YMin + ((envelope.YMax - envelope.YMin) / 2);
                var longitudeMid = point.X;
                var leftLongSegment = (point.X - extent.XMin) / numSegments;
                var rightLongSegment = (extent.XMax - point.X) / numSegments;
                var topLatSegment = (extent.YMax - point.Y) / numSegments;
                var bottomLatSegment = (point.Y - extent.YMin) / numSegments;
                var fromLeftLong = extent.XMin;
                var fromRightLong = extent.XMax;
                var fromTopLat = extent.YMax;
                var fromBottomLat = extent.YMin;
                var av = (ArcMap.Application.Document as IMxDocument).ActiveView;
                if (av == null)
                    return;

                var leftPolyline = new PolylineClass();
                var rightPolyline = new PolylineClass();
                var topPolyline = new PolylineClass();
                var bottomPolyline = new PolylineClass();

                leftPolyline.SpatialReference = geometry.SpatialReference;
                rightPolyline.SpatialReference = geometry.SpatialReference;
                topPolyline.SpatialReference = geometry.SpatialReference;
                bottomPolyline.SpatialReference = geometry.SpatialReference;

                var leftPC = (IPointCollection)leftPolyline;
                var rightPC = (IPointCollection)rightPolyline;
                var topPC = (IPointCollection)topPolyline;
                var bottomPC = (IPointCollection)bottomPolyline;

                leftPC.AddPoint(new PointClass() { X = fromLeftLong, Y = latitudeMid });
                rightPC.AddPoint(new PointClass() { X = fromRightLong, Y = latitudeMid });
                topPC.AddPoint(new PointClass() { X = longitudeMid, Y = fromTopLat });
                bottomPC.AddPoint(new PointClass() { X = longitudeMid, Y = fromBottomLat });

                for (int x = 1; x <= numSegments; x++)
                {
                    //Flash the input polygon geometry.
                    display.SetSymbol(markerSymbol);
                    display.SetSymbol(lineSymbol);

                    leftPC.AddPoint(new PointClass() { X = fromLeftLong + leftLongSegment * x, Y = latitudeMid });
                    rightPC.AddPoint(new PointClass() { X = fromRightLong - rightLongSegment * x, Y = latitudeMid });
                    topPC.AddPoint(new PointClass() { X = longitudeMid, Y = fromTopLat - topLatSegment * x });
                    bottomPC.AddPoint(new PointClass() { X = longitudeMid, Y = fromBottomLat + bottomLatSegment * x });

                    // draw
                    display.DrawPolyline(leftPolyline);
                    display.DrawPolyline(rightPolyline);
                    display.DrawPolyline(topPolyline);
                    display.DrawPolyline(bottomPolyline);

                    System.Threading.Thread.Sleep(15);
                    display.FinishDrawing();
                    av.PartialRefresh(esriViewDrawPhase.esriViewForeground, null, null);
                    System.Windows.Forms.Application.DoEvents();
                    display.StartDrawing(display.hDC, (System.Int16)ESRI.ArcGIS.Display.esriScreenCache.esriNoScreenCache); // Explicit Cast
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}
