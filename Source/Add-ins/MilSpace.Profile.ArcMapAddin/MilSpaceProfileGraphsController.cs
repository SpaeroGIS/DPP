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
            _surfaceProfileChartController.GetSession(profileSession);
            SurfaceProfileChart surfaceProfileChart = _surfaceProfileChartController.CreateProfileChart();

            View.AddNewTab(surfaceProfileChart);
        } 
    }
}
