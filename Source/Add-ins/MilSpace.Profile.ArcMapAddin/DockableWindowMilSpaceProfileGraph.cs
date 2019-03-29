using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ADF.CATIDs;
using MilSpace.Profile.SurfaceProfileChartControl;
using System.Linq;

namespace MilSpace.Profile
{
    [Guid("80eb5b70-d4ba-476a-a107-49e96cf1b38d")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("MilSpace.Profile.DockableWindowMilSpaceProfileGraph")]
    public partial class DockableWindowMilSpaceProfileGraph : UserControl
    {
        private MilSpaceProfileGraphsController controller;

        private object Hook
        {
            get;
            set;
        }

        public DockableWindowMilSpaceProfileGraph Instance { get; }

        //private IApplication m_application;

        //#region COM Registration Function(s)
        //[ComRegisterFunction()]
        //[ComVisible(false)]
        //static void RegisterFunction(Type registerType)
        //{
        //    // Required for ArcGIS Component Category Registrar support
        //    ArcGISCategoryRegistration(registerType);
        //    //
        //    // TODO: Add any COM registration code here
        //    //
        //}

        //[ComUnregisterFunction()]
        //[ComVisible(false)]
        //static void UnregisterFunction(Type registerType)
        //{
        //    // Required for ArcGIS Component Category Registrar support
        //    ArcGISCategoryUnregistration(registerType);

        //    //
        //    // TODO: Add any COM unregistration code here
        //    //
        //}

        //#region ArcGIS Component Category Registrar generated code
        ///// <summary>
        ///// Required method for ArcGIS Component Category registration -
        ///// Do not modify the contents of this method with the code editor.
        ///// </summary>
        //private static void ArcGISCategoryRegistration(Type registerType)
        //{
        //    string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
        //    MxDockableWindows.Register(regKey);

        //}
        ///// <summary>
        ///// Required method for ArcGIS Component Category unregistration -
        ///// Do not modify the contents of this method with the code editor.
        ///// </summary>
        //private static void ArcGISCategoryUnregistration(Type registerType)
        //{
        //    string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
        //    MxDockableWindows.Unregister(regKey);

        //}

        //#endregion
        //#endregion

        public DockableWindowMilSpaceProfileGraph(MilSpaceProfileGraphsController controller)
        {
            this.Instance = this;
            SetController(controller);
            controller.SetView(this);
        }

        public DockableWindowMilSpaceProfileGraph(object hook, MilSpaceProfileGraphsController controller)
        {
            InitializeComponent();

            profilesTabControl.TabPages.Clear();
            string title = $"Профиль графика {profilesTabControl.TabCount + 1}";
            TabPage tabPage = new TabPage(title);
            tabPage.Name = $"profileTabPage{profilesTabControl.TabCount}";
            profilesTabControl.TabPages.Add(tabPage);

            SetController(controller);
            controller.SetView(this);

            this.Hook = hook;
            this.Instance = this;
            SubscribeForEvents();
        }

        internal void AddNewTab(SurfaceProfileChart surfaceProfileChart)
        {
            string title = $"Профиль графика {profilesTabControl.TabCount + 1}";
            TabPage tabPage = new TabPage(title);
            tabPage.Name = $"profileTabPage{profilesTabControl.TabCount}";
            profilesTabControl.TabPages.Add(tabPage);

            profilesTabControl.TabPages[profilesTabControl.TabCount-2].Controls.Add(surfaceProfileChart);
            profilesTabControl.TabPages[profilesTabControl.TabCount - 2].Show();
        }

        #region AddIn Instance

        public void SetController(MilSpaceProfileGraphsController controller)
        {
            this.controller = controller;
        }

        private void SubscribeForEvents()
        {

        }

        public class AddinImpl : ESRI.ArcGIS.Desktop.AddIns.DockableWindow
        {
            private DockableWindowMilSpaceProfileGraph m_windowUI;
            private MilSpaceProfileGraphsController controller;

            public AddinImpl()
            {
            }

            protected override IntPtr OnCreateChild()
            {
                controller = new MilSpaceProfileGraphsController();

                m_windowUI = new DockableWindowMilSpaceProfileGraph(this.Hook, controller);


                return m_windowUI.Handle;
            }

            protected override void Dispose(bool disposing)
            {
                if (m_windowUI != null)
                    m_windowUI.Dispose(disposing);

                base.Dispose(disposing);
            }

            internal DockableWindowMilSpaceProfileGraph DockableWindowUI => m_windowUI;


            internal MilSpaceProfileGraphsController MilSpaceProfileCalsController => controller;

        }
        #endregion
    }
}
