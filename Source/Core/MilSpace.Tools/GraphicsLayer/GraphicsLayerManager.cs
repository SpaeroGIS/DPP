using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core.Tools;
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

            int elementId = profileTypeId * 100;
            foreach (var line in profileLines)
            {
                var ge = new GraphicElement() { Source = line, ElementId = ++elementId, ProfileId = profileId };
                AddPolyline(ge, graphicsType);
            }

            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);

        }

        public void AddLinesToSessionGraphics(IEnumerable<IPolyline> profileLines, int profileId, int profileTypeId)
        {

            int elementId = profileId * 100;
            foreach (var line in profileLines)
            {
                var graphic = milSpaceSessionGraphics.FirstOrDefault(g => g.ElementId == ++elementId);
                if (graphic != null)
                {
                    graphics.DeleteElement(graphic.Element);
                    milSpaceSessionGraphics.Remove(graphic);
                }

                var ge = new GraphicElement() { Source = line, ElementId = elementId, ProfileId = profileId };
                AddPolyline(ge, MilSpaceGraphicsTypeEnum.Session);
            }
        }

        public void UpdateWorkingingGraphics(IEnumerable<IPolyline> profileLines, int profileId, int profileTypeId)
        {
            UpdateGraphic(profileLines, profileId, profileTypeId, MilSpaceGraphicsTypeEnum.Session);
        }

        public void UpdateCalculatingGraphic(IEnumerable<IPolyline> profileLines, int profileId, int profileTypeId)
        {
            UpdateGraphic(profileLines, profileId, profileTypeId, MilSpaceGraphicsTypeEnum.Calculating);
        }

        public void EmptyProfileGraphics(MilSpaceGraphicsTypeEnum profileType)
        {
            var curList = allGraphics[profileType];
            curList.ForEach(e => graphics.DeleteElement(e.Element));
            curList.RemoveRange(0, curList.Count);
            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        public void AddCalculationPolyline(GraphicElement graphicElement, bool doFeresh = false)
        {
            AddPolyline(graphicElement, MilSpaceGraphicsTypeEnum.Calculating, false);
        }
        private void AddPolyline(GraphicElement graphicElement, MilSpaceGraphicsTypeEnum graphicsType, bool doFeresh = false)
        {
            IPolyline profileLine = graphicElement.Source;
            ILineElement lineElement = new LineElementClass();

            int id = profileLine.GetHashCode();

            if (milSpaceCalculatingGraphics.Any(ge => ge.ElementId == graphicElement.ElementId))
            {
                return;
            }

            lineElement.Symbol = DefineProfileLineSymbol(graphicsType);
            IElement elem = (IElement)lineElement;
            elem.Geometry = profileLine;

            graphics.AddElement(elem, 0);

            graphicElement.Element = elem;
            var curList = allGraphics[graphicsType];
            curList.Add(graphicElement);

            if (doFeresh)
            {
                activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            }
        }

        private static ILineSymbol DefineProfileLineSymbol(MilSpaceGraphicsTypeEnum graphicsType)
        {

            IRgbColor rgbColor = grapchucsTypeColors[graphicsType]();


            //Define an arrow marker  
            IArrowMarkerSymbol arrowMarkerSymbol = new ArrowMarkerSymbolClass();
            arrowMarkerSymbol.Color = rgbColor;
            arrowMarkerSymbol.Size = 6;
            arrowMarkerSymbol.Length = 8;
            arrowMarkerSymbol.Width = 6;
            //Add an offset to make sure the square end of the line is hidden  
            arrowMarkerSymbol.XOffset = 0.8;

            //Create cartographic line symbol  
            ICartographicLineSymbol cartographicLineSymbol = new CartographicLineSymbolClass();
            cartographicLineSymbol.Color = rgbColor;
            cartographicLineSymbol.Width = 1;

            //Define simple line decoration  
            ISimpleLineDecorationElement simpleLineDecorationElement = new SimpleLineDecorationElementClass();
            //Place the arrow at the end of the line (the "To" point in the geometry below)  
            simpleLineDecorationElement.AddPosition(1);
            simpleLineDecorationElement.MarkerSymbol = arrowMarkerSymbol;

            //Define line decoration  
            ILineDecoration lineDecoration = new LineDecorationClass();
            lineDecoration.AddElement(simpleLineDecorationElement);

            //Set line properties  
            ILineProperties lineProperties = (ILineProperties)cartographicLineSymbol;
            lineProperties.LineDecoration = lineDecoration;

            return cartographicLineSymbol;
        }
    }
}
