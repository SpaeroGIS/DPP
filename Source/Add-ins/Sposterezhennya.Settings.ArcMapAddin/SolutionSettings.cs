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

        public SolutionSettingsForm()
        {
            InitializeComponent();
            LocalizeStrings();
            PopulateComboBox();

            _controller = new SolutionSettingsController(this);
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
            mainTabControl.TabPages["tbGraphics"].Text = LocalizationContext.Instance.FindLocalizedElement("SolutionSettingsWindow_tabSurfaceCaption", "поверхня");
            mainTabControl.TabPages["tbConfiguration"].Text = LocalizationContext.Instance.FindLocalizedElement("SolutionSettingsWindow_tabGraphicsCaption", "графіка");
            mainTabControl.TabPages["tbSurface"].Text = LocalizationContext.Instance.FindLocalizedElement("SolutionSettingsWindow_tabConfigurationCaption", "конфігурація (сеанс)");

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
            ConnectToMap();
        }

        private void ConnectToMap()
        {
            if (_changeAllLayers && _controller != null)
            {
                _controller.ChangeRasterLayer(_rasterLayer);
            }
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            var selectedGraphicsToClear = new List<GraphicsTypesEnum>();

            foreach (var item in chckListBoxClearGraphics.CheckedItems)
            {
                selectedGraphicsToClear.Add(_controller.GetClearGraphicsTypeByString(item.ToString()));
            }
        }
    }
}
