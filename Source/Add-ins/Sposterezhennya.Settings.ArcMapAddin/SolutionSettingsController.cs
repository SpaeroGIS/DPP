using ESRI.ArcGIS.DataSourcesRaster;
using MilSpace.Configurations;
using MilSpace.Core.DataAccess;
using MilSpace.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Settings
{
    internal class SolutionSettingsController
    {
        private readonly SolutionSettingsForm _view;
       // public string RasterLayer => _view.RasterLayer;

        public SolutionSettingsController(SolutionSettingsForm view)
        {
            _view = view;
            SettingsManager.SetViewController(this);
        }

        public void ChangeRasterLayer(string rasterLayer)
        {
            if (!String.IsNullOrEmpty(rasterLayer))
            {
                SettingsManager.RasterChangedInvoke(rasterLayer);
            }
        }

        public void SetNewRasterLayer(string rasterLayer)
        {
           _view.SetNewRasterLayer(rasterLayer);
        }

        public GraphicsTypesEnum GetClearGraphicsTypeByString(string typeAsString)
        {
            return LocalizationContext.Instance.ClearGraphicsLocalisation
                                               .First(text => text.Value.Equals(typeAsString))
                                               .Key;
        }

        public GraphicsTypesEnum GetShowGraphicsTypeByString(string typeAsString)
        {
            return LocalizationContext.Instance.ShowGraphicsLocalisation
                                               .First(text => text.Value.Equals(typeAsString))
                                               .Key;
        }

        public void ClearSelectedGraphics(IEnumerable<GraphicsTypesEnum> selectedGraphics)
        {
            SettingsManager.ClearSelectedGraphics(selectedGraphics);
        }

        public void ShowSelectedGraphics(IEnumerable<GraphicsTypesEnum> selectedGraphics)
        {
            SettingsManager.ShowSelectedGraphics(selectedGraphics);
        }

        public string[] GetRasterInfo(string rasterName)
        {
            if (String.IsNullOrEmpty(rasterName))
            {
                return null;
            }

            var mapLayerManager = new MapLayersManager(ArcMap.Document.ActiveView);
            var rasterInfo = new string[5];
            IRasterFunctionHelper functionHelper = new RasterFunctionHelper();

            var rasterLayer = mapLayerManager.RasterLayers.First(layer => layer.Name.Equals(rasterName));
            
            var filePath = rasterLayer.FilePath;

            var rasterProps = rasterLayer.Raster as IRasterProps;
            var defaultRasterProps = rasterLayer.Raster as IRasterDefaultProps;

            var pixelSize = EsriTools.GetPixelSize(ArcMap.Document.ActiveView);
            var spatialResolution = pixelSize / rasterProps.MeanCellSize().X;

            var heightInPixels = rasterProps.Height;
            var widthInPixels = rasterProps.Width;

            var heightInKilometres = rasterProps.Extent.Height / EsriTools.GetMetresInMapUnits(1000, ArcMap.Document.FocusMap.SpatialReference);
            var widthInKilometres = rasterProps.Extent.Width / EsriTools.GetMetresInMapUnits(1000, ArcMap.Document.FocusMap.SpatialReference);

            var area = heightInKilometres * widthInKilometres;

            rasterInfo[0] = 
                        String.Format(LocalizationContext.Instance
                                                         .FindLocalizedElement("SolutionSettingsWindow_lbRasterInfoLocationText",
                                                                                "розташування: {0}"), filePath);

            rasterInfo[1] =
                        String.Format(LocalizationContext.Instance
                                                         .FindLocalizedElement("SolutionSettingsWindow_lbRasterInfoSpatialResolutionText",
                                                                                "просторова роздільна здатність: {0}"), spatialResolution);

            rasterInfo[2] = 
                        String.Format(LocalizationContext.Instance
                                                         .FindLocalizedElement("SolutionSettingsWindow_lbRasterInfoAreaText",
                                                                                "площа: {0}"), area);

            rasterInfo[3] =
                        String.Format(LocalizationContext.Instance
                                                         .FindLocalizedElement("SolutionSettingsWindow_lbRasterInfoSizeInKilometresText",
                                                                                "розмір (км): висота {0}  ширина {1}"), heightInKilometres, widthInKilometres);

            rasterInfo[4] = 
                        String.Format(LocalizationContext.Instance
                                                         .FindLocalizedElement("SolutionSettingsWindow_lbRasterInfoSizeInPixelsText",
                                                                                "розмір (пікс.): висота {0}  ширина {1}"), heightInPixels, widthInPixels);
            
            return rasterInfo;
        }

        internal Dictionary<string, string> GetSessionInfo()
        {
            var sessionInfo = new Dictionary<string, string>();

            sessionInfo.Add(LocalizationContext.Instance
                                               .FindLocalizedElement("SolutionSettingsWindow_lvConfigurationConnectionStringText", "рядок підключення:"),
                             MilSpaceConfiguration.ConnectionProperty.WorkingDBConnection);

            sessionInfo.Add(LocalizationContext.Instance
                                               .FindLocalizedElement("SolutionSettingsWindow_lvConfigurationUserString", "користувач:"),
                             Environment.UserName);

            sessionInfo.Add(LocalizationContext.Instance
                                               .FindLocalizedElement("SolutionSettingsWindow_lvConfigurationDataBaseString", "робоча геобаза:"),
                             MilSpaceConfiguration.ConnectionProperty.WorkingGDBConnection);

            return sessionInfo;
        }
    }
}
