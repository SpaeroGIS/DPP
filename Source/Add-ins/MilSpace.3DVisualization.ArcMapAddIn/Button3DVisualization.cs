using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MilSpace.Visualization3D
{
    public class Button3DVisualization : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        private System.Windows.Forms.Form profilesForm;        

        public Button3DVisualization()
        {
        }

        protected override void OnClick()
        {
            if (profilesForm == null)
                profilesForm = new ProfilesVisualizationForm();

            profilesForm.FormClosing += ProfilesForm_FormClosing; 

            if (!profilesForm.Visible)
                profilesForm.Show();
            else
                profilesForm.Visible = false;

            Checked = profilesForm.Visible;
            
            ArcMap.Application.CurrentTool = null;
        }

        private void ProfilesForm_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            Checked = false;
        }

        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;
        }       
    }

}
