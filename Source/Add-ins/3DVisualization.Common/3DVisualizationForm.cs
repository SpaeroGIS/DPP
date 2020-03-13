using ESRI.ArcGIS.Carto;
using MilSpace.Core;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.Visualization3D.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private BindingList<TreeViewNodeModel> profilesModels = new BindingList<TreeViewNodeModel>();
        private BindingList<VisibilityResultModel> visibilitySessionsModel = new BindingList<VisibilityResultModel>();

        private static Logger log = Logger.GetLoggerEx("MilSpace.Visualization3D.Visualization3DMainForm");

        public Visualization3DMainForm(object hook)
        {
            log.DebugEx(">>> Visualization3DMainForm START <<<");

            InitializeComponent();
            LocalizeComponent();
            SetSessionsListView();
            SubscribeForArcMapEvents();
            OnDocumentOpenFillDropdowns();
            this.Hook = hook;

            Helper.SetConfiguration();

            log.DebugEx("> Visualization3DMainForm END");
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
            log.InfoEx("> SubscribeForArcMapEvents START");

            ArcMap.Events.OpenDocument += OnDocumentOpenFillDropdowns;

            log.InfoEx("> SubscribeForArcMapEvents END");
        }

        private void SetSessionsListView()
        {
            SessionsListBox.DataSource = visibilitySessionsModel;
            SessionsListBox.DisplayMember = "Gui";
        }

        private void LocalizeComponent()
        {
            log.InfoEx("> LocalizeComponent START");
            try
            {
                context = new LocalizationContext();

                //Captions
                this.Text = context.WindowCaption;

                //Labels
                this.SurfaceLabel.Text = context.SurfaceLabel;
                this.lbl3DProfiles.Text = context.ArcSceneParamsLabel;
                this.lblProfiles.Text = context.ProfilesLabel;
                this.BuildingsHight.Text = context.HightLablel;
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
                this.lblZFactor.Text = context.lblZFactor;

                //Buttons
                this.GenerateButton.Text = context.GenerateButton;
                toolTip.SetToolTip(btnRefreshLayers, context.ButtonUpdateToolTip);
                this.AddSurface.ToolTipText = context.ButtonAddSurfaceToolTip;
                this.RemoveSurface.ToolTipText = context.ButtonRemoveSurfaceToolTip;
                this.AddProfile.ToolTipText = context.ButtonAddProfileToolTip;
                this.RemoveProfile.ToolTipText = context.ButtonRemoveProfileToolTip;

                log.InfoEx("> LocalizeComponent END");
            }
            catch (Exception ex)
            {
                log.InfoEx("> LocalizeComponent Exception: {0}", ex.Message);

                MessageBox.Show(
                    "No Localization.xml found or there is an error during loading. Coordinates Converter window is not fully localized.",
                    "Module Visibility",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void OnDocumentOpenFillDropdowns()
        {
            log.InfoEx("> OnDocumentOpenFillDropdowns START");

            this.SurfaceComboBox.Items.Clear();
            this.TransportLayerComboBox.Items.Clear();
            this.HydroLayerComboBox.Items.Clear();
            this.BuildingsLayerComboBox.Items.Clear();
            this.PlantsLayerComboBox.Items.Clear();

            var mapLayerManager = new MapLayersManager(ArcMap.Document.ActiveView);

            PopulateComboBox(SurfaceComboBox, mapLayerManager.RasterLayers);
            PopulateComboBox(TransportLayerComboBox, mapLayerManager.PolygonLayers);
            PopulateComboBox(HydroLayerComboBox, mapLayerManager.PolygonLayers);
            PopulateComboBox(BuildingsLayerComboBox, mapLayerManager.PolygonLayers);
            PopulateComboBox(PlantsLayerComboBox, mapLayerManager.PolygonLayers);

            log.InfoEx("> OnDocumentOpenFillDropdowns END");
        }

        private static void PopulateComboBox(ComboBox comboBox, IEnumerable<ILayer> layers)
        {
            comboBox.Items.AddRange(layers.Select(l => l.Name).ToArray());
        }

        private Dictionary<ILayer, double> GetAdditionalLayers()
        {
            var selectedLayers = new Dictionary<object, string>();
            if(TransportLayerComboBox.SelectedItem != null)
            {
                selectedLayers.Add(TransportLayerComboBox.SelectedItem, TransportHightTextBox.Text);
            }

            if(BuildingsLayerComboBox.SelectedItem != null)
            {
                selectedLayers.Add(BuildingsLayerComboBox.SelectedItem, BuildingsHight.Text);
            }

            if(PlantsLayerComboBox.SelectedItem != null)
            {
                selectedLayers.Add(PlantsLayerComboBox.SelectedItem, PlantsTextBox.Text);
            }

            if(HydroLayerComboBox.SelectedItem != null)
            {
                selectedLayers.Add(HydroLayerComboBox.SelectedItem, HydroHightTextBox.Text);
            }

            var additionalLayers = new Dictionary<ILayer, double>();

            var mapLayerManager = new MapLayersManager(ArcMap.Document.ActiveView);
            var polygonLayers = mapLayerManager.PolygonLayers.ToArray();

            foreach (var selectedLayer in selectedLayers)
            {
                double layerHeight;

                if(String.IsNullOrEmpty(selectedLayer.Value) || !Double.TryParse(selectedLayer.Value, out double height))
                {
                    layerHeight = 0;
                }
                else
                {
                    layerHeight = height;
                }

                if(selectedLayer.Key != null)
                {
                    additionalLayers.Add(polygonLayers.First(layer => layer.Name == selectedLayer.Key.ToString()),  layerHeight);
                }
            }

            return additionalLayers;
        }

        private List<VisibilityResultInfo> GetVisibilityResultsInfo()
        {
            var info = new List<VisibilityResultInfo>();

            foreach(var session in visibilitySessionsModel)
            {
                info.AddRange(session.VisibilityTask.ResultsInfo);    
            }

            return info;
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
                    var profiles = profilesTreeView.SelectedTreeViewNodes.Where(item => !profilesModels.Any(model => item.NodeProfileSession.SessionId == model.NodeProfileSession.SessionId));

                    foreach(var profile in profiles)
                    {
                        profilesModels.Add(profile);
                    }

                    ProfilesListBox.DataSource = profilesModels;
                    ProfilesListBox.DisplayMember = "Name";
                    ProfilesListBox.ValueMember = "NodeProfileSession";

                    profilesTreeView.Dispose();
                }
            }
            else if (RemoveProfile.Equals(e.Button))
            {
                var selectedItems = new List<TreeViewNodeModel>();

                for(int i = 0; i < ProfilesListBox.SelectedItems.Count; i++)
                {
                    selectedItems.Add(ProfilesListBox.SelectedItems[i] as TreeViewNodeModel);
                }

                foreach(var item in selectedItems)
                {
                    profilesModels.Remove(item);
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
            try
            {
                var profilesSets = new List<ProfileSession>();

                foreach(var profileSetModel in profilesModels)
                {
                    var profilesSet = profileSetModel.NodeProfileSession;
                    profilesSet.ConvertLinesToEsriPolypile(ArcMap.Document.FocusMap.SpatialReference);

                    profilesSets.Add(profilesSet);
                }

                if(SurfaceComboBox.SelectedItem != null && (profilesSets.Count > 0 || visibilitySessionsModel.Count > 0))
                {
                    var arcSceneArguments = Feature3DManager.Get3DFeatures(SurfaceComboBox.SelectedItem.ToString(), profilesSets);


                    if(!String.IsNullOrEmpty(tbZFactor.Text))
                    {
                        arcSceneArguments.ZFactor = Convert.ToDouble(tbZFactor.Text);
                    }

                    arcSceneArguments.AdditionalLayers = GetAdditionalLayers();
                    arcSceneArguments.VisibilityResultsInfo = GetVisibilityResultsInfo();

                    Visualization3DHandler.OpenProfilesSetIn3D(arcSceneArguments);
                }
                else
                {
                    MessageBox.Show(context.LackOfParametersMessage, context.MessageBoxTitle);
                }
            }
            catch (Exception ex)
            {
                log.ErrorEx($"> GenerateButton_Click. Exception: {ex.Message}");
            }
        }

        private void btnRefreshLayers_Click(object sender, EventArgs e)
        {
            OnDocumentOpenFillDropdowns();
        }

        private void SurfaceToolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if(AddSurface.Equals(e.Button))
            {
                var visibilitySessionsWindow = new VisibilitySessionsModalWindow(context);

                if(visibilitySessionsWindow.ShowDialog(this) == DialogResult.OK)
                {
                    var newSessions = visibilitySessionsWindow.SelectedVisibilitySessions.Where(session => !visibilitySessionsModel.Any(model => model.VisibilityTask.Id == session.Id));

                    foreach(var session in newSessions)
                    {
                        visibilitySessionsModel.Add(new VisibilityResultModel
                        {
                            VisibilityTask = session,
                            Gui = $"{session.Name} {session.Created.Value.ToString(Helper.DateFormat)}"
                        });
                    }
                }
            }
            else if(RemoveSurface.Equals(e.Button))
            {
                var selectedItems = new List<VisibilityResultModel>();

                for(int i = 0; i < SessionsListBox.SelectedItems.Count; i++)
                {
                    selectedItems.Add((VisibilityResultModel)SessionsListBox.SelectedItems[i]);
                }

                foreach(var item in selectedItems)
                {
                    visibilitySessionsModel.Remove(item);
                }
            }
        }

        private void TbZFactor_TextChanged(object sender, EventArgs e)
        {
            if(String.IsNullOrEmpty(tbZFactor.Text))
            {
                return;
            }

            if(!Double.TryParse(tbZFactor.Text, out double res) || res < 0)
            {
                var charsCount = tbZFactor.Text.Count();
                tbZFactor.Text =  (charsCount > 1)?  tbZFactor.Text.Remove(tbZFactor.Text.Count() - 1) : string.Empty;
                MessageBox.Show(
                    "Enter a decimal number greater than 0"
                    );
            }
        }

        #endregion

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void HydroHightLabel_Click(object sender, EventArgs e)
        {

        }
    }
}
