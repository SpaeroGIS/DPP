using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core.Tools;
using MilSpace.DataAccess;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.Profile.SurfaceProfileChartControl;
using MilSpace.Tools.GraphicsLayer;
using ESRI.ArcGIS.Desktop.AddIns;
using System.Collections.Generic;

namespace MilSpace.Profile
{
    public class MilSpaceProfileGraphsController
    {
        private SurfaceProfileChartController _surfaceProfileChartController;

        private GraphicsLayerManager _graphicsLayerManager;
        private IDockableWindow dockableWindow;



        internal delegate void ProfileRedrawDelegate(GroupedLines profileLines, int sessionId,
                                                                        bool update, List<int> linesIds = null);

        internal delegate void DeleteProfileDelegate(int sessionId, int lineId);
        internal delegate void SelectedProfileChangedDelegate(GroupedLines newSelectedLines, int profileId);
        internal delegate void GetIntersectionLines(ProfileSession profileSession);

        internal event ProfileRedrawDelegate ProfileRedrawn;
        internal event DeleteProfileDelegate ProfileRemoved;
        internal event SelectedProfileChangedDelegate SelectedProfileChanged;
        internal event GetIntersectionLines IntersectionLinesDrawing;

        internal DockableWindowMilSpaceProfileGraph View { get; private set; }

        internal void SetView(DockableWindowMilSpaceProfileGraph view)
        {
            View = view;
        }

        internal MilSpaceProfileGraphsController()
        {
            
        }

        internal GraphicsLayerManager GraphicsLayerManager
        {
            get
            {
                if (_graphicsLayerManager == null)
                {
                    //Take the GraphicsLayerManager fron the Calc controller to use one
                    // TODO: reimplement it as singleton. But check if there is a possibility to work with nore then one ActiveView
                    var winImpl = AddIn.FromID<DockableWindowMilSpaceProfileCalc.AddinImpl>(ThisAddIn.IDs.DockableWindowMilSpaceProfileCalc);
                    _graphicsLayerManager = winImpl.MilSpaceProfileCalsController.GraphicsLayerManager;
                }

                return _graphicsLayerManager;
            }
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

        private void InvokeInvisibleZonesChanged(GroupedLines profileLines, int sessionId, bool update,
                                                                                        List<int> linesIds)
        {
            ProfileRedrawn?.Invoke(profileLines, sessionId, update, linesIds);
        }

       
        private void InvokeProfileRemoved(int sessionId, int lineId)
        {
            ProfileRemoved?.Invoke(sessionId, lineId);
        }

        internal void InvokeSelectedProfileChanged(GroupedLines newSelectedLines, int profileId)
        {
            SelectedProfileChanged?.Invoke(newSelectedLines, profileId);
        }

        internal void InvokeIntersectionLinesDrawing(ProfileSession profileSession)
        {
            IntersectionLinesDrawing?.Invoke(profileSession);
        }

        internal void SetIntersections(List<IntersectionsInLayer> intersectionsLines, int lineId)
        {
            _surfaceProfileChartController.SetIntersectionLines(intersectionsLines, lineId);
        }

        internal void ShowWindow()
        {
            ArcMap.Application.CurrentTool = null;
            if (!IsWindowVisible)
            {
                Docablewindow.Show(true);
            }
        }
        
        internal bool IsWindowVisible => Docablewindow.IsVisible();

        internal void AddSession(ProfileSession profileSession)
        {
            _surfaceProfileChartController = new SurfaceProfileChartController();

            _surfaceProfileChartController.OnProfileGraphClicked += OnProfileGraphClicked;
            _surfaceProfileChartController.InvisibleZonesChanged += InvokeInvisibleZonesChanged;
            _surfaceProfileChartController.ProfileRemoved += InvokeProfileRemoved;
            _surfaceProfileChartController.SelectedProfileChanged += InvokeSelectedProfileChanged;
            _surfaceProfileChartController.IntersectionLinesDrawing += InvokeIntersectionLinesDrawing;

            _surfaceProfileChartController.SetSession(profileSession);
            SurfaceProfileChart surfaceProfileChart = _surfaceProfileChartController.CreateProfileChart(profileSession.ObserverHeight);

            View.AddNewTab(surfaceProfileChart, profileSession.SessionId);
        }

        internal void SetChart(SurfaceProfileChart currentChart)
        {
            _surfaceProfileChartController.SetCurrentChart(currentChart, this);
        }

        internal void RemoveTab()
        {
            View.RemoveCurrentTab();
        }

        internal void RemoveTab(int sessionId)
        {
            View.RemoveTabBySessionId(sessionId);
        }

        private IDockableWindow Docablewindow
        {
            get
            {
                if (dockableWindow == null)
                {
                    UID dockWinID = new UIDClass();
                    dockWinID.Value = ThisAddIn.IDs.DockableWindowMilSpaceProfileGraph;
                    dockableWindow = ArcMap.DockableWindowManager.GetDockableWindow(dockWinID);
                }

                return dockableWindow;
            }
        }
}
}
