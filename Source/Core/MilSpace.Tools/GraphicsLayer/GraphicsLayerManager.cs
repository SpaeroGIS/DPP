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
        private Dictionary<int, GraphicElement> milSpaceGraphics = new Dictionary<int, GraphicElement>();

        public GraphicsLayerManager(IActiveView activeView)
        {
            this.activeView = activeView;
            graphics = activeView.GraphicsContainer;
        }


        public void UpdateGraphic(IEnumerable<IPolyline> profileLines, int profileId)
        {
            var toDel = milSpaceGraphics.Keys.Where(k => milSpaceGraphics[k].ProfileId != profileId ||
                                                    !profileLines.Any(p => p.GetHashCode() == k));

            foreach(int id in toDel)
            {
                var elem = milSpaceGraphics[id];

                RemovePolyline(elem.Element.Geometry as IPolyline);
            }

            foreach (var line in profileLines)
            {
                AddPolyline(line, profileId);
            }

            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        public void AddPolyline(IPolyline profileLine, int profileId, bool doFeresh = false)
        {
            
            ILineElement lineElement = new LineElementClass();

            int id = profileLine.GetHashCode();

            if (milSpaceGraphics.ContainsKey(id))
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

            milSpaceGraphics.Add(id, new GraphicElement() { Element = elem, ElementId = id, ProfileId = profileId });

            if (doFeresh)
            {
                activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            }
        }

        public void RemovePolyline(IPolyline profileLine, bool doFeresh = false)
        {
            ILineElement lineElement = new ESRI.ArcGIS.Carto.LineElementClass();
            IElement elem = lineElement as IElement;
            elem.Geometry = profileLine;

            if (milSpaceGraphics.ContainsKey(profileLine.GetHashCode()))
            {
                Removeelement(milSpaceGraphics[profileLine.GetHashCode()].Element);
            }

            if (doFeresh)
            {
                activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            }

        }

        private void Removeelement(IElement elem)
        {
            graphics.DeleteElement(elem);
            milSpaceGraphics.Remove(elem.Geometry.GetHashCode());
        }
    }
}
