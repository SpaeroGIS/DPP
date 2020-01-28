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
using System.Linq;
using System.Runtime.InteropServices;

namespace MilSpace.Visualization3D
{
    public static class Visualization3DHandler
    {
        private static IApplication m_application;

        //Application removed event
        private static IAppROTEvents_Event m_appROTEvent;
        private static int m_appHWnd = 0;
        private static double zFactor;
        private static readonly string _profileGdb = MilSpaceConfiguration.ConnectionProperty.TemporaryGDBConnection;

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
                AddExtraLayers(layers.AdditionalLayers, objFactory, document, baseSurface);
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

            if(preparedLayers.Count > 1)
            {
                SetSurface3DProperties(preparedLayers[0], objFactory, functionalSurface);
                SetFeatures3DProperties((IFeatureLayer)preparedLayers[LayerTypeEnum.LineFeature], objFactory, functionalSurface);
                SetHightFeatures3DProperties((IFeatureLayer)preparedLayers[LayerTypeEnum.PointFeature], objFactory);
                SetHightFeatures3DProperties((IFeatureLayer)preparedLayers[LayerTypeEnum.PolygonFeature], objFactory);
            }

            foreach(var layer in preparedLayers)
            {
                try
                {
                    document.AddLayer(layer.Value);
                }
                catch(Exception ex)
                {
                    //todo add log
                }
            }

            document.UpdateContents();

            if(preparedLayers.Count > 1)
            {
                document.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography,
                                                   VisibilityColorsRender((IFeatureLayer)preparedLayers[LayerTypeEnum.LineFeature], objFactory), document.ActiveView.Extent);

                document.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography,
                                                      PointsRender((IFeatureLayer)preparedLayers[LayerTypeEnum.PointFeature], new RgbColor() { Red = 255, Blue = 24, Green = 198 }, objFactory), document.ActiveView.Extent);

