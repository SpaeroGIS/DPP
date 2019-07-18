using ESRI.ArcGIS.Desktop.AddIns;

namespace MilSpace.GeoCalculator
{
    public partial class MapInteropTool : ESRI.ArcGIS.Desktop.AddIns.Tool
    {
        private DockableWindowGeoCalculator dockableWindowGeoCalculator;

        protected override void OnActivate()
        {
            dockableWindowGeoCalculator = AddIn.FromID<DockableWindowGeoCalculator.AddinImpl>(ThisAddIn.IDs.DockableWindowGeoCalculator)?.UI;
            Cursor = System.Windows.Forms.Cursors.Cross;
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
