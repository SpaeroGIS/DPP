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
using MilSpace.DataAccess.DataTransfer;
using MilSpace.Visualization3D.Models;
using System;
using System.Collections.Generic;

namespace MilSpace.Visualization3D
{
    public static class Visualization3DHandler
    {
        private static IApplication m_application;

        //Application removed event
        private static IAppROTEvents_Event m_appROTEvent;
        private static int m_appHWnd = 0;
        private static double zFactor;

        private enum LayerTypeEnum
        {
            Raster,
            LineFeature,
            PointFeature,
            PolygonFeature
        }

        static Visualization3DHandler()
        {

        }

        internal static void OpenProfilesSetIn3D(ArcSceneArguments layers)
        {
            OpenArcScene();

            try
            {
                IObjectFactory objFactory = m_application as IObjectFactory;
                var document = (IBasicDocument)m_application.Document;

                zFactor = layers.ZFactor;

                var baseSurface = AddBaseLayers(layers, objFactory, document);
                AddVisibilityLayers(layers.VisibilityResultsInfo, objFactory, document, baseSurface);
                AddExtraLayers(layers.AdditionalLayers, objFactory, document);
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

        private static IFunctionalSurface AddBaseLayers(ArcSceneArguments layers, IObjectFactory objFactory, IBasicDocument document)
        {
            var preparedLayers = GetLayers(layers, objFactory);

            var surface = (IRasterSurface)objFactory.Create("esrianalyst3d.RasterSurface");
            var rasterLayer = (IRasterLayer)preparedLayers[LayerTypeEnum.Raster];
            surface.PutRaster(rasterLayer.Raster, 0);
            var functionalSurface = (IFunctionalSurface)surface;

            SetSurface3DProperties(preparedLayers[0], objFactory, functionalSurface);
            SetFeatures3DProperties((IFeatureLayer)preparedLayers[LayerTypeEnum.LineFeature], objFactory, functionalSurface);
            SetFeatures3DProperties((IFeatureLayer)preparedLayers[LayerTypeEnum.PointFeature], objFactory, functionalSurface);
            SetFeatures3DProperties((IFeatureLayer)preparedLayers[LayerTypeEnum.PolygonFeature], objFactory, functionalSurface);

            foreach(var layer in preparedLayers)
            {
                document.AddLayer(layer.Value);
            }

            document.UpdateContents();

            document.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography,
                                               VisibilityColorsRender((IFeatureLayer)preparedLayers[LayerTypeEnum.LineFeature], objFactory), document.ActiveView.Extent);

            document.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography,
                                               VisibilityColorsRender((IFeatureLayer)preparedLayers[LayerTypeEnum.PolygonFeature], objFactory), document.ActiveView.Extent);

            return functionalSurface;
        }

        private static void AddExtraLayers(IEnumerable<ILayer> additionalLayers, IObjectFactory objFactory, IBasicDocument document)
        {
            foreach(var extraLayer in additionalLayers)
            {
                var featureLayer = CreateLayerCopy((IFeatureLayer)extraLayer, objFactory);

                document.AddLayer(featureLayer);
            }

            document.UpdateContents();
        }

        private static void AddVisibilityLayers(IEnumerable<VisibilityResultInfo> info, IObjectFactory objFactory, IBasicDocument document, IFunctionalSurface baseSurface)
        {
            foreach(var resultInfo in info)
            {
                var layers = GetVisibilityLayers(resultInfo, objFactory, baseSurface);

                foreach(var layer in layers)
                {
                    document.AddLayer(layer.Value);
                }
            }

            document.UpdateContents();
        }

        private static Dictionary<LayerTypeEnum, ILayer> GetVisibilityLayers(VisibilityResultInfo info, IObjectFactory objFactory, IFunctionalSurface baseSurface)
        {
            var layers = new Dictionary<LayerTypeEnum, ILayer>();
            var name = info.ResultName.Replace(" ", string.Empty);

            var rasterLayer = CreateRasterLayer($"{name}_img", objFactory, info.GdbPath);
            if (rasterLayer != null)
            {
                SetVisibilitySessionRaster3DProperties(rasterLayer, objFactory, baseSurface);
                layers.Add(LayerTypeEnum.Raster, rasterLayer);
            }

            var pointFeatureLayer = CreateFeatureLayer($"{name}_op", objFactory, info.GdbPath);
            if (pointFeatureLayer != null)
            {
                SetFeatures3DProperties(pointFeatureLayer, objFactory, baseSurface);
                layers.Add(LayerTypeEnum.PointFeature, pointFeatureLayer);
            }

            var polygonFeatureLayer = CreateFeatureLayer($"{name}_oo", objFactory, info.GdbPath);
            if(polygonFeatureLayer != null)
            {
                SetFeatures3DProperties(polygonFeatureLayer, objFactory, baseSurface);
                layers.Add(LayerTypeEnum.PolygonFeature, polygonFeatureLayer);
            }

            return layers;
        }