                document.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography,
                                                   VisibilityColorsRender((IFeatureLayer)preparedLayers[LayerTypeEnum.PolygonFeature], objFactory), document.ActiveView.Extent);
            }

            return functionalSurface;
        }

        private static void AddExtraLayers(Dictionary<ILayer, double> additionalLayers, IObjectFactory objFactory,
                                            IBasicDocument document, IFunctionalSurface surface)
        {
            foreach(var extraLayer in additionalLayers)
            {
                var featureLayer = CreateLayerCopy((IFeatureLayer)extraLayer.Key, objFactory);
                SetFeatures3DProperties(featureLayer, objFactory, surface, extraLayer.Value);

                try
                {
                    document.AddLayer(featureLayer);
                }
                catch(Exception ex)
                {
                    //todo add log
                }
            }

            document.UpdateContents();
        }

        private static void AddVisibilityLayers(IEnumerable<VisibilityResultInfo> info, IObjectFactory objFactory, IBasicDocument document, IFunctionalSurface baseSurface)
        {
            Dictionary<ILayer, LayerTypeEnum> layers = new Dictionary<ILayer, LayerTypeEnum>();

            foreach(var resultInfo in info)
            {
                var layer = GetVisibilityLayer(resultInfo, objFactory, baseSurface);

                if(layer.Key != null)
                {
                    layers.Add(layer.Key, layer.Value);

                    try
                    {
                        document.AddLayer(layer.Key);
                    }
                    catch(Exception ex)
                    {
                        //todo add log
                    }
                }
            }

            if(layers.ContainsValue(LayerTypeEnum.PointFeature))
            {
                document.UpdateContents();

                foreach(var layer in layers)
                {
                    if(layer.Value == LayerTypeEnum.PointFeature)
                    {
                        document.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography,
                                                   PointsRender((IFeatureLayer)layer.Key, new RgbColor() { Red = 24, Blue = 255, Green = 163 }, objFactory), document.ActiveView.Extent);
                    }
                }
            }
        }

        private static KeyValuePair<ILayer, LayerTypeEnum> GetVisibilityLayer(VisibilityResultInfo info, IObjectFactory objFactory, IFunctionalSurface baseSurface)
        {
            KeyValuePair<ILayer, LayerTypeEnum> layerKeyValuePair = new KeyValuePair<ILayer, LayerTypeEnum>();
            Type factoryType = Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory");
            string typeFactoryID = factoryType.GUID.ToString("B");

            IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)objFactory.Create(typeFactoryID);
            IWorkspace2 workspace = (IWorkspace2)workspaceFactory.OpenFromFile(info.GdbPath, 0);

            var rastersTypes = VisibilityCalcResults.GetRasterResults();

            if (rastersTypes.Any(type => type == info.RessutType))
            {
                var rasterLayer = CreateRasterLayer(info.ResultName, workspace, objFactory, info.GdbPath);
                if(rasterLayer != null)
                {
                    SetVisibilitySessionRaster3DProperties(rasterLayer, objFactory, baseSurface);
                    layerKeyValuePair =  new KeyValuePair<ILayer, LayerTypeEnum>(rasterLayer, LayerTypeEnum.Raster);
                }
            }

            if(info.RessutType == VisibilityCalculationResultsEnum.ObservationPoints || info.RessutType == VisibilityCalculationResultsEnum.ObservationPointSingle)
            {
                var pointFeatureLayer = CreateFeatureLayer(info.ResultName, workspace, objFactory);
                if(pointFeatureLayer != null)
                {
                    SetFeatures3DProperties(pointFeatureLayer, objFactory, baseSurface);
                    layerKeyValuePair = new KeyValuePair<ILayer, LayerTypeEnum>(pointFeatureLayer, LayerTypeEnum.PointFeature);
                }
            }

            if(info.RessutType == VisibilityCalculationResultsEnum.VisibilityAreaPolygons || info.RessutType == VisibilityCalculationResultsEnum.ObservationObjects)
            {
                var polygonFeatureLayer = CreateFeatureLayer(info.ResultName, workspace, objFactory);
                if(polygonFeatureLayer != null)
                {
                    SetFeatures3DProperties(polygonFeatureLayer, objFactory, baseSurface);
                    layerKeyValuePair = new KeyValuePair<ILayer, LayerTypeEnum>(polygonFeatureLayer, LayerTypeEnum.PolygonFeature);
                }
            }

            Marshal.ReleaseComObject(workspaceFactory);

            return layerKeyValuePair;
        }

        private static Dictionary<LayerTypeEnum, ILayer> GetLayers(ArcSceneArguments layers, IObjectFactory objFactory)
        {
            var preparedLayers = new Dictionary<LayerTypeEnum, ILayer>();

            Type rasterLayerType = typeof(RasterLayerClass);
            string typeRasterLayerID = rasterLayerType.GUID.ToString("B");

            var elevationRasterLayer = (IRasterLayer)objFactory.Create(typeRasterLayerID);
            elevationRasterLayer.CreateFromFilePath(layers.DemLayer);
            preparedLayers.Add(LayerTypeEnum.Raster, elevationRasterLayer);

            Type factoryType = Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory");
            string typeFactoryID = factoryType.GUID.ToString("B");

            IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)objFactory.Create(typeFactoryID);
            IWorkspace2 workspace = (IWorkspace2)workspaceFactory.OpenFromFile(_profileGdb, 0);

            if(!string.IsNullOrEmpty(layers.Line3DLayer))
            {
                preparedLayers.Add(LayerTypeEnum.LineFeature, CreateFeatureLayer(layers.Line3DLayer, workspace, objFactory));
                preparedLayers.Add(LayerTypeEnum.PointFeature, CreateFeatureLayer(layers.Point3DLayer, workspace, objFactory));

                var polygon3DLayer = CreateFeatureLayer(layers.Polygon3DLayer, workspace, objFactory);


                var polygonLayerEffects = (ILayerEffects)polygon3DLayer;
                polygonLayerEffects.Transparency = 50;

                preparedLayers.Add(LayerTypeEnum.PolygonFeature, polygon3DLayer);
            }

            Marshal.ReleaseComObject(workspaceFactory);

            return preparedLayers;
        }

      
        private static IFeatureLayer CreateFeatureLayer(string featureClass, IWorkspace2 workspace, IObjectFactory objFactory)
        {
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

        private static IRasterLayer CreateRasterLayer(string layerName, IWorkspace2 workspace, IObjectFactory objFactory, string gdb)
        {
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

            //var objCopy = (IObjectCopy)objFactory.Create("esriSystem.ObjectCopy");

            //IGeoFeatureLayer geoFL = featureLayer as IGeoFeatureLayer;
            //var renderer = geoFL.Renderer as object;

            //var rendererCopyObj = objCopy.Copy(geoArcMapLayer.Renderer);

            ////geoFL.Renderer = rendererCopyObj as IFeatureRenderer;
            //objCopy.Overwrite(geoArcMapLayer.Renderer, ref rendererCopyObj);
            Marshal.ReleaseComObject(workspaceFactory);

            return geoFL;
        }

        private static IGeoFeatureLayer PointsRender(IFeatureLayer layer, RgbColor color, IObjectFactory objFactory)
        {
            Type renderType = typeof(SimpleRendererClass);
            string typeRenderID = renderType.GUID.ToString("B");

            ISimpleRenderer renderer = (ISimpleRenderer)objFactory.Create(typeRenderID);
            renderer.Symbol = GetSymbol(esriGeometryType.esriGeometryPoint, color, objFactory);
            
            IGeoFeatureLayer geoFL = layer as IGeoFeatureLayer;
            geoFL.Renderer = renderer as IFeatureRenderer;

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


        private static void SetFeatures3DProperties(IFeatureLayer layer, IObjectFactory objFactory, IFunctionalSurface surface, double height = double.NaN)
        {
            var properties3D = (I3DProperties)objFactory.Create("esrianalyst3d.Feature3DProperties");
            properties3D.BaseOption = esriBaseOption.esriBaseSurface;
            properties3D.BaseSurface = surface;
            properties3D.ZFactor =  zFactor ;
            properties3D.OffsetExpressionString = (height == double.NaN) ? "3" : height.ToString();

            ILayerExtensions layerExtensions = (ILayerExtensions)layer;
            layerExtensions.AddExtension(properties3D);
            properties3D.Apply3DProperties(layer);
        }

        private static void SetHightFeatures3DProperties(IFeatureLayer layer, IObjectFactory objFactory)
        {
            var properties3D = (I3DProperties)objFactory.Create("esrianalyst3d.Feature3DProperties");
            properties3D.BaseOption = esriBaseOption.esriBaseShape;
            properties3D.ZFactor = zFactor;
            properties3D.OffsetExpressionString = "3";

            ILayerExtensions layerExtensions = (ILayerExtensions)layer;
            layerExtensions.AddExtension(properties3D);
            properties3D.Apply3DProperties(layer);
        }

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

        private static ISymbol GetSymbol(esriGeometryType featureGeometryType, RgbColor color, IObjectFactory objFactory = null)
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

            if(featureGeometryType == esriGeometryType.esriGeometryPolyline)
            {
                ISimpleLineSymbol simplePolylineSymbol = new SimpleLineSymbolClass();
                simplePolylineSymbol.Color = color;
                simplePolylineSymbol.Width = 4;

                return simplePolylineSymbol as ISymbol;
            }

            if(featureGeometryType == esriGeometryType.esriGeometryPoint)
            {
                Type factoryType = Type.GetTypeFromProgID("esriDisplay.SimpleMarkerSymbol");
                string typeFactoryID = factoryType.GUID.ToString("B");

                ISimpleMarkerSymbol pointMarkerSymbol = (ISimpleMarkerSymbol)objFactory.Create(typeFactoryID);
                pointMarkerSymbol.Color = color;
                pointMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;
                pointMarkerSymbol.Size = 30;

                return pointMarkerSymbol as ISymbol;
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
