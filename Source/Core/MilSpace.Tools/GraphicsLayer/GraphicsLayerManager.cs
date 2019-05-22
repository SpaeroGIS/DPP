using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
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
        private static Dictionary<MilSpaceGraphicsTypeEnum, Func<IRgbColor>> grapchucsTypeColors = new Dictionary<MilSpaceGraphicsTypeEnum, Func<IRgbColor>>
        {
            { MilSpaceGraphicsTypeEnum.Calculating, () =>  new RgbColor(){ Red = 255,Green = 0, Blue = 0} },
            { MilSpaceGraphicsTypeEnum.Session, () =>  new RgbColor(){ Red = 0,Green = 255, Blue = 0} }
        };

        private Dictionary<MilSpaceGraphicsTypeEnum, List<GraphicElement>> allGraphics = new Dictionary<MilSpaceGraphicsTypeEnum, List<GraphicElement>>();

        private enum LineType { Point, Line, Arrow, DefaultLine };

        public GraphicsLayerManager(IActiveView activeView)
        {
            this.activeView = activeView;
            graphics = activeView.GraphicsContainer;
            allGraphics.Add(MilSpaceGraphicsTypeEnum.Calculating, milSpaceCalculatingGraphics);
            allGraphics.Add(MilSpaceGraphicsTypeEnum.Session, milSpaceSessionGraphics);

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
                LineType lineType;

                if (groupedLines.Lines.Count() == 1) { lineType = LineType.DefaultLine; }
                else if (groupedLines.Lines.Last() == groupedLines.Lines[lineNumber]) { lineType = LineType.Arrow; }
                else if (groupedLines.Lines.First() == groupedLines.Lines[lineNumber]) { lineType = LineType.Point; }
                else { lineType = LineType.Line; }

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
                EsriTools.FlashGeometry(activeView.ScreenDisplay, flashGeometry);
                activeView.Refresh();
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
                    var ge = new GraphicElement() { Source = line, ElementId = elementId, ProfileId = profileId, LineId = profileColorLines.LineId };
                    var color = (profileColorLines.Lines[lineNumber].Visible) ? profileColorLines.VisibleColor
                                                                              : profileColorLines.InvisibleColor;
                    LineType lineType;

                    if (profileColorLines.Lines.Count() == 1) { lineType = LineType.DefaultLine; }
                    else if (profileColorLines.Lines.Last() == profileColorLines.Lines[lineNumber]) { lineType = LineType.Arrow; }
                    else if (profileColorLines.Lines.First() == profileColorLines.Lines[lineNumber]) { lineType = LineType.Point; }
                    else { lineType = LineType.Line; }


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

        private void AddPolyline(GraphicElement graphicElement, MilSpaceGraphicsTypeEnum graphicsType,
                                    IRgbColor color = null, LineType lineType = LineType.DefaultLine, bool doRefresh = false,
                                    bool persist = false, int width = 2)
        {
            IPolyline profileLine = graphicElement.Source;
            ILineElement lineElement = new LineElementClass();

            var curList = allGraphics[graphicsType];

            bool exists = curList.Any(ge => ge.ProfileId == graphicElement.ProfileId
                                           && ge.ElementId == graphicElement.ElementId
                                           && ge.LineId == graphicElement.LineId);

            if (!persist && exists)
            {
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
            graphicElement.Element = elem;

            DeleteGraphicsElement(graphicElement);

            graphics.AddElement(elem, 0);


            if (!exists)
            {
                curList.Add(graphicElement);
            }

            if (doRefresh)
            {
                activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            }
        }

        private static ILineSymbol DefineProfileDecorationLineSymbol(MilSpaceGraphicsTypeEnum graphicsType, IRgbColor color,
                                                                        int width = 2, LineType lineType = LineType.Arrow)
        {
            //TODO: Get symbol from ESRITools

            //Create cartographic line symbol  
            ICartographicLineSymbol cartographicLineSymbol = new CartographicLineSymbolClass();
            cartographicLineSymbol.Color = color;
            cartographicLineSymbol.Width = width;

            //Define line decoration  
            ILineDecoration lineDecoration = new LineDecorationClass();

            if (lineType == LineType.Arrow || lineType == LineType.DefaultLine)
            {
                //Define simple line decoration  
                ISimpleLineDecorationElement simpleLineDecorationElement = new SimpleLineDecorationElementClass();

                //Place the arrow at the end of the line(the "To" point in the geometry below)
                simpleLineDecorationElement.AddPosition(1);

                //Define an arrow marker 
                IArrowMarkerSymbol arrowMarkerSymbol = new ArrowMarkerSymbolClass();
                arrowMarkerSymbol.Color = color;
                arrowMarkerSymbol.Size = 5;
                arrowMarkerSymbol.Length = 8;
                arrowMarkerSymbol.Width = 5 + width;

                //Add an offset to make sure the square end of the line is hidden  
                arrowMarkerSymbol.XOffset = 0.8;
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

            //Set line properties  
            ILineProperties lineProperties = (ILineProperties)cartographicLineSymbol;
            lineProperties.LineDecoration = lineDecoration;

            return cartographicLineSymbol;
        }

        private static ILineSymbol DefineProfileLineSymbol(MilSpaceGraphicsTypeEnum graphicsType, IRgbColor color, int width = 2)
        {
            //Create cartographic line symbol  
            ICartographicLineSymbol cartographicLineSymbol = new CartographicLineSymbolClass();
            cartographicLineSymbol.Color = color;
            cartographicLineSymbol.Width = width;

            return cartographicLineSymbol;
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

        private void DeleteGraphicsElement(GraphicElement milSpaceElement, bool doRefresh = false)
        {
            if (CheckElementOnGraphics(milSpaceElement))
            {
                graphics.DeleteElement(milSpaceElement.Element);
                if (doRefresh)
                {
                    activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
                }
            }
        }
    }
}