        private static Dictionary<LayerTypeEnum, ILayer> GetLayers(ArcSceneArguments layers, IObjectFactory objFactory)
        {
            var preparedLayers = new Dictionary<LayerTypeEnum, ILayer>();

            Type rasterLayerType = typeof(RasterLayerClass);
            string typeRasterLayerID = rasterLayerType.GUID.ToString("B");

            var elevationRasterLayer = (IRasterLayer)objFactory.Create(typeRasterLayerID);
            elevationRasterLayer.CreateFromFilePath(layers.DemLayer);
            preparedLayers.Add(LayerTypeEnum.Raster, elevationRasterLayer);

            preparedLayers.Add(LayerTypeEnum.LineFeature, CreateFeatureLayer(layers.Line3DLayer, objFactory));
            preparedLayers.Add(LayerTypeEnum.PointFeature, CreateFeatureLayer(layers.Point3DLayer, objFactory));
            var polygon3DLayer = CreateFeatureLayer(layers.Polygon3DLayer, objFactory);

            var polygonLayerEffects = (ILayerEffects)polygon3DLayer;
            polygonLayerEffects.Transparency = 50;

            preparedLayers.Add(LayerTypeEnum.PolygonFeature, polygon3DLayer);

            return preparedLayers;
        }

      
        private static IFeatureLayer CreateFeatureLayer(string featureClass, IObjectFactory objFactory, string gdb = null)
        {
            if(gdb == null)
            {
                gdb = MilSpaceConfiguration.ConnectionProperty.TemporaryGDBConnection;
            }

            Type factoryType = Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory");
            string typeFactoryID = factoryType.GUID.ToString("B");

            IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)objFactory.Create(typeFactoryID);
            IWorkspace2 workspace = (IWorkspace2)workspaceFactory.OpenFromFile(gdb, 0);

            if(workspace.NameExists[esriDatasetType.esriDTFeatureClass, featureClass])
            {
                IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)workspace;

                var featureLayer = (IFeatureLayer)objFactory.Create("esriCarto.FeatureLayer");
                var featureClassC = featureWorkspace.OpenFeatureClass(featureClass);

                featureLayer.FeatureClass = featureClassC;
                featureLayer.Name = featureLayer.FeatureClass.AliasName;

                return featureLayer;
            }

            return null;
        }

        private static IRasterLayer CreateRasterLayer(string layerName, IObjectFactory objFactory, string gdb)
        {
            Type factoryType = Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory");
            string typeFactoryID = factoryType.GUID.ToString("B");

            IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)objFactory.Create(typeFactoryID);
            IWorkspace2 workspace = (IWorkspace2)workspaceFactory.OpenFromFile(gdb, 0);

            if(workspace.NameExists[esriDatasetType.esriDTRasterDataset, layerName])
            {
                  Type rasterLayerType = typeof(RasterLayerClass);
                  string typeRasterLayerID = rasterLayerType.GUID.ToString("B");

                  var rasterLayer = (IRasterLayer)objFactory.Create(typeRasterLayerID);
                  rasterLayer.CreateFromFilePath($"{gdb}\\{layerName}");
                  return rasterLayer;
            }

            return null;
        }

        private static IGeoFeatureLayer CreateLayerCopy(IFeatureLayer layer, IObjectFactory objFactory)
        {
            var workspacePath = GetWorkspacePathForLayer(layer);

            Type factoryType = Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory");
            string typeFactoryID = factoryType.GUID.ToString("B");

            IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)objFactory.Create(typeFactoryID);
            IFeatureWorkspace workspace = (IFeatureWorkspace)workspaceFactory.OpenFromFile(workspacePath, 0);

            var featureLayer = (IFeatureLayer)objFactory.Create("esriCarto.FeatureLayer");
            var featureClassC = workspace.OpenFeatureClass(layer.FeatureClass.AliasName);

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

        private static void SetVisibilitySessionRaster3DProperties(IRasterLayer rasterLayer, IObjectFactory objFactory, IFunctionalSurface surface)
        {
            var properties3D = (I3DProperties3)objFactory.Create("esrianalyst3d.Raster3DProperties");
            properties3D.BaseOption = esriBaseOption.esriBaseSurface;
            properties3D.BaseSurface = surface;
            properties3D.OffsetExpressionString = "2";
            properties3D.ZFactor = zFactor;

            properties3D.RenderVisibility = esriRenderVisibility.esriRenderAlways;
            properties3D.RenderMode = esriRenderMode.esriRenderCache;
            properties3D.TextureDownsamplingFactor = 0.7;
            properties3D.AlphaThreshold = 0.1;
            properties3D.RenderRefreshRate = 0.75;
            properties3D.Illuminate = true;
            properties3D.DepthPriorityValue = 1;

            ILayerExtensions layerExtensions = (ILayerExtensions)rasterLayer;
            layerExtensions.AddExtension(properties3D);
            properties3D.Apply3DProperties(rasterLayer);
        }


        private static void SetFeatures3DProperties(IFeatureLayer layer, IObjectFactory objFactory, IFunctionalSurface surface)
        {
            var properties3D = (I3DProperties)objFactory.Create("esrianalyst3d.Feature3DProperties");
            properties3D.BaseOption = esriBaseOption.esriBaseSurface;
            properties3D.BaseSurface = surface;
            properties3D.ZFactor = zFactor;
            properties3D.OffsetExpressionString = "3";

            ILayerExtensions layerExtensions = (ILayerExtensions)layer;
            layerExtensions.AddExtension(properties3D);
            properties3D.Apply3DProperties(layer);
        }

        //private static void SetFeatures3DProperties(IFeatureLayer layer, IObjectFactory objFactory)
        //{
        //    var properties3D = (I3DProperties)objFactory.Create("esrianalyst3d.Feature3DProperties");
        //    properties3D.ZFactor = zFactor;
        //    properties3D.OffsetExpressionString = "3";

        //    ILayerExtensions layerExtensions = (ILayerExtensions)layer;
        //    layerExtensions.AddExtension(properties3D);
        //    properties3D.Apply3DProperties(layer);
        //}

        private static void SetSurface3DProperties(ILayer layer, IObjectFactory objFactory, IFunctionalSurface surface)
        {
            var properties3D = (I3DProperties)objFactory.Create("esrianalyst3d.Raster3DProperties");
            properties3D.BaseOption = esriBaseOption.esriBaseSurface;
            properties3D.BaseSurface = surface;
            properties3D.ZFactor = zFactor;

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
