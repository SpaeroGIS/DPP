using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;

namespace MilSpace.Profile
{
    public class MilSpaceProfileCalcOpen : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public MilSpaceProfileCalcOpen()
        {
        }

        protected override void OnClick()
        {
            ArcMap.Application.CurrentTool = null;
            UID dockWinID = new UIDClass();
            dockWinID.Value = ThisAddIn.IDs.DockableWindowMilSpaceProfileCalc;
            IDockableWindow dockWindow = ArcMap.DockableWindowManager.GetDockableWindow(dockWinID);
            dockWindow.Show(true);  

        }
        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;
        }
    }

}
