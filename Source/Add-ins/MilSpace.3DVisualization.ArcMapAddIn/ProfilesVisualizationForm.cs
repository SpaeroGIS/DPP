using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.Facade;
using MilSpace.Visualization3D.ReferenceData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MilSpace.Visualization3D
{
    internal partial class ProfilesVisualizationForm : Form
    {
        private ProfilesTreeView profilesTreeView;
        private LocalizationContext context;

        internal ProfilesVisualizationForm()
        {
            InitializeComponent();
            LocalizeComponent();            
        }        

        private void LocalizeComponent()
        {
            try
            {
                context = new LocalizationContext();

                //Captions
                this.Text = context.WindowCaption;

                //Labels
                this.SurfaceLabel.Text = context.SurfaceLabel;
                this.BuildingsHightLabel.Text = context.HightLablel;
                this.HydroHightLabel.Text = context.HightLablel;
                this.PlantsHightLablel.Text = context.HightLablel;
                this.TransportHightLabel.Text = context.HightLablel;
                this.BuildingsLayerLabel.Text = context.BuildingsLayerLabel;
                this.HydroLayerLabel.Text = context.HydroLayerLabel;
                this.PlantsLayerLabel.Text = context.PlantsLayerLabel;
                this.TransportLayerLabel.Text = context.TransportLayerLabel;
                this.ProfilesTabPage.Text = context.ProfilesLabel;

                //Buttons
                this.GenerateButton.Text = context.GenerateButton;
            }
            catch { MessageBox.Show("No Localization.xml found or there is an error during loading. Coordinates Converter window is not fully localized."); }
        }

        #region Control Event Handlers
        private void ProfilesVisualizationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Visible = false;
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (profilesTreeView == null) profilesTreeView = new ProfilesTreeView(context);

            profilesTreeView.LoadProfiles();

            profilesTreeView.ShowDialog(this);
        }
        #endregion
    }
}
