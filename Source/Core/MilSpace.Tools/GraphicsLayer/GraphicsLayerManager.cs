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
            int elementId = profileTypeId * 100;

            var lines = profileLines.Select(l => new GraphicElement() { Source = l, ElementId = ++elementId, ProfileId = profileId });


            var toDel = milSpaceGraphics.Where(k => k.ProfileId != profileId ||
                                                    !lines.Any(p => p.ElementId == k.ElementId));

            foreach(var ge in toDel)
            {

                RemovePolyline(ge);
            }

            foreach (var line in lines)
            {
                AddPolyline(line);
            }

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
            simpleLineSymbol.Style = ESRI.ArcGIS.Display.esriSimpleLineStyle.esriSLSSolid;
            simpleLineSymbol.Width = 15;
         

            lineElement.Symbol = simpleLineSymbol;
            IElement elem = (IElement)lineElement;
            elem.Geometry = profileLine;

            graphics.AddElement(elem, 0); // Explicit Cast


            graphicElement.Element = elem;
            milSpaceGraphics.Add(graphicElement);

            if (doFeresh)
            {
                activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            }
        }

        public void RemovePolyline(GraphicElement grephicElement, bool doFeresh = false)
        {
            ILineElement lineElement = new ESRI.ArcGIS.Carto.LineElementClass();
            IElement elem = lineElement as IElement;
            elem.Geometry = grephicElement.Source;

            if (milSpaceGraphics.Any(ge => grephicElement.ElementId == ge.ElementId))
            {
                Removeelement(milSpaceGraphics.First(ge => grephicElement.ElementId == ge.ElementId));
            }

            if (doFeresh)
            {
                activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            }

        }

        private void Removeelement(GraphicElement elem)
        {
            graphics.DeleteElement(elem.Element);
            milSpaceGraphics.Remove(elem);
        }
    }
}
