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

            IDockableWindow dockWindow = DockableWindowMilSpaceProfileCalc.AddinImpl.AtivateDocableWindow();
            dockWindow.Show(true);  

        }
        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;
        }
    }

}
