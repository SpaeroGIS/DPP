using MilSpace.Profile.SurfaceProfileChartControl;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

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

            SetController(controller);
            controller.SetView(this);

            this.Hook = hook;
            this.Instance = this;
            SubscribeForEvents();
        }

        internal void AddNewTab(SurfaceProfileChart surfaceProfileChart, string sessionName)
        {
            string title = $"Graph {profilesTabControl.TabPages.Count + 1}";
            TabPage tabPage = new TabPage(title);
            tabPage.Name = $"Graph {profilesTabControl.TabPages.Count + 1}";
            profilesTabControl.TabPages.Add(tabPage);

            var curTab = profilesTabControl.TabPages[profilesTabControl.TabCount - 1];
            surfaceProfileChart.Width = curTab.Width;
            surfaceProfileChart.Height = curTab.Height;
            surfaceProfileChart.Name = "profileChart";
            curTab.Controls.Add(surfaceProfileChart);
            curTab.Show();
            curTab.Select();
            surfaceProfileChart.SetControlSize();
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

        private void DockableWindowMilSpaceProfileGraph_Resize(object sender, EventArgs e)
        {
            profilesTabControl.Width = this.Width;
            profilesTabControl.Height = this.Height;
        }

        private void ProfilesTabControl_Resize(object sender, EventArgs e)
        {
           foreach (TabPage page in profilesTabControl.TabPages)
            {
                page.Controls["profileChart"].Width = profilesTabControl.Width;
                page.Controls["profileChart"].Height = profilesTabControl.Height - 30;
            }
        }

        private void ProfilesTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            controller.ChangeChart((SurfaceProfileChart)profilesTabControl.SelectedTab.Controls["profileChart"]);
        }
    }
}
