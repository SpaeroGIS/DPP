using ESRI.ArcGIS.Desktop.AddIns;
using System;
using System.Runtime.InteropServices;

namespace MilSpace.GeoCalculator
{
    [Guid("B22EEC3E-7BD1-4407-9934-88150C6B7FD1")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("ArcMapAddin_MapInteropTool")]
    public class MapInteropTool : ESRI.ArcGIS.Desktop.AddIns.Tool
    {
        private DockableWindowGeoCalculator dockableWindowGeoCalculator;

        protected override void OnActivate()
        {
            dockableWindowGeoCalculator = AddIn.FromID<DockableWindowGeoCalculator.AddinImpl>(ThisAddIn.IDs.DockableWindowGeoCalculator)?.UI;            
            base.OnActivate();            
        }

        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;            
        }

        protected override void OnMouseDown(MouseEventArgs arg)
        {
            if (arg.Button == System.Windows.Forms.MouseButtons.Left)
                dockableWindowGeoCalculator?.ArcMap_OnMouseDown(arg.X, arg.Y);
        }

        protected override void OnMouseMove(MouseEventArgs arg)
        {
            if (arg.Button == System.Windows.Forms.MouseButtons.None)
                dockableWindowGeoCalculator.ArcMap_OnMouseMove(arg.X, arg.Y);            
        }       
    }
}
