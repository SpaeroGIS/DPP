using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;

namespace MilSpace.Visualization3D
{
    public class Button3DVisualization : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public Button3DVisualization()
        {
        }

        protected override void OnClick()
        {
            //if (profilesForm == null)
            //    profilesForm = new ProfilesVisualizationForm();

            //profilesForm.FormClosing += ProfilesForm_FormClosing; 

            //if (!profilesForm.Visible)
            //    profilesForm.Show();
            //else
            //    profilesForm.Visible = false;

            //Checked = profilesForm.Visible;
       
            UID dockWinID = new UIDClass
            {
                Value = ThisAddIn.IDs.Visualization3DMainForm
            };
            IDockableWindow dockWindow = ArcMap.DockableWindowManager.GetDockableWindow(dockWinID);
            dockWindow.Show(true);

            ArcMap.Application.CurrentTool = null;
        }       

        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;
        }       
    }

}
