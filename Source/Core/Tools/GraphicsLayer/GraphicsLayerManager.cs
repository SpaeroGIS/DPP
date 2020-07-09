using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core;
using MilSpace.Core.Tools;
using MilSpace.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MilSpace.Tools.GraphicsLayer
{
    public class GraphicsLayerManager
    {
        IGraphicsContainer graphics;
        IActiveView activeView;

        static Logger logger = Logger.GetLoggerEx("GraphicsLayerManager");

        private List<GraphicElement> milSpaceCalculatingGraphics = new List<GraphicElement>();
        private List<GraphicElement> milSpaceSessionGraphics = new List<GraphicElement>();
        private List<GraphicElement> milSpaceGeoCalcGraphics = new List<GraphicElement>();
        private List<GraphicElement> milSpaceVisibilityGraphics = new List<GraphicElement>();

        private static Dictionary<MilSpaceGraphicsTypeEnum, Func<IRgbColor>> grapchucsTypeColors = new Dictionary<MilSpaceGraphicsTypeEnum, Func<IRgbColor>>
        {
            { MilSpaceGraphicsTypeEnum.Calculating, () =>  new RgbColor(){ Red = 255,Green = 0, Blue = 0} },
            { MilSpaceGraphicsTypeEnum.Session, () =>  new RgbColor(){ Red = 0,Green = 255, Blue = 0} },
            { MilSpaceGraphicsTypeEnum.GeoCalculator, () =>  new RgbColor(){ Red = 0,Green = 255, Blue = 0} },
            { MilSpaceGraphicsTypeEnum.Visibility, () =>  new RgbColor(){ Red = 96,Green = 154, Blue = 185} }
        };

        private static Dictionary<IActiveView, GraphicsLayerManager> activeViews = new Dictionary<IActiveView, GraphicsLayerManager>();

        private Dictionary<MilSpaceGraphicsTypeEnum, List<GraphicElement>> allGraphics = new Dictionary<MilSpaceGraphicsTypeEnum, List<GraphicElement>>();

        private enum LineType { Point, Line, Arrow, Cross, DefaultLine };
        private string geoCalcPointsSuffix = "_geoCalcPoint";

        internal GraphicsLayerManager(IActiveView activeView)
        {
            this.activeView = activeView;
            graphics = activeView.GraphicsContainer;
            allGraphics.Add(MilSpaceGraphicsTypeEnum.Calculating, milSpaceCalculatingGraphics);
            allGraphics.Add(MilSpaceGraphicsTypeEnum.Session, milSpaceSessionGraphics);
            allGraphics.Add(MilSpaceGraphicsTypeEnum.GeoCalculator, milSpaceGeoCalcGraphics);
            allGraphics.Add(MilSpaceGraphicsTypeEnum.Visibility, milSpaceVisibilityGraphics);

        }

        public static GraphicsLayerManager GetGraphicsLayerManager(IActiveView activeView)
        {
            if (activeViews.ContainsKey(activeView))
            { return activeViews[activeView]; }

            GraphicsLayerManager graphicsLayerManager = new GraphicsLayerManager(activeView);
            activeViews.Add(activeView, graphicsLayerManager);
            return graphicsLayerManager;
        }

        private void UpdateGraphic(IEnumerable<IPolyline> profileLines, int profileId, int profileTypeId, MilSpaceGraphicsTypeEnum graphicsType)
        {
            logger.InfoEx($"Update Graphic for profile {profileId}");

            EmptyProfileGraphics(graphicsType);

            int elementId = profileTypeId;
            foreach (var line in profileLines)
            {
                var ge = new GraphicElement() { Source = line, ElementId = ++elementId, ProfileId = profileId };
                AddPolyline(ge, graphicsType);
            }

            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);

        }

        private void UpdateGraphicLine(GroupedLines groupedLines, int profileId,
                                       MilSpaceGraphicsTypeEnum graphicsType, bool selectionRemove = false)
        {
            RemoveLineFromSessionGraphicsByLineId(profileId, groupedLines.LineId, graphicsType);

            logger.InfoEx($"UpdateGraphicLine {profileId}");

            int elementId = 0;
            int lineNumber = 0;
            int width;

            if (selectionRemove)
            {
                width = 2;
            }
            else
            {
                width = (groupedLines.IsSelected) ? 4 : 2;
            }

            foreach (var line in groupedLines.Polylines)
            {
                var ge = new GraphicElement()
                {
                    Source = line,
                    ElementId = ++elementId,
                    ProfileId = profileId,
                    LineId = groupedLines.LineId
                };

                var color = (groupedLines.Lines[lineNumber].Visible) ? groupedLines.VisibleColor
                                                                     : groupedLines.InvisibleColor;

                var pointFrom = groupedLines.Lines[lineNumber].PointFrom;
                var isVertex = groupedLines.IsPrimitive && groupedLines.Vertices.Exists(point => point.X == pointFrom.X && point.Y == pointFrom.Y);

                LineType lineType;

                if (groupedLines.Lines.Count() == 1) { lineType = LineType.DefaultLine; }
                else if (groupedLines.Lines.First() == groupedLines.Lines[lineNumber]) { lineType = LineType.Point; }
                else if (groupedLines.Lines.Last() == groupedLines.Lines[lineNumber])
                { lineType = (!isVertex) ? LineType.Arrow : LineType.DefaultLine; }
                else { lineType = (!isVertex) ? LineType.Line : LineType.Point; }

                AddPolyline(ge, graphicsType, color, lineType, false, false, width);

                lineNumber++;
            }

            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        private void RemoveLineFromSessionGraphicsByLineId(int profileId, int LineId, MilSpaceGraphicsTypeEnum graphicsType)
        {
            var graphicLine = milSpaceSessionGraphics.Where(g => g.ProfileId == profileId && g.LineId == LineId).ToList();
            var curList = allGraphics[graphicsType];

            if (graphicLine.Count() > 0)
            {
                foreach (var graphic in graphicLine)
                {
                    DeleteGraphicsElement(graphic);
                    curList.Remove(graphic);
                }
            }

            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        private void RemoveLineFromSessionGraphicsByProfileId(int profileId, MilSpaceGraphicsTypeEnum graphicsType)
        {
            var graphicLine = milSpaceSessionGraphics.Where(g => g.ProfileId == profileId).ToList();
            var curList = allGraphics[graphicsType];

            if (graphicLine.Count > 0)
            {
                foreach (var graphic in graphicLine)
                {
                    DeleteGraphicsElement(graphic);
                    curList.Remove(graphic);
                }
            }

            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }


        public void RemoveLinesFromSessionGraphics(IEnumerable<IPolyline> profileLines, int profileId, int profileTypeId)
        {
            logger.InfoEx($"Remove Lines From Session Graphics {profileId}");
            int elementId = profileId;
            foreach (var line in profileLines)
            {
                var graphic = milSpaceSessionGraphics.FirstOrDefault(g => g.ProfileId == profileId && g.ElementId == ++elementId);
                if (graphic != null)
                {
                    graphics.DeleteElement(graphic.Element);
                    milSpaceSessionGraphics.Remove(graphic);
                }
            }

            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);

        }

        public void FlashLineOnWorkingGraphics(IEnumerable<IGeometry> flashingGeometry)
        {
            var curList = allGraphics[MilSpaceGraphicsTypeEnum.Session];
            IGeometry flashGeometry = null;

            IGeometryCollection theGeomColl = new GeometryBagClass();
            flashingGeometry.ToList().ForEach(pl => theGeomColl.AddGeometry(pl));

            ITopologicalOperator theTopoOp = new PolylineClass();
            theTopoOp.ConstructUnion((IEnumGeometry)theGeomColl);

            flashGeometry = theTopoOp as IGeometry;

            if (flashGeometry != null)
            {
                EsriTools.FlashGeometry(activeView.ScreenDisplay, flashingGeometry);
            }
        }

        public void ChangeSelectProfileOnGraph(GroupedLines oldSelectedLines, GroupedLines newSelectedLines, int profileId)
        {
            if (oldSelectedLines != null && oldSelectedLines.Polylines != null)
            {
                UpdateGraphicLine(oldSelectedLines, profileId, MilSpaceGraphicsTypeEnum.Session, true);
            }

            if (newSelectedLines != null && newSelectedLines.Polylines != null)
            {
                newSelectedLines.IsSelected = true;
                UpdateGraphicLine(newSelectedLines, profileId, MilSpaceGraphicsTypeEnum.Session, false);
            }
        }

        public void ShowLineOnWorkingGraphics(int profileId, GroupedLines groupedLines)
        {
            AddLinesToGraphics(groupedLines.Polylines, profileId, MilSpaceGraphicsTypeEnum.Session, groupedLines);
        }

        public void HideLineFromWorkingGraphics(int profileId, int lineId)
        {
            RemoveLineFromGraphic(profileId, lineId);
        }

        public void RemoveLinesFromWorkingGraphics(List<GroupedLines> allGroupedLines, int profileId)
        {
            RemoveLinesFromGraphics(allGroupedLines, profileId, MilSpaceGraphicsTypeEnum.Session);
        }

        public void RemoveGraphic(int profileId, List<int> linesIds)
        {
            foreach (var id in linesIds)
            {
                RemoveLineFromSessionGraphicsByLineId(profileId, id, MilSpaceGraphicsTypeEnum.Session);
            }
        }

        public void RemoveGraphic(int profileId)
        {
            RemoveLineFromSessionGraphicsByProfileId(profileId, MilSpaceGraphicsTypeEnum.Session);
        }

        public void RemoveLineFromGraphic(int profileId, int lineId)
        {
            RemoveLineFromSessionGraphicsByLineId(profileId, lineId, MilSpaceGraphicsTypeEnum.Session);
        }

        public void AddLinesToWorkingGraphics(IEnumerable<IPolyline> profileLines, int profileId,
                                                GroupedLines profileColorLines = null)
        {
            AddLinesToGraphics(profileLines, profileId, MilSpaceGraphicsTypeEnum.Session, profileColorLines);
        }

        public void AddLinesToCalcGraphics(IEnumerable<IPolyline> profileLines, int profileId)
        {
            AddLinesToGraphics(profileLines, profileId, MilSpaceGraphicsTypeEnum.Calculating);

        }

        public void UpdateWorkingingGraphics(IEnumerable<IPolyline> profileLines, int profileId, int profileTypeId)
        {
            UpdateGraphic(profileLines, profileId, profileTypeId, MilSpaceGraphicsTypeEnum.Session);
        }

        public void UpdateCalculatingGraphic(IEnumerable<IPolyline> profileLines, int profileId, int profileTypeId)
        {
            UpdateGraphic(profileLines, profileId, profileTypeId, MilSpaceGraphicsTypeEnum.Calculating);
        }

        public void UpdateGraphicLine(GroupedLines groupedLines, int profileId)
        {
            UpdateGraphicLine(groupedLines, profileId, MilSpaceGraphicsTypeEnum.Session);
        }

        public void EmptyProfileGraphics(MilSpaceGraphicsTypeEnum profileType)
        {
            var curList = allGraphics[profileType];
            curList.ForEach(e => DeleteGraphicsElement(e));
            curList.RemoveRange(0, curList.Count);
            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        public void AddCalculationPolyline(GraphicElement graphicElement, bool doRefresh = false)
        {
            AddPolyline(graphicElement, MilSpaceGraphicsTypeEnum.Calculating, null);
        }

        private void AddLinesToGraphics(IEnumerable<IPolyline> profileLines, int profileId,
                                            MilSpaceGraphicsTypeEnum graphicsType, GroupedLines profileColorLines = null)
        {
            var curList = allGraphics[graphicsType];
            int elementId = 0;
            var lineNumber = 0;
            int width = 2;

            if (profileColorLines != null)
            {
                RemoveLineFromSessionGraphicsByLineId(profileId, profileColorLines.LineId, graphicsType);
                width = (profileColorLines.IsSelected) ? 4 : 2;
            }

            foreach (var line in profileLines)
            {
                elementId++;

                if (profileColorLines != null)
                {
                    logger.InfoEx($"From Point - Graphics {line.FromPoint.X}: {line.FromPoint.Y}");
                    logger.InfoEx($"To Point - Graphics {line.ToPoint.X}: {line.ToPoint.Y}");
                    line.Project(activeView.FocusMap.SpatialReference);
                    logger.InfoEx($"Projected From Point - Graphics {line.FromPoint.X}: {line.FromPoint.Y}");
                    logger.InfoEx($"Projected To Point - Graphics {line.ToPoint.X}: {line.ToPoint.Y}");
                    var ge = new GraphicElement() { Source = line, ElementId = elementId, ProfileId = profileId, LineId = profileColorLines.LineId };
                    var color = (profileColorLines.Lines[lineNumber].Visible) ? profileColorLines.VisibleColor
                                                                              : profileColorLines.InvisibleColor;
                    var pointFrom = profileColorLines.Lines[lineNumber].PointFrom;
                    var isVertex = profileColorLines.IsPrimitive && profileColorLines.Vertices.Exists(point => point.X == pointFrom.X && point.Y == pointFrom.Y);

                    LineType lineType;

                    if (profileColorLines.Lines.Count() == 1) { lineType = LineType.DefaultLine; }
                    else if (profileColorLines.Lines.First() == profileColorLines.Lines[lineNumber]) { lineType = LineType.Point; }
                    else if (profileColorLines.Lines.Last() == profileColorLines.Lines[lineNumber])
                    { lineType = (!isVertex) ? LineType.Arrow : LineType.DefaultLine; }
                    else { lineType = (!isVertex) ? LineType.Line : LineType.Point; }


                    AddPolyline(ge, graphicsType, color, lineType, false, false, width);

                    lineNumber++;
                }
                else
                {
                    var graphic = curList.FirstOrDefault(g => g.ProfileId == profileId && g.ElementId == elementId);
                    if (graphic != null)
                    {
                        DeleteGraphicsElement(graphic);
                        curList.Remove(graphic);
                    }

                    var ge = new GraphicElement() { Source = line, ElementId = elementId, ProfileId = profileId, LineId = elementId };
                    AddPolyline(ge, graphicsType);
                }

            }
            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        public void RemoveLinesFromGraphics(IEnumerable<IPolyline> profileLines, int profileId,
                                                MilSpaceGraphicsTypeEnum graphicsType)
        {
            var curList = allGraphics[graphicsType];
            int elementId = 0;
            foreach (var line in profileLines)
            {
                elementId++;
                var graphic = curList.FirstOrDefault(g => g.ProfileId == profileId && g.ElementId == elementId);
                DeleteGraphicsElement(graphic);
                if (graphic != null)
                {
                    curList.Remove(graphic);
                }
            }

            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);

        }

        public void RemoveLinesFromGraphics(List<GroupedLines> allGroupedLines, int profileId,
                                                MilSpaceGraphicsTypeEnum graphicsType)
        {
            var curList = allGraphics[graphicsType];

            foreach (var groupedLine in allGroupedLines)
            {
                int elementId = 0;

                foreach (var line in groupedLine.Polylines)
                {
                    elementId++;

                    var graphic = curList.FirstOrDefault(g => g.ProfileId == profileId && g.ElementId == elementId
                                                                                       && g.LineId == groupedLine.LineId);

                    DeleteGraphicsElement(graphic);

                    if (graphic != null)
                    {
                        curList.Remove(graphic);
                    }
                }
            }
            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        /// <summary>
        /// Returns selected graphics
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IPolyline> GetSelectedGraphics()
        {
            var graphContainerSelection = (IGraphicsContainerSelect)activeView.FocusMap;

            if (graphContainerSelection.ElementSelectionCount == 0)
                return null;

            var selected = graphContainerSelection.SelectedElements;

            selected.Reset();
            var element = selected.Next();
            var res = new List<IPolyline>();
            while (element != null)
            {
                var geometry = element.Geometry;
                //To use others geometry types - convert to Polyline
                if (geometry.GeometryType == esriGeometryType.esriGeometryPolyline)
                {
                    res.Add(geometry as IPolyline);
                }
                element = selected.Next();
            }
            return res;
        }

        public IEnumerable<IGeometry> GetAllSelectedGraphics()
        {
            var graphContainerSelection = (IGraphicsContainerSelect)activeView.FocusMap;

            if (graphContainerSelection.ElementSelectionCount == 0)
                return null;

            var selected = graphContainerSelection.SelectedElements;

            selected.Reset();
            var element = selected.Next();
            var res = new List<IGeometry>();
            while (element != null)
            {
                var geometry = element.Geometry;
                res.Add(geometry);

                element = selected.Next();
            }
            return res;
        }

        private void AddPolyline(GraphicElement graphicElement, MilSpaceGraphicsTypeEnum graphicsType,
                                    IRgbColor color = null, LineType lineType = LineType.DefaultLine, bool doRefresh = false,
                                    bool persist = false, int width = 2)
        {

            logger.InfoEx($"AddPolyline {graphicElement.ProfileId} Element {graphicElement.ElementId}");

            IPolyline profileLine = graphicElement.Source as IPolyline;
            ILineElement lineElement = new LineElementClass();

            var curList = allGraphics[graphicsType];

            bool exists = curList.Any(ge => ge.ProfileId == graphicElement.ProfileId
                                           && ge.ElementId == graphicElement.ElementId
                                           && ge.LineId == graphicElement.LineId
                                           && ge.Name == graphicElement.Name);




            if (!persist && exists)
            {
                logger.InfoEx($"! Persists & Exists");
                return;
            }

            if (color == null)
            {
                color = grapchucsTypeColors[graphicsType]();
            }

            if (lineType == LineType.Line)
            {
                lineElement.Symbol = DefineProfileLineSymbol(graphicsType, color, width);
            }
            else
            {
                lineElement.Symbol = DefineProfileDecorationLineSymbol(graphicsType, color, width, lineType);
            }

            IElement elem = (IElement)lineElement;
            elem.Geometry = profileLine;

            var elementProp = lineElement as IElementProperties;
            elementProp.Name = graphicElement.Name;

            graphicElement.Element = elem;

            DeleteGraphicsElement(graphicElement);


            //    ////////////////
            //    //Create a new text element.  
            //ITextElement textElement = new TextElementClass();  
            ////Create a text symbol.  
            //ITextSymbol textSymbol = new TextSymbolClass();  
            //textSymbol.Size = 25;  

            ////Set the text element properties.  
            //textElement.Symbol = textSymbol;  
            //textElement.Text = DateTime.Now.ToShortDateString();  

            ////Query interface (QI) for IElement.  
            //IElement element = (IElement)textElement;  
            ////Create a point.  
            //IPoint point = new PointClass();  
            //point = profileLine.FromPoint;  
            ////Set the element's geometry.  
            //element.Geometry = point;  

            ////Add the element to the graphics container.  
            //activeView.GraphicsContainer.AddElement(element, 0);  
            //    ///


            logger.InfoEx($"Adding element to Graphic container..");
            graphics.AddElement(elem, 0);
            logger.InfoEx($"Element addied to Graphic container.");

            if (!exists)
            {
                logger.InfoEx($"Adding element to cache..");
                curList.Add(graphicElement);
                logger.InfoEx($"Element added to cache.");
            }

            if (exists && persist)
            {
                curList.Remove(graphicElement);

                logger.InfoEx($"Adding element to cache..");
                curList.Add(graphicElement);
                logger.InfoEx($"Element added to cache.");
            }

            if (doRefresh)
            {
                logger.InfoEx($"Refreshing view..");
                activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
                logger.InfoEx($"Refreshed");
            }
        }

        private static ILineSymbol DefineProfileDecorationLineSymbol(MilSpaceGraphicsTypeEnum graphicsType, IRgbColor color,
                                                                        int width = 2, LineType lineType = LineType.Arrow)
        {
            //TODO: Get symbol from ESRITools

            //Create cartographic line symbol  
            ICartographicLineSymbol cartographicLineSymbol = new CartographicLineSymbolClass
            {
                Color = color,
                Width = width
            };

            //Define line decoration  
            ILineDecoration lineDecoration = new LineDecorationClass();

            if (lineType == LineType.Arrow || lineType == LineType.DefaultLine)
            {
                //Define simple line decoration  
                ISimpleLineDecorationElement simpleLineDecorationElement = new SimpleLineDecorationElementClass();

                //Place the arrow at the end of the line(the "To" point in the geometry below)
                simpleLineDecorationElement.AddPosition(1);

                //Define an arrow marker 
                IArrowMarkerSymbol arrowMarkerSymbol = new ArrowMarkerSymbolClass
                {
                    Color = color,
                    Size = 5,
                    Length = 8,
                    Width = 5 + width,

                    //Add an offset to make sure the square end of the line is hidden  
                    XOffset = 0.8
                };

                simpleLineDecorationElement.MarkerSymbol = arrowMarkerSymbol;
                lineDecoration.AddElement(simpleLineDecorationElement);
            }

            if (lineType == LineType.Point || lineType == LineType.DefaultLine)
            {
                //Define simple line decoration  
                ISimpleLineDecorationElement simpleLineDecorationElement = new SimpleLineDecorationElementClass();

                simpleLineDecorationElement.AddPosition(0);

                SimpleMarkerSymbol circleMarkerSymbol = new SimpleMarkerSymbol()
                {
                    Color = color,
                    Size = 5,
                    Style = esriSimpleMarkerStyle.esriSMSCircle,
                };

                //Add an offset to make sure the square end of the line is hidden  
                circleMarkerSymbol.XOffset = 0.8;
                simpleLineDecorationElement.MarkerSymbol = circleMarkerSymbol;

                lineDecoration.AddElement(simpleLineDecorationElement);
            }

            if (lineType == LineType.Cross)
            {
                ISimpleLineDecorationElement simpleLineDecorationElement = new SimpleLineDecorationElementClass();

                simpleLineDecorationElement.AddPosition(1);

                SimpleMarkerSymbol crossMarkerSymbol = new SimpleMarkerSymbol()
                {
                    Color = color,
                    Size = 4 * width,
                    Style = esriSimpleMarkerStyle.esriSMSCross,
                };

                simpleLineDecorationElement.MarkerSymbol = crossMarkerSymbol;

                lineDecoration.AddElement(simpleLineDecorationElement);
            }

            //Set line properties  
            ILineProperties lineProperties = (ILineProperties)cartographicLineSymbol;
            lineProperties.LineDecoration = lineDecoration;

            return cartographicLineSymbol;
        }

        private static ILineSymbol DefineProfileLineSymbol(MilSpaceGraphicsTypeEnum graphicsType, IRgbColor color, int width = 2)
        {
            //Create cartographic line symbol  
            ICartographicLineSymbol cartographicLineSymbol = new CartographicLineSymbolClass
            {
                Color = color,
                Width = width
            };

            return cartographicLineSymbol;
        }

        public void ClearMapFromOldGraphics()
        {
            var actualGraphics = new List<GraphicElement>(milSpaceSessionGraphics);

            graphics.Reset();
            IElement ge = graphics.Next();
            while (ge != null)
            {
                var graphic = actualGraphics.FirstOrDefault(graph => graph.Element.Equals(ge));

                if (graphic != null)
                {
                    actualGraphics.Remove(graphic);
                }
                else
                {
                    graphics.DeleteElement(ge);
                }

                ge = graphics.Next();
            }

            graphics.Reset();
            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        public KeyValuePair<Guid, IPoint> AddGraphicToMap(
            IGeometry geom,
            IColor color,
            int number,
            bool showNums,
            string textName,
            string guid = null,
            esriSimpleMarkerStyle markerStyle = esriSimpleMarkerStyle.esriSMSCircle,
            int size = 5)
        {
            var emptyResult = new KeyValuePair<Guid, IPoint>();

            if ((geom == null)
                || (geom.GeometryType != esriGeometryType.esriGeometryPoint)
                || (geom.SpatialReference == null)
                )
                return emptyResult;

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
            element.Geometry.SpatialReference = activeView.FocusMap.SpatialReference;

            var eprop = (IElementProperties)element;
            var newGuid = Guid.NewGuid().ToString();
            var pointGuid = (String.IsNullOrEmpty(guid)) ? newGuid : guid;
            eprop.Name = pointGuid + geoCalcPointsSuffix;

            var ge = new GraphicElement() { Source = geom, Name = eprop.Name, Element = element };

            if (allGraphics[MilSpaceGraphicsTypeEnum.GeoCalculator].Exists(el => el.Name == eprop.Name))
            {
                DeleteGraphicsElement(ge, true, true);
            }

            graphics.AddElement(element, 0);
            allGraphics[MilSpaceGraphicsTypeEnum.GeoCalculator].Add(ge);

            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);

            if (showNums)
            {
                var point = geom as IPoint;
                DrawText(point, number.ToString(), $"{textName}{pointGuid}", MilSpaceGraphicsTypeEnum.GeoCalculator, color);
            }

            return new KeyValuePair<Guid, IPoint>(Guid.Parse(pointGuid), element.Geometry as IPoint);
        }

        public void RemoveGeoCalcGraphicsFromMap()
        {
            graphics.Reset();
            IElement ge = graphics.Next();

            while (ge != null)
            {
                var currentElProperties = ge as IElementProperties;

                if (currentElProperties.Name.EndsWith(geoCalcPointsSuffix))
                {
                    graphics.DeleteElement(ge);
                }

                ge = graphics.Next();
            }

            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        public void DrawText(IPoint point, string text, string textName, MilSpaceGraphicsTypeEnum graphicsType, IColor textColor = null, int size = 12, int distance = 5)
        {
            var units = GetLengthInMapUnits(activeView, distance);
            var textPoint = new Point() { X = point.X + units, Y = point.Y + units, SpatialReference = point.SpatialReference };

            if (textColor == null)
            {
                textColor = new RgbColorClass() { Red = 255 };
            }

            ITextElement textElement = new TextElementClass
            {
                Text = text
            };

            var textSymbol = new TextSymbol
            {
                Color = textColor,
                Size = size
            };

            textElement.Symbol = textSymbol;
            IElement textElementEl = (IElement)textElement;
            textElementEl.Geometry = textPoint;

            var textPropr = (IElementProperties)textElementEl;
            textPropr.Name = textName;

            if (graphicsType == MilSpaceGraphicsTypeEnum.GeoCalculator)
            {
                textPropr.Name += geoCalcPointsSuffix;
            }

            var ge = new GraphicElement() { Source = textPoint, Name = textPropr.Name, Element = textElementEl };

            if (allGraphics[graphicsType].Exists(el => el.Name == textPropr.Name))
            {
                DeleteGraphicsElement(ge, true, true);
            }

            allGraphics[graphicsType].Add(ge);

            graphics.AddElement(textElementEl, 0);

            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        public void AddLineToMap(Dictionary<Guid, IPoint> points, string name)
        {
            var prevPoint = points.First();

            foreach (var point in points)
            {
                if (point.Key == points.First().Key)
                {
                    continue;
                }

                AddLineSegmentToMap(prevPoint.Value, point.Value, name, prevPoint.Key.ToString());
                prevPoint = point;
            }

            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        public void AddLineSegmentToMap(IPoint pointFrom, IPoint pointTo, string name, string fromPointGuid)
        {
            var color = (IRgbColor)new RgbColorClass() { Green = 255 };
            var polyline = EsriTools.CreatePolylineFromPoints(pointFrom, pointTo);
            var segmentName = name + "_" + fromPointGuid + geoCalcPointsSuffix;
            var ge = new GraphicElement() { Source = polyline, Name = segmentName };
            AddPolyline(ge, MilSpaceGraphicsTypeEnum.GeoCalculator, color, LineType.Line, true, true);
        }

        public void AddObservPointsGraphicsToMap(IPolygon coverageArea, string areaName)
        {
            if ((coverageArea == null) || (coverageArea.SpatialReference == null))
            {
                return;
            }

            ISimpleFillSymbol simplePolygonSymbol = new SimpleFillSymbolClass();
            var color = (IRgbColor)new RgbColorClass() { Red = 255, Green = 253, Blue = 3 };
            simplePolygonSymbol.Color = color;
            simplePolygonSymbol.Style = esriSimpleFillStyle.esriSFSHollow;

            ILineSymbol polygonOutline = new SimpleLineSymbol
            {
                Color = color,
                Width = 2
            };

            simplePolygonSymbol.Outline = polygonOutline;

            IFillShapeElement markerElement = new PolygonElementClass
            {
                Symbol = simplePolygonSymbol
            };

            IElement element = null;
            element = (IElement)markerElement;

            if (element == null)
                return;
            element.Geometry = coverageArea;
            element.Geometry.SpatialReference = activeView.FocusMap.SpatialReference;

            var eprop = (IElementProperties)element;
            eprop.Name = areaName;

            var ge = new GraphicElement() { Source = coverageArea, Name = eprop.Name, Element = element };

            if (allGraphics[MilSpaceGraphicsTypeEnum.Visibility].Exists(el => el.Name == eprop.Name))
            {
                DeleteGraphicsElement(ge, true, true);
            }

            graphics.AddElement(element, 0);
            allGraphics[MilSpaceGraphicsTypeEnum.Visibility].Add(ge);

            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        public void AddCrossPointerToPoint(IPoint point, int length, string name)
        {
            int segmentLength;
            int segmentCount = 5;
            var color = grapchucsTypeColors[MilSpaceGraphicsTypeEnum.Visibility]();

            var north = EsriTools.GetNorthDirrection(point);
            var delta = north - 90;

            var angels = new double[]
            {
                delta,
                90 + delta,
                180+ delta,
                270 + delta
            };

            var fromPoints = new IPoint[]
            {
                point,
                point,
                point,
                point,
            };

            decimal segments;

            if (length % 5 != 0)
            {
                segments = length / 4;
            }
            else
            {
                segments = length / 5;
            }

            segmentLength = Convert.ToInt32(Math.Round(segments, 0));

            if (segmentLength >= 100)
            {
                segmentLength = segmentLength / 100 * 100;
            }
            else if (segmentLength >= 10)
            {
                segmentLength = segmentLength / 10 * 10;
            }

            for (int i = 1; i <= segmentCount; i++)
            {
                double segEndPointDistance = segmentLength * i;

                // for (int j = 0; j < 360; j += 90)
                int j = 0;
                foreach (var angel in angels)
                {
                    double radian = (90 + angel) * (Math.PI / 180);
                    var toPoint = EsriTools.GetPointFromAngelAndDistance(point, radian, segEndPointDistance);
                    var line = EsriTools.CreatePolylineFromPoints(fromPoints[j], toPoint);
                    var segmentName = name + "_" + j + "_" + i;
                    var ge = new GraphicElement() { Source = line, Name = segmentName };

                    if (j == 0 && i == segmentCount)
                    {
                        AddPolyline(ge, MilSpaceGraphicsTypeEnum.Visibility, color, LineType.Arrow, true, true, 1);
                    }
                    else
                    {
                        AddPolyline(ge, MilSpaceGraphicsTypeEnum.Visibility, color, LineType.Cross, true, true, 1);
                    }
                    fromPoints[j++] = toPoint.Clone();
                }
            }

            var markPoint = EsriTools.GetPointFromAngelAndDistance(point, 0, segmentLength);
            DrawText(markPoint, segmentLength.ToString(), $"text_{name}", MilSpaceGraphicsTypeEnum.Visibility, color, 9, 1);
        }

        public void AddObservPointsRelationLineToMap(IPolyline polyline, IRgbColor color, string name, string title)
        {
            var ge = new GraphicElement() { Source = polyline, Name = name };
            AddPolyline(ge, MilSpaceGraphicsTypeEnum.Visibility, color, LineType.DefaultLine, true, true);

            DrawText(polyline.ToPoint.Clone(), title, $"text_{name}", MilSpaceGraphicsTypeEnum.Visibility, color);
        }

        private static double GetLengthInMapUnits(IActiveView activeView, double mm)
        {
            if (activeView == null)
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

            if (pixelExtent == 0)
            {
                return -1;
            }

            var sizeOfOnePixel = (realWorldDisplayExtent / pixelExtent);

            return (pixels * sizeOfOnePixel);
        }

        public void RemoveGraphicsFromMap(string[] pointIds)
        {
            foreach (var pointId in pointIds)
            {
                var elements = allGraphics[MilSpaceGraphicsTypeEnum.GeoCalculator].Where(el => el.Name.Equals(pointId + geoCalcPointsSuffix));
                var geomCopyList = new List<GraphicElement>(elements);

                foreach (var geom in geomCopyList)
                {
                    DeleteGraphicsElement(geom, false, true);
                }
            }

            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        public void RemoveAllGeometryFromMap(string name, MilSpaceGraphicsTypeEnum graphicstype, bool contains = false)
        {
            IEnumerable<GraphicElement> geometry;

            if (contains)
            {
                geometry = allGraphics[graphicstype].Where(el => el.Name.Contains(name));
            }
            else
            {
                geometry = allGraphics[graphicstype].Where(el => el.Name.StartsWith(name));
            }

            var geomCopyList = new List<GraphicElement>(geometry);

            foreach (var geom in geomCopyList)
            {
                DeleteGraphicsElement(geom, false, true);
            }

            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        public void RemovePoint(string name)
        {
            var geometry = allGraphics[MilSpaceGraphicsTypeEnum.GeoCalculator].Where(el => el.Name.EndsWith(name + geoCalcPointsSuffix));
            var geomCopyList = new List<GraphicElement>(geometry);

            foreach (var geom in geomCopyList)
            {
                DeleteGraphicsElement(geom, false, true);
            }

            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        public void TestObjects(IGeometry geometry)
        {
            var color = new RgbColor { Red = 80, Blue = 150, Green = 150 };
            ISymbol symbol;
            IElement element = null;

            if (geometry.GeometryType == esriGeometryType.esriGeometryPolygon)
            {
                ISimpleFillSymbol simplePolygonSymbol = new SimpleFillSymbolClass
                {
                    Color = color,
                    Style = esriSimpleFillStyle.esriSFSSolid
                };

                IFillShapeElement markerElement = new PolygonElementClass
                {
                    Symbol = simplePolygonSymbol
                };

                symbol = simplePolygonSymbol as ISymbol;
                element = markerElement as IElement;
            }

            if (geometry.GeometryType == esriGeometryType.esriGeometryPolyline)
            {
                ISimpleLineSymbol simplePolylineSymbol = new SimpleLineSymbolClass
                {
                    Color = color,
                    Width = 4
                };

                symbol = simplePolylineSymbol as ISymbol;

                ILineElement lineElement = new LineElementClass
                {
                    Symbol = simplePolylineSymbol
                };

                element = lineElement as IElement;
            }

            if (geometry.GeometryType == esriGeometryType.esriGeometryPoint)
            {
                Type factoryType = Type.GetTypeFromProgID("esriDisplay.SimpleMarkerSymbol");
                string typeFactoryID = factoryType.GUID.ToString("B");

                ISimpleMarkerSymbol pointMarkerSymbol = new SimpleMarkerSymbolClass
                {
                    Color = color,
                    Style = esriSimpleMarkerStyle.esriSMSCircle,
                    Size = 10
                };

                IMarkerElement markerElement = new MarkerElementClass
                {
                    Symbol = pointMarkerSymbol
                };

                element = markerElement as IElement;
                symbol = pointMarkerSymbol as ISymbol;
            }

            if (element == null)
                return;
            element.Geometry = geometry;
            element.Geometry.SpatialReference = activeView.FocusMap.SpatialReference;

            var eprop = (IElementProperties)element;
            eprop.Name = "test";

            var ge = new GraphicElement() { Source = geometry, Name = eprop.Name, Element = element };

            if (allGraphics[MilSpaceGraphicsTypeEnum.Visibility].Exists(el => el.Name == eprop.Name))
            {
                DeleteGraphicsElement(ge, true, true);
            }

            graphics.AddElement(element, 0);
            allGraphics[MilSpaceGraphicsTypeEnum.Visibility].Add(ge);

            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        private bool CheckElementOnGraphics(GraphicElement milSpaceElement)
        {
            if (milSpaceElement == null)
            {
                return false;
            }

            graphics.Reset();
            IElement ge = graphics.Next();
            bool result = false;
            while (ge != null)
            {

                //if (ge is ILineElement line)
                //{
                //    var smbl = line.Symbol;
                //}
                if (ge.Equals(milSpaceElement.Element))
                {
                    result = true;
                    break;
                }
                ge = graphics.Next();
            }
            graphics.Reset();

            return result;
        }

        private void DeleteGraphicsElement(GraphicElement milSpaceElement, bool doRefresh = false, bool removeFromList = false)
        {
            if (CheckElementOnGraphics(milSpaceElement))
            {
                graphics.DeleteElement(milSpaceElement.Element);
                if (doRefresh)
                {
                    activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
                }

                if (removeFromList)
                {
                    allGraphics[MilSpaceGraphicsTypeEnum.GeoCalculator].Remove(milSpaceElement);
                }
            }
        }


        private void RemoveElementFromMapGraphics(string elementName)
        {
            graphics.Reset();
            IElement ge = graphics.Next();

            while (ge != null)
            {
                var currentElProperties = ge as IElementProperties;

                if (currentElProperties.Name == elementName)
                {
                    graphics.DeleteElement(ge);
                    return;
                }

                ge = graphics.Next();
            }

            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }
    }
}
