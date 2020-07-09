using MilSpace.Core.ArcMap;
using MilSpace.Core.DataAccess;
using MilSpace.Tools.GraphicsLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Settings
{
    public static class SettingsManager
    {
        public static event Action<string> OnRasterChanged;
        public static string RasterLayer;

        private static SolutionSettingsController _solutionSettingsController;

        internal static void RasterChangedInvoke(string rasterLayer)
        {
            RasterLayer = rasterLayer;
            OnRasterChanged.Invoke(rasterLayer);
        }

        internal static void SetViewController(SolutionSettingsController solutionSettingsController)
        {
            _solutionSettingsController = solutionSettingsController;
        }

        public static void SetNewRaster(string rasterLayer)
        {
            if (_solutionSettingsController != null)
            {
                _solutionSettingsController.SetNewRasterLayer(rasterLayer);
            }
        }

        public static void ClearSelectedGraphics(IEnumerable<GraphicsTypesEnum> selectedGraphics)
        {
            if(selectedGraphics.Contains(GraphicsTypesEnum.All))
            {
                ClearGraphicsBySelectedType(GraphicsTypesEnum.All);
                return;
            }

            bool skipModulesGraphics = selectedGraphics.Contains(GraphicsTypesEnum.Solution);

            foreach (var graphicsType in selectedGraphics)
            {
                if(skipModulesGraphics)
                {
                    if(graphicsType == GraphicsTypesEnum.Profile 
                        || graphicsType == GraphicsTypesEnum.Visibility
                        || graphicsType == GraphicsTypesEnum.Geocalculator)
                    {
                        continue;
                    }
                }

                ClearGraphicsBySelectedType(graphicsType);
            }
        }

        private static void ClearGraphicsBySelectedType(GraphicsTypesEnum graphicsType)
        {
            var graphicsLayerManager = GraphicsLayerManager.GetGraphicsLayerManager(ArcMapInstance.Document.ActiveView);

            switch (graphicsType)
            {
                case GraphicsTypesEnum.All:

                    graphicsLayerManager.RemoveAllGraphicsFromMap();

                    break;

                case GraphicsTypesEnum.AllButSolution:

                    graphicsLayerManager.RemoveSolutionGraphics(true);

                    break;

                case GraphicsTypesEnum.Solution:

                    graphicsLayerManager.RemoveSolutionGraphics(false);

                    break;

                case GraphicsTypesEnum.Geocalculator:

                    graphicsLayerManager.RemoveModuleGeometryFromMap(null, MilSpaceGraphicsTypeEnum.GeoCalculator);

                    break;

                case GraphicsTypesEnum.Visibility:

                    graphicsLayerManager.RemoveModuleGeometryFromMap(null, MilSpaceGraphicsTypeEnum.Visibility);

                    break;

                case GraphicsTypesEnum.Profile:

                    graphicsLayerManager.RemoveModuleGeometryFromMap(null, MilSpaceGraphicsTypeEnum.Session);

                    break;

                case GraphicsTypesEnum.None:

                    return;
            }
        }
    }
}
