using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using MilSpace.Configurations;
using MilSpace.DataAccess.Facade;
using MilSpace.Visualization3D.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                //Type shpWkspFactType = typeof(WorkSpaceFactory);
                //string typeClsID = shpWkspFactType.GUID.ToString("B");

                //string shapeFile = System.IO.Path.GetFileNameWithoutExtension(txtShapeFilePath.Text);
                //string fileFolder = System.IO.Path.GetDirectoryName(txtShapeFilePath.Text);
                //IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)objFactory.Create(typeClsID);
                //IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)workspaceFactory.OpenFromFile(fileFolder, 0); //(@"C:\data\test", 0);

                //Create the layer
                //Type rasterLayerType = typeof(RasterLayerClass);
                //string typeRasterLayerID = rasterLayerType.GUID.ToString("B");

                //IRasterLayer elevationRasterLayer = (IRasterLayer)objFactory.Create(typeRasterLayerID);
                //elevationRasterLayer.CreateFromFilePath(layers.DemLayer);
                //elevationRasterLayer.SpatialReference = layers.SpatialReference;

                //Type basemapLayerType = typeof(BasemapLayerClass);
                //string typeBasemapLayerID = basemapLayerType.GUID.ToString("B");

                //IBasemapLayer basemapLayer = (BasemapLayerClass)objFactory.Create(typeBasemapLayerID);
                //IGroupLayer basemapGroupLayer = basemapLayer as IGroupLayer;
                //basemapGroupLayer.Add(elevationRasterLayer);
                //basemapGroupLayer.Name = "Basemap Content";

                //Type rasterBasemapLayerFactoryType = typeof(RasterBasemapLayerFactory);
                //string typeRasterBaseMapLayerFactoryID = rasterBasemapLayerFactoryType.GUID.ToString("B");

                //IRasterBasemapLayerFactory rasterBMLFactory = (IRasterBasemapLayerFactory)objFactory.Create(typeRasterBaseMapLayerFactoryID);
                //IRasterBasemapLayer rasterBML = rasterBMLFactory.Create(elevationRasterLayer);
                //ILayer layer = (ILayer)rasterBML;
                //layer.Name = layer.Name + " Accelerated";


                var line3DLayer = CreateLayer(layers.Line3DLayer, objFactory);
                //var point3DLayer = CreateLayer(layers.Point3DLayer, objFactory);
                //var polygon3DLayer = CreateLayer(layers.Polygon3DLayer, objFactory);

                //var polygonLayerEffects = (ILayerEffects)polygon3DLayer;
                //polygonLayerEffects.Transparency = 20;

                //Add the layer to document
                IBasicDocument document = (IBasicDocument)m_application.Document;

                //document.AddLayer((ILayer)elevationRasterLayer/*(ILayer)basemapGroupLayer*/);
               // document.AddLayer((ILayer)basemapGroupLayer);
                document.AddLayer(line3DLayer);
                //document.AddLayer(point3DLayer);
                //document.AddLayer(polygon3DLayer);

                document.UpdateContents();
            }
            catch(Exception ex) { } //Or make sure it is a valid shp file first

        }

        internal static void ClosingHandler()
        {
            if(m_appROTEvent != null)
            {
                m_appROTEvent.AppRemoved -= new IAppROTEvents_AppRemovedEventHandler(m_appROTEvent_AppRemoved);
                m_appROTEvent = null;
            }
        }

        private static IFeatureLayer CreateLayer(IFeatureClass featureClass, IObjectFactory objFactory)
        {
            
            var featureLayer = (IFeatureLayer)objFactory.Create("esriCarto.FeatureLayer");
            featureLayer.FeatureClass = featureClass;
            featureLayer.Name = featureLayer.FeatureClass.AliasName;

            return featureLayer;
        }
        private static IFeatureLayer CreateLayer(string featureClass, IObjectFactory objFactory)
        {
            string calcGdb = MilSpaceConfiguration.ConnectionProperty.TemporaryGDBConnection;

            Type factoryType = typeof(FileGDBWorkspaceFactoryClass);
            string typeFactoryID = factoryType.GUID.ToString("B");

            var smth = objFactory.Create(typeFactoryID);
            IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)objFactory.Create(typeFactoryID);
            IFeatureWorkspace calcWorkspace = (IFeatureWorkspace)workspaceFactory.OpenFromFile(calcGdb, 0);


            var featureLayer = (IFeatureLayer)objFactory.Create("esriCarto.FeatureLayer");

            var featureClassC = calcWorkspace.OpenFeatureClass(featureClass);

            IQueryFilter queryFilter = new QueryFilter()
            {
                WhereClause = "OBJECTID > 0"
            };

            var allrecords = featureClassC.Search(queryFilter, true);


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
