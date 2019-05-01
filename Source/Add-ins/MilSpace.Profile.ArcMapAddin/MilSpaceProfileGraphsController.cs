using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MilSpace.Tools.GraphicsLayer;
using MilSpace.Profile.SurfaceProfileChartControl;
using MilSpace.DataAccess.DataTransfer;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Desktop.AddIns;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core.Tools;

namespace MilSpace.Profile
{
    public class MilSpaceProfileGraphsController
    {
        private SurfaceProfileChartController _surfaceProfileChartController;

        internal DockableWindowMilSpaceProfileGraph View { get; private set; }

        internal void SetView(DockableWindowMilSpaceProfileGraph view)
        {
            View = view;
        }

        internal MilSpaceProfileGraphsController()
        {
            _surfaceProfileChartController = new SurfaceProfileChartController();
            _surfaceProfileChartController.OnProfileGraphClicked += OnProfileGraphClicked;
        }

        private void OnProfileGraphClicked(GraphProfileClickedArgs e)
        {

            IPoint point = new Point() { X = e.ProfilePoint.X, Y = e.ProfilePoint.Y, SpatialReference = e.ProfilePoint.SpatialReference };

            IEnvelope env = new EnvelopeClass();

            var av = ArcMap.Document.ActivatedView;
            point.Project(av.FocusMap.SpatialReference);

            env = av.Extent;
            env.CenterAt(point);
            av.Extent = env;
            av.Refresh();
            EsriTools.FlashGeometry(av.ScreenDisplay, point);
            av.Refresh();
        }

        internal void ShowWindow()
        {
            ArcMap.Application.CurrentTool = null;
            UID dockWinID = new UIDClass();
            dockWinID.Value = ThisAddIn.IDs.DockableWindowMilSpaceProfileGraph;
            IDockableWindow dockWindow = ArcMap.DockableWindowManager.GetDockableWindow(dockWinID);
            dockWindow.Show(true);
        }

        internal void AddSession(ProfileSession profileSession)
        {
            _surfaceProfileChartController.SetSession(profileSession);
            SurfaceProfileChart surfaceProfileChart = _surfaceProfileChartController.CreateProfileChart();

            View.AddNewTab(surfaceProfileChart, profileSession.SessionName);
        }

        internal void SetChart(SurfaceProfileChart currentChart)
        {
            _surfaceProfileChartController.SetCurrentChart(currentChart, this);
        }

        internal void RemoveTab()
        {
            View.RemoveCurrentTab();
        }
    }
}
