using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using MilSpace.Configurations;
using MilSpace.Visualization3D.Models;
using System;

namespace MilSpace.Visualization3D
{
    public static class Visualization3DHandler
    {
        private static IApplication m_application;

        //Application removed event
        private static IAppROTEvents_Event m_appROTEvent;
        private static int m_appHWnd = 0;

        static Visualization3DHandler()
        {

        }

        internal static void OpenProfilesSetIn3D(ArcSceneArguments layers)
        {
            OpenArcScene();

            try
            {
                IObjectFactory objFactory = m_application as IObjectFactory;

                Type rasterLayerType = typeof(RasterLayerClass);
                string typeRasterLayerID = rasterLayerType.GUID.ToString("B");

                IRasterLayer elevationRasterLayer = (IRasterLayer)objFactory.Create(typeRasterLayerID);
                elevationRasterLayer.CreateFromFilePath(layers.DemLayer);
                ILayer layer = (ILayer)elevationRasterLayer;

                var line3DLayer = CreateLayer(layers.Line3DLayer, objFactory);
                var point3DLayer = CreateLayer(layers.Point3DLayer, objFactory);
                var polygon3DLayer = CreateLayer(layers.Polygon3DLayer, objFactory);

                var polygonLayerEffects = (ILayerEffects)polygon3DLayer;
                polygonLayerEffects.Transparency = 50;

                IBasicDocument document = (IBasicDocument)m_application.Document;

                document.AddLayer(layer);
                document.AddLayer(line3DLayer);
                document.AddLayer(point3DLayer);
                document.AddLayer(polygon3DLayer);

                document.UpdateContents();
            }
            catch(Exception ex) { }

        }

        internal static void ClosingHandler()
        {
            if(m_appROTEvent != null)
            {
                m_appROTEvent.AppRemoved -= new IAppROTEvents_AppRemovedEventHandler(m_appROTEvent_AppRemoved);
                m_appROTEvent = null;
            }
        }

        private static IFeatureLayer CreateLayer(string featureClass, IObjectFactory objFactory)
        {
            string calcGdb = MilSpaceConfiguration.ConnectionProperty.TemporaryGDBConnection;

            Type factoryType = Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory");
            string typeFactoryID = factoryType.GUID.ToString("B");

            IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)objFactory.Create(typeFactoryID);
            IFeatureWorkspace calcWorkspace = (IFeatureWorkspace)workspaceFactory.OpenFromFile(calcGdb, 0);

            var featureLayer = (IFeatureLayer)objFactory.Create("esriCarto.FeatureLayer");
            var featureClassC = calcWorkspace.OpenFeatureClass(featureClass);

            featureLayer.FeatureClass = featureClassC;
            featureLayer.Name = featureLayer.FeatureClass.AliasName;

            return featureLayer;
        }

        private static bool OpenArcScene()
        {
            IDocument doc = null;
            try
            {
                doc = new ESRI.ArcGIS.ArcScene.SxDocument();
            }
            catch
            {
                return false;
            }

            if(doc != null)
            {
                m_appROTEvent = new AppROTClass();
                m_appROTEvent.AppRemoved += new IAppROTEvents_AppRemovedEventHandler(m_appROTEvent_AppRemoved);

                m_application = doc.Parent;
                m_application.Visible = true;
                m_appHWnd = m_application.hWnd;
            }
            else
            {
                m_appROTEvent = null;
                m_application = null;

                return false;
            }

            return true;
        }

        #region "Handle the case when the application is shutdown by user manually"

        static void m_appROTEvent_AppRemoved(AppRef pApp)
        {
            //Application manually shuts down. Stop listening
            if(pApp.hWnd == m_appHWnd)
            {
                m_appROTEvent.AppRemoved -= new IAppROTEvents_AppRemovedEventHandler(m_appROTEvent_AppRemoved);
                m_appROTEvent = null;
                m_application = null;
                m_appHWnd = 0;
            }
        }
        #endregion
    }

}
