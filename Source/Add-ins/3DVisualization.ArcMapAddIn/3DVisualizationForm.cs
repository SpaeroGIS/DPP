using ESRI.ArcGIS.Carto;
using MilSpace.Core;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.Visualization3D.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MilSpace.Visualization3D
{
    /// <summary>
    /// Designer class of the dockable window add-in. It contains user interfaces that
    /// make up the dockable window.
    /// </summary>
    public partial class Visualization3DMainForm : UserControl
    {
        private ProfilesTreeView profilesTreeView;
        private LocalizationContext context;
        private List<Models.TreeViewNodeModel> profilesModels = new List<Models.TreeViewNodeModel>();

        public Visualization3DMainForm(object hook)
        {
            InitializeComponent();
            LocalizeComponent();
            SubscribeForArcMapEvents();
            OnDocumentOpenFillDropdowns();
            this.Hook = hook;
            Helper.SetConfiguration();
        }

        /// <summary>
        /// Host object of the dockable window
        /// </summary>
        private object Hook
        {
            get;
            set;
        }

        /// <summary>
        /// Implementation class of the dockable window add-in. It is responsible for 
        /// creating and disposing the user interface class of the dockable window.
        /// </summary>
        public class AddinImpl : ESRI.ArcGIS.Desktop.AddIns.DockableWindow
        {
            private Visualization3DMainForm m_windowUI;

            public AddinImpl()
            {
            }

            protected override IntPtr OnCreateChild()
            {
                m_windowUI = new Visualization3DMainForm(this.Hook);
                return m_windowUI.Handle;
            }

            protected override void Dispose(bool disposing)
            {
                Visualization3DHandler.ClosingHandler();

                if (m_windowUI != null)
                    m_windowUI.Dispose(disposing);

                base.Dispose(disposing);
            }

        }

        #region Private methods
        private void SubscribeForArcMapEvents()
        {
            ArcMap.Events.OpenDocument += OnDocumentOpenFillDropdowns;
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
                this.lbl3DProfiles.Text = context.ArcSceneParamsLabel;
                this.lblProfiles.Text = context.ProfilesLabel;
                this.BuildingsHightLabel.Text = context.HightLablel;
                this.HydroHightLabel.Text = context.HightLablel;
                this.PlantsHightLablel.Text = context.HightLablel;
                this.TransportHightLabel.Text = context.HightLablel;
                this.BuildingsLayerLabel.Text = context.BuildingsLayerLabel;
                this.HydroLayerLabel.Text = context.HydroLayerLabel;
                this.PlantsLayerLabel.Text = context.PlantsLayerLabel;
                this.TransportLayerLabel.Text = context.TransportLayerLabel;
                this.ProfilesTabPage.Text = context.Generate3DSceneTabHeader;
                this.GenerateImageTab.Text = context.GenerateImageTabHeader;

                this.surfaceLabels.Text = context.SurfacessLabel;


                //Buttons
                this.GenerateButton.Text = context.GenerateButton;
            }
            catch { MessageBox.Show("No Localization.xml found or there is an error during loading. Coordinates Converter window is not fully localized."); }
        }

        private void OnDocumentOpenFillDropdowns()
        {
            this.SurfaceComboBox.Items.Clear();
            this.TransportLayerComboBox.Items.Clear();
            this.HydroLayerComboBox.Items.Clear();
            this.BuildingsLayerComboBox.Items.Clear();
            this.PlantsLayerComboBox.Items.Clear();

            PopulateComboBox(SurfaceComboBox, ProfileLayers.RasterLayers);
            PopulateComboBox(TransportLayerComboBox, ProfileLayers.PolygonLayers);
            PopulateComboBox(HydroLayerComboBox, ProfileLayers.PolygonLayers);
            PopulateComboBox(BuildingsLayerComboBox, ProfileLayers.PolygonLayers);
            PopulateComboBox(PlantsLayerComboBox, ProfileLayers.PolygonLayers);
        }

        private static void PopulateComboBox(ComboBox comboBox, IEnumerable<ILayer> layers)
        {
            comboBox.Items.AddRange(layers.Select(l => l.Name).ToArray());
        }
        #endregion

        #region Control Event Handlers       

        private void ToolBars_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if (AddProfile.Equals(e.Button))
            {
                profilesTreeView = new ProfilesTreeView(context);
                profilesTreeView.LoadProfiles();

                var dialogResult = profilesTreeView.ShowDialog(this);

                if (dialogResult == DialogResult.OK)
                {
                    profilesModels.AddRange(profilesTreeView.SelectedTreeViewNodes.Where(item => !profilesModels.Contains(item)));

                    ProfilesListBox.DataSource = profilesModels;
                    ProfilesListBox.DisplayMember = "Name";
                    ProfilesListBox.ValueMember = "NodeProfileSession";

                    profilesTreeView.Dispose();
                }
            }
            else if (RemoveProfile.Equals(e.Button))
            {
                foreach (var item in ProfilesListBox.SelectedItems)
                {
                    profilesModels.Remove(item as Models.TreeViewNodeModel);
                }
                //TODO: Remove SPIKE with DataBindings reassigning
                ProfilesListBox.DataSource = null;
                ProfilesListBox.DataSource = profilesModels;
                ProfilesListBox.DisplayMember = "Name";
                ProfilesListBox.ValueMember = "NodeProfileSession";
            }
        }

        private void GenerateButton_Click(object sender, EventArgs e)
        {
            var profilesSets = new List<ProfileSession>();

            foreach (var profileSetModel in profilesModels)
            {
                var profilesSet = profileSetModel.NodeProfileSession;
                profilesSet.ConvertLinesToEsriPolypile(ArcMap.Document.FocusMap.SpatialReference);

                profilesSets.Add(profilesSet);
            }

            try
            {
                if (SurfaceComboBox.SelectedItem != null)
                {
                    var arcSceneArguments = Feature3DManager.Get3DFeatures(SurfaceComboBox.SelectedItem.ToString(), profilesSets);
                    Visualization3DHandler.OpenProfilesSetIn3D(arcSceneArguments);
                }
                else
                {
                    //TODO: Exception message
                }
            }
            catch (Exception ex)
            {
                //TODO: Log Error

            }
        }
        #endregion
    }
}
