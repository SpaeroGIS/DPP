using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using MilSpace.Core;
using MilSpace.Core.Actions;
using MilSpace.Core.Actions.ActionResults;
using MilSpace.Core.Actions.Base;
using MilSpace.Core.Actions.Interfaces;
using MilSpace.Core.Tools.SurfaceProfile.Actions;
using MilSpace.Configurations;
using System.Reflection;
using System.IO;
using System.Linq;

namespace MilSpace.Profile
{
    /// <summary>
    /// Designer class of the dockable window add-in. It contains user interfaces that
    /// make up the dockable window.
    /// </summary>
    public partial class DockableWindowMilSpaceProfileCalc : UserControl
    {
        public DockableWindowMilSpaceProfileCalc(object hook)
        {
            InitializeComponent();

            this.Hook = hook;
            SubscribeForEvents();
        }

        /// <summary>
        /// Host object of the dockable window
        /// </summary>
        private object Hook
        {
            get;
            set;
        }

        protected override void OnLoad(EventArgs e)
        {
            Helper.SetConfiguration();
        }


        private void OnRasterComboDropped()
        {
            //var cmbItems = new List<string>();
            //foreach (var layer in ProfileLayers.RasterLayers)
            //{
            //    cmbItems.Add(layer.Name);
            //}
            //cmbRasterLayers.Items.AddRange(cmbItems.ToArray());
            cmbRasterLayers.Items.Clear();
            PopulateComboBox(cmbRasterLayers, ProfileLayers.RasterLayers);
        }


        private void OnRoadComboDropped()
        {

            cmbRoadLayers.Items.Clear();
            PopulateComboBox(cmbRoadLayers, ProfileLayers.LineLayers);
        }

        private void OnHydrographyDropped()
        {
            cmbHydrographyLayer.Items.Clear();
            PopulateComboBox(cmbHydrographyLayer, ProfileLayers.LineLayers);
        }

        private void OnVegetationDropped()
        {
            cmbPolygonLayer.Items.Clear();
            PopulateComboBox(cmbPolygonLayer, ProfileLayers.PolygonLayers);
        }

        private void OnObservationPointDropped()
        {
            cmbPointLayers.Items.Clear();
            PopulateComboBox(cmbPointLayers, ProfileLayers.PointLayers);
        }


        private static void PopulateComboBox(ComboBox comboBox, IEnumerable<ILayer> layers)
        {
            comboBox.Items.AddRange(layers.Select(l => l.Name).ToArray());
        }

        private void SubscribeForEvents()
        {

            //((IActiveViewEvents_Event) (ArcMap.Document.FocusMap)).ItemAdded += OnRasterComboDropped;
            ArcMap.Events.OpenDocument += OnRasterComboDropped;
            ArcMap.Events.OpenDocument += OnHydrographyDropped;
            ArcMap.Events.OpenDocument += OnObservationPointDropped;
            ArcMap.Events.OpenDocument += OnRoadComboDropped;
            ArcMap.Events.OpenDocument += OnVegetationDropped;

        }

        /// <summary>
        /// Implementation class of the dockable window add-in. It is responsible for 
        /// creating and disposing the user interface class of the dockable window.
        /// </summary>
        public class AddinImpl : ESRI.ArcGIS.Desktop.AddIns.DockableWindow
        {
            private DockableWindowMilSpaceProfileCalc m_windowUI;

            public AddinImpl()
            {
            }

            protected override IntPtr OnCreateChild()
            {
                m_windowUI = new DockableWindowMilSpaceProfileCalc(this.Hook);
                return m_windowUI.Handle;
            }

            protected override void Dispose(bool disposing)
            {
                if (m_windowUI != null)
                    m_windowUI.Dispose(disposing);

                base.Dispose(disposing);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ArcMap.Application.CurrentTool = null;
            UID dockWinID = new UIDClass();
            dockWinID.Value = ThisAddIn.IDs.DockableWindowMilSpaceProfileGraph;
            IDockableWindow dockWindow = ArcMap.DockableWindowManager.GetDockableWindow(dockWinID);
            dockWindow.Show(true);
        }

        private void toolBar1_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            var action = new ActionParam<string>()
            {
                ParamName = ActionParamNamesCore.Action,
                Value = ActionsEnum.bsp.ToString()
            };


            var profileSource = cmbRasterLayers.Text;
            string sdtnow = DateTime.Now.ToString("yyyyMMddHHmmss");
            var resuTable = $"{MilSpaceConfiguration.ConnectionProperty.TemporaryGDBConnection}\\StackProfile{sdtnow}";

            
            var prm = new List<IActionParam>
            {
                action,
//                new ActionParam<string>() { ParamName = ActionParameters.FeatureClass, Value = "E:\\Data\\MilSpace3D\\3DUTM368.gdb\\FCProfiles\\Profile01_L"},
//                new ActionParam<string>() { ParamName = ActionParameters.ProfileSource, Value = "E:\\Data\\MilSpace3D\\3D\\cmr004" },
                new ActionParam<string>() { ParamName = ActionParameters.FeatureClass, Value = "Profile01_L"},
                new ActionParam<string>() { ParamName = ActionParameters.ProfileSource, Value = profileSource },
                new ActionParam<string>() { ParamName = ActionParameters.DataWorkSpace, Value = resuTable},
                new ActionParam<string>() { ParamName = ActionParameters.OutGraphName, Value = ""}
            };


            var procc = new ActionProcessor(prm);
            var res = procc.Process<StringActionResult>();

            MessageBox.Show("Calculated");
        }
    }
}
