using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
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
        private List<GraphicElement> milSpaceGraphics = new List<GraphicElement>();

        public GraphicsLayerManager(IActiveView activeView)
        {
            this.activeView = activeView;
            graphics = activeView.GraphicsContainer;
        }


        public void UpdateGraphic(IEnumerable<IPolyline> profileLines, int profileId, int profileTypeId)
        {

            EmptyProfileGraphics();

            int elementId = profileTypeId * 100;
            foreach (var line in profileLines)
            {
                var ge = new GraphicElement() { Source = line, ElementId = ++elementId, ProfileId = profileId };
                AddPolyline(ge);
            }

            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        public void EmptyProfileGraphics()
        {

            graphics.DeleteAllElements();
            milSpaceGraphics.RemoveRange(0, milSpaceGraphics.Count);

            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        public void AddPolyline(GraphicElement graphicElement, bool doFeresh = false)
        {
            IPolyline profileLine = graphicElement.Source;
            ILineElement lineElement = new LineElementClass();

            int id = profileLine.GetHashCode();

            if (milSpaceGraphics.Any(ge => ge.ElementId == graphicElement.ElementId))
            {
                return;
            }


            lineElement.Symbol = DefineProfileLineSymbol();
            IElement elem = (IElement)lineElement;
            elem.Geometry = profileLine;

            graphics.AddElement(elem, 0);

            graphicElement.Element = elem;
            milSpaceGraphics.Add(graphicElement);

            if (doFeresh)
            {
                activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            }
        }

        private static ILineSymbol DefineProfileLineSymbol()
        {

            IRgbColor rgbColor = new RgbColor()
            {
                Red = 255,
                Green = 0,
                Blue = 0
            };


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
