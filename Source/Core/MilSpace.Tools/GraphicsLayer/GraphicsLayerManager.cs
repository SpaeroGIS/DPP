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


            //  Line elements
            ISimpleLineSymbol simpleLineSymbol = new SimpleLineSymbol();

            IRgbColor col2 = new RgbColor();
            col2.Red = 255;
            col2.Green = 0;
            col2.Blue = 0;

            simpleLineSymbol.Color = new RgbColor();
            simpleLineSymbol.Color = col2;
            simpleLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
            simpleLineSymbol.Width = 2;


            lineElement.Symbol = simpleLineSymbol;
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

    }
}
