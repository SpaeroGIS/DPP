using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core.Tools;
using MilSpace.DataAccess;
using MilSpace.DataAccess.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Tools.GraphicsLayer
{
    public class GraphicsLayerManager
    {
        IGraphicsContainer graphics;
        IActiveView activeView;
        private List<GraphicElement> milSpaceCalculatingGraphics = new List<GraphicElement>();
        private List<GraphicElement> milSpaceSessionGraphics = new List<GraphicElement>();
        private static Dictionary<MilSpaceGraphicsTypeEnum, Func<IRgbColor>> grapchucsTypeColors = new Dictionary<MilSpaceGraphicsTypeEnum, Func<IRgbColor>>
        {
            { MilSpaceGraphicsTypeEnum.Calculating, () =>  new RgbColor(){ Red = 255,Green = 0, Blue = 0} },
            { MilSpaceGraphicsTypeEnum.Session, () =>  new RgbColor(){ Red = 0,Green = 255, Blue = 0} }
        };

        private Dictionary<MilSpaceGraphicsTypeEnum, List<GraphicElement>> allGraphics = new Dictionary<MilSpaceGraphicsTypeEnum, List<GraphicElement>>();


        public GraphicsLayerManager(IActiveView activeView)
        {
            this.activeView = activeView;
            graphics = activeView.GraphicsContainer;
            allGraphics.Add(MilSpaceGraphicsTypeEnum.Calculating, milSpaceCalculatingGraphics);
            allGraphics.Add(MilSpaceGraphicsTypeEnum.Session, milSpaceSessionGraphics);

        }

        private void UpdateGraphic(IEnumerable<IPolyline> profileLines, int profileId, int profileTypeId, MilSpaceGraphicsTypeEnum graphicsType)
        {
            EmptyProfileGraphics(graphicsType);

            int elementId = profileTypeId;
            foreach (var line in profileLines)
            {
                var ge = new GraphicElement() { Source = line, ElementId = ++elementId, ProfileId = profileId };
                AddPolyline(ge, graphicsType);
            }

            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);

        }

        private void UpdateGraphicLine(IEnumerable<IPolyline> profileLines, int profileId,
                                       MilSpaceGraphicsTypeEnum graphicsType, GroupedLines groupedLines,
                                       IRgbColor visibleColor, IRgbColor invisibleColor)
        {
            RemoveLineFromSessionGraphicsByLineId(profileId, groupedLines.LineId, graphicsType);

            int elementId = 0;
            int lineNumber = 0;

            foreach (var line in profileLines)
            {
                var ge = new GraphicElement()
                {
                    Source = line,
                    ElementId = ++elementId,
                    ProfileId = profileId,
                    LineId = groupedLines.LineId
                };

                var color = (groupedLines.Lines[lineNumber].Visible) ? visibleColor : invisibleColor;

                if (groupedLines.Lines.Last() == groupedLines.Lines[lineNumber])
                {
                    AddPolyline(ge, graphicsType, color, false);
                }
                else
                {
                    AddPolyline(ge, graphicsType, color, true);
                }

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
                foreach(var graphic in graphicLine)
                {
                    DeleteGraphicsElement(graphic);
                    curList.Remove(graphic);
                }
            }

            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }


        public void RemoveLinesFromSessionGraphics(IEnumerable<IPolyline> profileLines, int profileId, int profileTypeId)
        {
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

        public void FlashLineOnWorkingGraphics(int profileId, int lineId)
        {
            var curList = allGraphics[MilSpaceGraphicsTypeEnum.Session];
            IGeometry flashGeometry = null;
            if (lineId > 0)
            {
                int elementId = lineId;

                var graphic = curList.FirstOrDefault(g => g.ProfileId == profileId && g.ElementId == elementId);
                if (graphic != null)
                {
                    flashGeometry = graphic.Source;
                }
            }
            else
            {
                var graphics = curList.Where(g => g.ProfileId == profileId).Select(g => g.Source).ToList();
                IGeometryCollection theGeomColl = new GeometryBagClass();
                graphics.ForEach(pl => theGeomColl.AddGeometry(pl));

                ITopologicalOperator theTopoOp = new PolylineClass();
                theTopoOp.ConstructUnion((IEnumGeometry)theGeomColl);

                flashGeometry = theTopoOp as IGeometry;
            }

            if (flashGeometry != null)
            {
                EsriTools.FlashGeometry(activeView.ScreenDisplay, flashGeometry);
                activeView.Refresh();
            }
        }


        public void ShowLineOnWorkingGraphics(int profileId, int lineId)
        {

            var curList = allGraphics[MilSpaceGraphicsTypeEnum.Session];
            int elementId = lineId;

            var graphic = curList.FirstOrDefault(g => g.ProfileId == profileId && g.ElementId == elementId);
            AddPolyline(graphic, MilSpaceGraphicsTypeEnum.Session, null, false, true, true);

        }

        public void HideLineFromWorkingGraphics(int profileId, int lineId)
        {

            var curList = allGraphics[MilSpaceGraphicsTypeEnum.Session];
            int elementId = lineId;

            var graphic = curList.FirstOrDefault(g => g.ProfileId == profileId && g.ElementId == elementId);
            DeleteGraphicsElement(graphic, true);
        }

        public void RemoveLinesFromWorkingGraphics(IEnumerable<IPolyline> profileLines, int profileId)
        {
            RemoveLinesFromGraphics(profileLines, profileId, MilSpaceGraphicsTypeEnum.Session);
        }

        public void RemoveGraphic(int profileId, List<int> linesIds)
        {
            foreach(var id in linesIds)
            {
                RemoveLineFromSessionGraphicsByLineId(profileId, id, MilSpaceGraphicsTypeEnum.Session);
            }
        }

        public void RemoveLineFromGraphic(int profileId, int lineId)
        {
            RemoveLineFromSessionGraphicsByLineId(profileId, lineId, MilSpaceGraphicsTypeEnum.Session);
        }

        public void AddLinesToWorkingGraphics(IEnumerable<IPolyline> profileLines, int profileId, GroupedLines profileColorLines = null,
                                                RgbColor visibleColor = null, RgbColor invisibleColor = null)
        {
            AddLinesToGraphics(profileLines, profileId, MilSpaceGraphicsTypeEnum.Session, profileColorLines, visibleColor, invisibleColor);
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

        public void UpdateGraphicLine(IEnumerable<IPolyline> profileLines, int profileId, GroupedLines groupedLines,
                                        IRgbColor visibleColor, IRgbColor invisibleColor)
        {
            UpdateGraphicLine(profileLines, profileId, MilSpaceGraphicsTypeEnum.Session,
                                groupedLines, visibleColor, invisibleColor);
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
            AddPolyline(graphicElement, MilSpaceGraphicsTypeEnum.Calculating, null, false);
        }

        private void AddLinesToGraphics(IEnumerable<IPolyline> profileLines, int profileId,
                                            MilSpaceGraphicsTypeEnum graphicsType, GroupedLines profileColorLines = null,
                                            IRgbColor visibleColor = null, IRgbColor invisibleColor = null)
        {
            var curList = allGraphics[graphicsType];
            int elementId = 0;
            var lineNumber = 0;

            if (profileColorLines != null)
            {
                RemoveLineFromSessionGraphicsByLineId(profileId, profileColorLines.LineId, graphicsType);
            }

            foreach (var line in profileLines)
            {
                elementId++;

                if (profileColorLines != null)
                {
                    var ge = new GraphicElement() { Source = line, ElementId = elementId, ProfileId = profileId, LineId = profileColorLines.LineId };
                    var color = (profileColorLines.Lines[lineNumber].Visible) ? visibleColor : invisibleColor;

                    if (profileColorLines.Lines.Last() == profileColorLines.Lines[lineNumber])
                    {
                        AddPolyline(ge, graphicsType, color, false);
                    }
                    else
                    {
                        AddPolyline(ge, graphicsType, color, true);
                    }

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

                    var ge = new GraphicElement() { Source = line, ElementId = elementId, ProfileId = profileId };
                    AddPolyline(ge, graphicsType);
                }

            }
            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        public void RemoveLinesFromGraphics(IEnumerable<IPolyline> profileLines, int profileId, MilSpaceGraphicsTypeEnum graphicsType)
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

        private void AddPolyline(GraphicElement graphicElement, MilSpaceGraphicsTypeEnum graphicsType,
                                    IRgbColor color = null, bool isLine = false, bool doRefresh = false,
                                    bool persist = false)
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

            if (!isLine)
            {
                lineElement.Symbol = DefineProfileArrowLineSymbol(graphicsType, color);

            }
            else
            {
                lineElement.Symbol = DefineProfileLineSymbol(graphicsType, color);
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

        private static ILineSymbol DefineProfileArrowLineSymbol(MilSpaceGraphicsTypeEnum graphicsType, IRgbColor color)
        {
            //TODO: Get symbol from ESRITools
            //  IRgbColor rgbColor = grapchucsTypeColors[graphicsType]();

            //Define an arrow marker 
            IArrowMarkerSymbol arrowMarkerSymbol = new ArrowMarkerSymbolClass();
            arrowMarkerSymbol.Color = color;
            arrowMarkerSymbol.Size = 4;
            arrowMarkerSymbol.Length = 8;
            arrowMarkerSymbol.Width = 6;

            //Create cartographic line symbol  
            ICartographicLineSymbol cartographicLineSymbol = new CartographicLineSymbolClass();
            cartographicLineSymbol.Color = color;
            cartographicLineSymbol.Width = 1;

            //Define simple line decoration  
            ISimpleLineDecorationElement simpleLineDecorationElement = new SimpleLineDecorationElementClass();
            //Place the arrow at the end of the line (the "To" point in the geometry below)  
            simpleLineDecorationElement.AddPosition(1);

            //Add an offset to make sure the square end of the line is hidden  
            arrowMarkerSymbol.XOffset = 0.8;
            simpleLineDecorationElement.MarkerSymbol = arrowMarkerSymbol;

            //Define line decoration  
            ILineDecoration lineDecoration = new LineDecorationClass();
            lineDecoration.AddElement(simpleLineDecorationElement);

            //Set line properties  
            ILineProperties lineProperties = (ILineProperties)cartographicLineSymbol;
            lineProperties.LineDecoration = lineDecoration;

            return cartographicLineSymbol;
        }

        private static ILineSymbol DefineProfileLineSymbol(MilSpaceGraphicsTypeEnum graphicsType, IRgbColor color, bool isLine = false)
        {
            //Create cartographic line symbol  
            ICartographicLineSymbol cartographicLineSymbol = new CartographicLineSymbolClass();
            cartographicLineSymbol.Color = color;
            cartographicLineSymbol.Width = 1;

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
