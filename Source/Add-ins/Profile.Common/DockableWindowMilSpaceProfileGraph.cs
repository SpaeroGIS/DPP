using ESRI.ArcGIS.Carto;
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
        private int selectedTabIndex = -1;

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

        internal bool CheckTabExistance(string sessionName)
        {
            TabPage tabPage = null;
            return CheckTabExistance(sessionName, out tabPage);
        }

        private bool CheckTabExistance(string sessionName, out TabPage foundTab)
        {
            foundTab = null;
            foreach (TabPage tab in profilesTabControl.TabPages)
            {
                if (tab.Tag.ToString() == sessionName)
                {
                    foundTab = tab;
                    return true;
                }
            }

            return false;
        }

        internal void AddNewTab(SurfaceProfileChart surfaceProfileChart, int sessionId)
        {
            if (profilesTabControl.TabPages.Count > 0)
            {
                var chart = (SurfaceProfileChart)profilesTabControl.SelectedTab.Controls["profileChart"];
                controller.ClearProfileSelection(chart);
            }

            TabPage tabPage = null;

            if (CheckTabExistance(sessionId.ToString(), out tabPage))
            {
                profilesTabControl.SelectedTab = tabPage;
                return;
            }

            string sHeaderTab = $"Ãðàô³ê";
            string title = sHeaderTab + $" {profilesTabControl.TabPages.Count + 1}";
            int i = 1;

            while (profilesTabControl.TabPages.ContainsKey(title))
            {
                title = sHeaderTab + $" {i + 1}";
                i++;
            }

            tabPage = new TabPage(title);
            tabPage.Name = title;
            tabPage.Tag = sessionId;
            tabPage.Padding = new Padding(0);

            profilesTabControl.TabPages.Add(tabPage);

            var curTab = profilesTabControl.TabPages[profilesTabControl.TabCount - 1];

            surfaceProfileChart.Width = curTab.Width;
            surfaceProfileChart.Height = curTab.Height;
            surfaceProfileChart.Name = "profileChart";
            curTab.Controls.Add(surfaceProfileChart);

            profilesTabControl.SelectTab(profilesTabControl.TabCount - 1);
            controller.SetChart(surfaceProfileChart);

            surfaceProfileChart.SetControlSize();
        }

        internal void RemoveCurrentTab()
        {
            profilesTabControl.TabPages.RemoveAt(profilesTabControl.SelectedIndex);
        }

        internal void RemoveTabBySessionId(int sessionId)
        {
            TabPage tabPage = null;

            foreach (TabPage page in profilesTabControl.TabPages)
            {
                if ((int)page.Tag == sessionId)
                {
                    tabPage = page;
                    break;
                }
            }

            if (tabPage != null)
            {
                profilesTabControl.TabPages.Remove(tabPage);
            }
        }

        internal void SetCurrentChart()
        {
            controller.SetChart((SurfaceProfileChart)profilesTabControl.SelectedTab.Controls["profileChart"]);
        }

        #region AddIn Instance

        public void SetController(MilSpaceProfileGraphsController controller)
        {
            this.controller = controller;
        }

        public IActiveView ActiveView => ArcMap.Document.ActiveView;

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


            internal MilSpaceProfileGraphsController MilSpaceProfileGraphsController => controller;

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
                page.Controls["profileChart"].Width = profilesTabControl.Width - 10;
                page.Controls["profileChart"].Height = profilesTabControl.Height - 30;
            }
        }

        private void ProfilesTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectedTabIndex != -1 && profilesTabControl.TabPages.Count > selectedTabIndex)
            {
                var chart = (SurfaceProfileChart)profilesTabControl.TabPages[selectedTabIndex].Controls["profileChart"];
                controller.ClearProfileSelection(chart);
            }

            if (profilesTabControl.TabPages.Count > 0)
            {
                SetCurrentChart();
                selectedTabIndex = profilesTabControl.SelectedIndex;
            }
        }
    }
}
