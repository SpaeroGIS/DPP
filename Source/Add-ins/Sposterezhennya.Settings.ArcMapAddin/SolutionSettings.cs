using ESRI.ArcGIS.Carto;
using MilSpace.Core.ArcMap;
using MilSpace.Core.DataAccess;
using MilSpace.Core.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MilSpace.Settings
{
    public partial class SolutionSettingsForm : Form
    {
        private string _rasterLayer => (cmbDEMLayer.SelectedItem == null)? string.Empty : cmbDEMLayer.SelectedItem.ToString();
        private SolutionSettingsController _controller;
        private bool _changeAllLayers = true;
        private bool _rasterWasChanges = false;

        public SolutionSettingsForm()
        {
            InitializeComponent();
            LocalizeStrings();
            PopulateComboBox();

            _controller = new SolutionSettingsController(this);
            FillSessionInfo();
        }

        public void SetNewRasterLayer(string rasterLayer)
        {
            if (String.IsNullOrEmpty(_rasterLayer) && cmbDEMLayer.Items.Contains(rasterLayer))
            {
                cmbDEMLayer.SelectedItem = rasterLayer;
                ConnectToMap();
            }
        }


        private void SubscribeOnEsriEvents()
        {
            IActiveViewEvents_Event activeViewEvent = (IActiveViewEvents_Event)ArcMapInstance.Document.ActiveView;
            activeViewEvent.ItemAdded += OnItemsChanged;
            activeViewEvent.ItemDeleted += OnItemsChanged;
            ArcMapInstance.Events.OpenDocument += UpdateDEMLayerComboBox;
            ArcMapInstance.Events.NewDocument += UpdateDEMLayerComboBox;
        }

        private void LocalizeStrings()
        {
            mainTabControl.TabPages["tbSurface"].Text = LocalizationContext.Instance.FindLocalizedElement("SolutionSettingsWindow_tabSurfaceCaption", "поверхня");
            mainTabControl.TabPages["tbGraphics"].Text = LocalizationContext.Instance.FindLocalizedElement("SolutionSettingsWindow_tabGraphicsCaption", "графіка");
            mainTabControl.TabPages["tbConfiguration"].Text = LocalizationContext.Instance.FindLocalizedElement("SolutionSettingsWindow_tabConfigurationCaption", "конфігурація (сеанс)");

            Text = LocalizationContext.Instance.FindLocalizedElement("SolutionSettingsWindow_FormCaption", "Спостереження. Налаштування");

            lblDEM.Text = LocalizationContext.Instance.FindLocalizedElement("SolutionSettingsWindow_lblDEMText", "Вибір поверхні (DEM) для розрахунків");
            lblSurfaceInfo.Text = LocalizationContext.Instance.FindLocalizedElement("SolutionSettingsWindow_lblSurfaceInfoText", "Інформація про поверхню");
            lblShowGraphics.Text = LocalizationContext.Instance.FindLocalizedElement("SolutionSettingsWindow_lblShowGraphicsText", "(2) відобразити графіку");
            lblClearGraphics.Text = LocalizationContext.Instance.FindLocalizedElement("SolutionSettingsWindow_lblClearGraphicsText", "(1) очистити графіку");
            lblSeanseInfo.Text = LocalizationContext.Instance.FindLocalizedElement("SolutionSettingsWindow_lblSeanseInfoText", "Відомості про сеанс роботи");

            btnApply.Text = LocalizationContext.Instance.FindLocalizedElement("SolutionSettingsWindow_btnApplyText", "Застосувати");
            btnExit.Text = LocalizationContext.Instance.FindLocalizedElement("SolutionSettingsWindow_btnExitText", "Вийти");
            btnConnectToMap.Text = LocalizationContext.Instance.FindLocalizedElement("SolutionSettingsWindow_btnConnectToMapText", "Приєднати до карти");

            PopulateCheckListBox();
        }

        private void PopulateCheckListBox()
        {
            chckListBoxClearGraphics.Items.Clear();
            chckListBoxClearGraphics.Items.AddRange(LocalizationContext.Instance.ClearGraphicsLocalisation.Values.ToArray());

            chckListBoxShowGraphics.Items.Clear();
            chckListBoxShowGraphics.Items.AddRange(LocalizationContext.Instance.ShowGraphicsLocalisation.Values.ToArray());
        }

        private void PopulateComboBox()
        {
            MapLayersManager mapLayersManager = new MapLayersManager(ArcMapInstance.Document.ActiveView);
            cmbDEMLayer.Items.Clear();
            cmbDEMLayer.Items.AddRange(mapLayersManager.RasterLayers.Select(i => i.Name).ToArray());
        }
        
        private void OnItemsChanged(object item)
        {
            UpdateDEMLayerComboBox();
        }

        private void UpdateDEMLayerComboBox()
        {
            _changeAllLayers = false;

            var selectedLayer = _rasterLayer;
            PopulateComboBox();
            
            if(cmbDEMLayer.Items.Contains(selectedLayer))
            {
                cmbDEMLayer.SelectedItem = selectedLayer;
            }
        }

        private void BtnConnectToMap_Click(object sender, EventArgs e)
        {
           
        }

        private void ConnectToMap()
        {
            if (_changeAllLayers && _controller != null)
            {
                _controller.ChangeRasterLayer(_rasterLayer);
            }
        }

        private void ClearCheckBoxes()
        {
            for (int i = 0; i < chckListBoxClearGraphics.Items.Count; i++)
            {
                chckListBoxClearGraphics.SetItemCheckState(i, CheckState.Unchecked);
            }

            for (int i = 0; i < chckListBoxShowGraphics.Items.Count; i++)
            {
                chckListBoxShowGraphics.SetItemCheckState(i, CheckState.Unchecked);
            }
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            // Set DEM layer
            if (_rasterWasChanges)
            {
                ConnectToMap();
                _rasterWasChanges = false;
            }

            // Clear graphics
            var selectedGraphicsToClear = new List<GraphicsTypesEnum>();

            foreach (var item in chckListBoxClearGraphics.CheckedItems)
            {
                selectedGraphicsToClear.Add(_controller.GetClearGraphicsTypeByString(item.ToString()));
            }

            _controller.ClearSelectedGraphics(selectedGraphicsToClear);

            // Show graphics

            var selectedGraphicsToShow = new List<GraphicsTypesEnum>();

            foreach (var item in chckListBoxShowGraphics.CheckedItems)
            {
                selectedGraphicsToShow.Add(_controller.GetShowGraphicsTypeByString(item.ToString()));
            }

            _controller.ShowSelectedGraphics(selectedGraphicsToShow);

            // Update
            ClearCheckBoxes();
            btnApply.Enabled = false;
        }

        private void CmbDEMLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            _rasterWasChanges = true;

            if (!btnApply.Enabled)
            {
                btnApply.Enabled = true;
            }

            FillRasterInfo();
        }

        private void FillRasterInfo()
        {
            var rasterInfo = _controller.GetRasterInfo(_rasterLayer);
            lbRasterInfo.Items.Clear();

            if (rasterInfo != null)
            {
                lbRasterInfo.Items.AddRange(rasterInfo.ToArray());
            }
        }

        private void FillSessionInfo()
        {
            lvConfiguration.View = View.Details;
            lvConfiguration.Columns.Clear();

            lvConfiguration.Columns.Add("Attribute", -1);
            lvConfiguration.Columns.Add("Value", -1);

            lvConfiguration.HeaderStyle = ColumnHeaderStyle.None;

            var sessionInfo = _controller.GetSessionInfo();
            lvConfiguration.Items.Clear();

            foreach (var info in sessionInfo)
            {
                var newItem = new ListViewItem(info.Key);
                newItem.SubItems.Add(info.Value);

                lvConfiguration.Items.Add(newItem);
            }
        }

        private void SolutionSettingsForm_Load(object sender, EventArgs e)
        {
            cmbDEMLayer.SelectedItem = SettingsManager.RasterLayer;
        }

        private void ChckListBoxClearGraphics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((chckListBoxClearGraphics.SelectedIndices.Count > 0 || chckListBoxShowGraphics.SelectedIndices.Count > 0)
                && !btnApply.Enabled)
            {
                btnApply.Enabled = true;
            }
        }
    }
}
