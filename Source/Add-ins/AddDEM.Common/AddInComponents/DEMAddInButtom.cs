using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;

namespace Sposterezhennya.AddDEM.ArcMapAddin
{
    public class DEMAddInButtom : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        protected override void OnClick()
        {
            UID dockWinID = new UIDClass
            {
                Value = ThisAddIn.IDs.DockableDEMWindow
            };

            IDockableWindow dockWindow = ArcMap.DockableWindowManager.GetDockableWindow(dockWinID);
            dockWindow.Show(true);

        }
    }

}
