﻿using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core.Tools;
using ESRI.ArcGIS.Display;
using MilSpace.Core.Tools.Helper;
using MilSpace.DataAccess;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.Profile.SurfaceProfileChartControl;
using MilSpace.Tools.GraphicsLayer;
using ESRI.ArcGIS.Desktop.AddIns;

namespace MilSpace.Profile
{
    public class MilSpaceProfileGraphsController
    {
        private SurfaceProfileChartController _surfaceProfileChartController;

        private GraphicsLayerManager _graphicsLayerManager;
        private IDockableWindow dockableWindow;

        internal DockableWindowMilSpaceProfileGraph View { get; private set; }

        internal void SetView(DockableWindowMilSpaceProfileGraph view)
        {
            View = view;
        }

        internal MilSpaceProfileGraphsController()
        {
            _surfaceProfileChartController = new SurfaceProfileChartController();
            _surfaceProfileChartController.OnProfileGraphClicked += OnProfileGraphClicked;
            _surfaceProfileChartController.InvisibleZonesChanged += InvisibleZonesChanged;
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

        private void InvisibleZonesChanged(GroupedLines profileLines, RgbColor rgbVisibleColor,
                                                RgbColor rgbInvisibleColor, int sessionId, bool update, int profilesCount)
        {
            if (update)
            {
                GraphicsLayerManager
                        .UpdateGraphicLine(Converter.ConvertLinesToEsriPolypile(profileLines.Lines, ArcMap.Document
                                                                                       .FocusMap
                                                                                       .SpatialReference),
                                                        sessionId, profileLines, rgbVisibleColor, rgbInvisibleColor);
            }
            else
            {
                if (profileLines.LineId == 1)
                {
                    GraphicsLayerManager.RemoveGraphic(sessionId, profilesCount);
                }

                GraphicsLayerManager
                    .AddLinesToWorkingGraphics(Converter.ConvertLinesToEsriPolypile(profileLines.Lines, ArcMap.Document
                                                                                      .FocusMap
                                                                                      .SpatialReference),
                                           sessionId, profileLines, rgbVisibleColor, rgbInvisibleColor);
            }
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

            _surfaceProfileChartController.SetSession(profileSession);
            SurfaceProfileChart surfaceProfileChart = _surfaceProfileChartController.CreateProfileChart(profileSession.ObserverHeight);

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
