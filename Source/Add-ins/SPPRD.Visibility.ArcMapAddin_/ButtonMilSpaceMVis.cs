using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;

namespace MilSpace.Visibility
{
    public class ButtonMilSpaceMVis : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public ButtonMilSpaceMVis()
        {
        }

        protected override void OnClick()
        {
            ArcMap.Application.CurrentTool = null;
            UID dockWinID = new UIDClass();
            dockWinID.Value = ThisAddIn.IDs.DockableWindowMilSpaceMVisibilitySt;
            IDockableWindow dockWindow = ArcMap.DockableWindowManager.GetDockableWindow(dockWinID);
            dockWindow.Show(true);
        }
        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;
        }
    }

}
