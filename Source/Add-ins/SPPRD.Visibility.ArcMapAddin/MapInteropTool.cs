using ESRI.ArcGIS.Desktop.AddIns;

namespace MilSpace.Visibility
{
    public partial class MapInteropTool : ESRI.ArcGIS.Desktop.AddIns.Tool
    {
        private DockableWindowMilSpaceMVisibilitySt dockableWindowMilSpaceMVisibilitySt;

        protected override void OnActivate()
        {
            dockableWindowMilSpaceMVisibilitySt = AddIn.FromID<DockableWindowMilSpaceMVisibilitySt.AddinImpl>(ThisAddIn.IDs.DockableWindowMilSpaceMVisibilitySt)?.UI;
            Cursor = System.Windows.Forms.Cursors.Arrow;
            base.OnActivate();            
        }

        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;            
        }

        protected override void OnMouseDown(MouseEventArgs arg)
        {
            if (arg.Button == System.Windows.Forms.MouseButtons.Left)
                dockableWindowMilSpaceMVisibilitySt?.ArcMap_OnMouseDown(arg.X, arg.Y);
        }

        protected override void OnMouseMove(MouseEventArgs arg)
        {
            if (arg.Button == System.Windows.Forms.MouseButtons.None)
                dockableWindowMilSpaceMVisibilitySt.ArcMap_OnMouseMove(arg.X, arg.Y);            
        }       
    }
}
