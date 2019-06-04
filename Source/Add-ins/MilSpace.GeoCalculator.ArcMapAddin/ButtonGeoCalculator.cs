using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Desktop.AddIns;

namespace ArcMapAddin
{
    public class ButtonGeoCalculator : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public ButtonGeoCalculator()
        {
        }

        protected override void OnClick()
        {
            UID dockWinID = new UIDClass
            {
                Value = ThisAddIn.IDs.DockableWindowGeoCalculator
            };
            IDockableWindow dockWindow = ArcMap.DockableWindowManager.GetDockableWindow(dockWinID);
            dockWindow.Show(true);
            ActivateMapTool();
        }

        private void ActivateMapTool()
        {
            UID mapToolID = new UIDClass
            {
                Value = ThisAddIn.IDs.MapInteropTool
            };
            var documentBars = ArcMap.Application.Document.CommandBars;
            var cmdItem = documentBars.Find(mapToolID, false, false);
            ArcMap.Application.CurrentTool = cmdItem;
        }

        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;
        }
    }

}
