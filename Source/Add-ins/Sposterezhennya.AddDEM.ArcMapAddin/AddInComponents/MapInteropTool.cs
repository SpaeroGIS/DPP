using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Desktop.AddIns;
using System.Threading.Tasks;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Display;

namespace Sposterezhennya.AddDEM.ArcMapAddin
{
    public partial class MapInteropTool : ESRI.ArcGIS.Desktop.AddIns.Tool
    {
        private DockableDEMWindow dockableDEMWindow;

        protected override void OnActivate()
        {
            dockableDEMWindow = AddIn.FromID<DockableDEMWindow.AddinImpl>(ThisAddIn.IDs.DockableDEMWindow)?.UI;
            Cursor = System.Windows.Forms.Cursors.Cross;
            base.OnActivate();
        }

        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;
        }

        protected override void OnMouseDown(MouseEventArgs arg)
        {
            IMap map;
            IPoint clickedPoint = dockableDEMWindow.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(arg.X, arg.Y);

            map = dockableDEMWindow.ActiveMap;

            IActiveView activeView = (IActiveView)map;
            IRubberBand rubberEnv = new RubberEnvelopeClass();
            IGeometry geom = rubberEnv.TrackNew(activeView.ScreenDisplay, null);
            IArea area = (IArea)geom;

            //Extra logic to cater for the situation where the user simply clicks a point on the map 
            //or where envelope is so small area is zero 
            if ((geom.IsEmpty == true) || (area.Area == 0))
            {

                //create a new envelope 
                IEnvelope tempEnv = new EnvelopeClass();

                //create a small rectangle 
                ESRI.ArcGIS.esriSystem.tagRECT RECT = new tagRECT();
                RECT.bottom = 0;
                RECT.left = 0;
                RECT.right = 5;
                RECT.top = 5;

                //transform rectangle into map units and apply to the tempEnv envelope 
                ESRI.ArcGIS.Display.IDisplayTransformation dispTrans = activeView.ScreenDisplay.DisplayTransformation;
                dispTrans.TransformRect(tempEnv, ref RECT, 4); //4 = esriDisplayTransformationEnum.esriTransformToMap)
                tempEnv.CenterAt(clickedPoint);
                geom = (IGeometry)tempEnv;
            }

            //Set the spatial reference of the search geometry to that of the Map 
            ISpatialReference spatialReference = map.SpatialReference;
            geom.SpatialReference = spatialReference;

            dockableDEMWindow.SelectTilesByArea(geom);

            //map.SelectByShape(geom, null, false);
            //activeView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, activeView.Extent);

            if (arg.Button == System.Windows.Forms.MouseButtons.Left)
                dockableDEMWindow?.ArcMap_OnMouseDown(arg.X, arg.Y);
        }

        protected override void OnMouseMove(MouseEventArgs arg)
        {
            //if (arg.Button == System.Windows.Forms.MouseButtons.None)
            //    dockableDEMWindow.ArcMap_OnMouseMove(arg.X, arg.Y);
        }
    }
}

