using ESRI.ArcGIS.Analyst3D;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using MilSpace.Configurations;
using MilSpace.Visualization3D.Models;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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

                var elevationRasterLayer = (IRasterLayer)objFactory.Create(typeRasterLayerID);
                elevationRasterLayer.CreateFromFilePath(layers.DemLayer);
                var layer = (ILayer)elevationRasterLayer;

                var surface = (IRasterSurface)objFactory.Create("esrianalyst3d.RasterSurface");
                surface.PutRaster(elevationRasterLayer.Raster, 0);
                var functionalSurface = (IFunctionalSurface)surface;

                var line3DLayer = CreateLayer(layers.Line3DLayer, objFactory);
                var point3DLayer = CreateLayer(layers.Point3DLayer, objFactory);
                var polygon3DLayer = CreateLayer(layers.Polygon3DLayer, objFactory);

                var polygonLayerEffects = (ILayerEffects)polygon3DLayer;
                polygonLayerEffects.Transparency = 50;

                var document = (IBasicDocument)m_application.Document;

                SetSurface3DProperties(layer, objFactory, functionalSurface);
                SetLine3DProperties(line3DLayer, objFactory, functionalSurface);
                SetFeatures3DProperties(point3DLayer, objFactory);
                SetFeatures3DProperties(polygon3DLayer, objFactory);


                document.AddLayer(layer);

                document.AddLayer(line3DLayer);
                document.AddLayer(point3DLayer);
                document.AddLayer(polygon3DLayer);

                foreach(var extraLayer in layers.AdditionalLayers)
                {
                    var featureLayer = CreateLayerCopy((IFeatureLayer)extraLayer, objFactory);

                    document.AddLayer(featureLayer);
                }

                document.UpdateContents();

                document.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography,
                                                   VisibilityColorsRender(line3DLayer, objFactory), document.ActiveView.Extent);

                document.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography,
                                                   VisibilityColorsRender(polygon3DLayer, objFactory), document.ActiveView.Extent);

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


        public static string GetWorkspacePathForLayer(ILayer layer)
        {
            if(layer == null || !(layer is IDataset))
            {
                return null;
            }

            IDataset dataset = (IDataset)(layer);

            return dataset.Workspace.PathName;
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

        private static IGeoFeatureLayer CreateLayerCopy(IFeatureLayer layer, IObjectFactory objFactory)
        {
            var workspacePath = GetWorkspacePathForLayer(layer);

            Type factoryType = Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory");
            string typeFactoryID = factoryType.GUID.ToString("B");

            IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)objFactory.Create(typeFactoryID);
            IFeatureWorkspace calcWorkspace = (IFeatureWorkspace)workspaceFactory.OpenFromFile(workspacePath, 0);

            var featureLayer = (IFeatureLayer)objFactory.Create("esriCarto.FeatureLayer");
            var featureClassC = calcWorkspace.OpenFeatureClass(layer.FeatureClass.AliasName);

            featureLayer.FeatureClass = featureClassC;
            featureLayer.Name = layer.Name;

            var arcMapLayerDefinition = layer as IFeatureLayerDefinition2;
            var layerDefinition = featureLayer as IFeatureLayerDefinition2;
            layerDefinition.DefinitionExpression = arcMapLayerDefinition.DefinitionExpression;

            IGeoFeatureLayer geoArcMapLayer = layer as IGeoFeatureLayer;
            IGeoFeatureLayer geoFL = featureLayer as IGeoFeatureLayer;
            geoFL.Renderer = geoArcMapLayer.Renderer;
            
            return geoFL;
        }
        
        private static IGeoFeatureLayer VisibilityColorsRender(IFeatureLayer layer, IObjectFactory objFactory)
        {
            const string fieldName = "IS_VISIBLE";
            string[] uniqueValues = new string[2];

            int fieldIndex = layer.FeatureClass.Fields.FindField(fieldName);

            uniqueValues[0] = "0";
            uniqueValues[1] = "1";

            Type renderType = typeof(UniqueValueRendererClass);
            string typeRenderID = renderType.GUID.ToString("B");

            IUniqueValueRenderer uVRenderer = (IUniqueValueRenderer)objFactory.Create(typeRenderID);
            uVRenderer.FieldCount = 1;
            uVRenderer.Field[0] = fieldName;

            var invisibleSymbol = GetSymbol(layer.FeatureClass.ShapeType, new RgbColor() { Red = 255, Blue = 0, Green = 0 });
            var visibleSymbol = GetSymbol(layer.FeatureClass.ShapeType, new RgbColor() { Red = 0, Blue = 0, Green = 255 });

            uVRenderer.AddValue(uniqueValues[0], "Visibility", invisibleSymbol);
            uVRenderer.AddValue(uniqueValues[1], "Visibility", visibleSymbol);

            IGeoFeatureLayer geoFL = layer as IGeoFeatureLayer;
            geoFL.Renderer = uVRenderer as IFeatureRenderer;

            return geoFL;

        }

        private static void SetLine3DProperties(IFeatureLayer layer, IObjectFactory objFactory, IFunctionalSurface surface)
        {
            var properties3D = (I3DProperties)objFactory.Create("esrianalyst3d.Feature3DProperties");
            properties3D.BaseOption = esriBaseOption.esriBaseSurface;
            properties3D.BaseSurface = surface;
            properties3D.ZFactor = 7;
            properties3D.OffsetExpressionString = "200";

            ILayerExtensions layerExtensions = (ILayerExtensions)layer;
            layerExtensions.AddExtension(properties3D);
            properties3D.Apply3DProperties(layer);
        }

        private static void SetFeatures3DProperties(IFeatureLayer layer, IObjectFactory objFactory)
        {
            var properties3D = (I3DProperties)objFactory.Create("esrianalyst3d.Feature3DProperties");
            properties3D.ZFactor = 7;
            properties3D.OffsetExpressionString = "200";

            ILayerExtensions layerExtensions = (ILayerExtensions)layer;
            layerExtensions.AddExtension(properties3D);
            properties3D.Apply3DProperties(layer);
        }

        private static void SetSurface3DProperties(ILayer layer, IObjectFactory objFactory, IFunctionalSurface surface)
        {
            var properties3D = (I3DProperties)objFactory.Create("esrianalyst3d.Raster3DProperties");
            properties3D.BaseOption = esriBaseOption.esriBaseSurface;
            properties3D.BaseSurface = surface;
            properties3D.ZFactor = 7;

            ILayerExtensions layerExtensions = (ILayerExtensions)layer;
            layerExtensions.AddExtension(properties3D);
            properties3D.Apply3DProperties(layer);
        }

        private static ISymbol GetSymbol(esriGeometryType featureGeometryType, RgbColor color)
        {
            if(featureGeometryType == esriGeometryType.esriGeometryPolygon)
            {
                ISimpleFillSymbol simplePolygonSymbol = new SimpleFillSymbolClass();
                simplePolygonSymbol.Color = color;

                ISimpleLineSymbol outlineSymbol = new SimpleLineSymbolClass();
                var outLine = color;
                color.Transparency = 255;
                outlineSymbol.Color = outLine;
                outlineSymbol.Width = 1;
                simplePolygonSymbol.Outline = outlineSymbol;

                return simplePolygonSymbol as ISymbol;
            }
            else if(featureGeometryType == esriGeometryType.esriGeometryPolyline)
            {
                ISimpleLineSymbol simplePolylineSymbol = new SimpleLineSymbolClass();
                simplePolylineSymbol.Color = color;
                simplePolylineSymbol.Width = 4;

                return simplePolylineSymbol as ISymbol;
            }

            ISimpleMarkerSymbol simpleMarkerSymbol = new SimpleMarkerSymbolClass();
            simpleMarkerSymbol.Color = color;
            return simpleMarkerSymbol as ISymbol;
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
