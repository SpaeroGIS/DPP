using MilSpace.Core;
using MilSpace.Core.ArcMap;
using MilSpace.Core.DataAccess;
using MilSpace.Core.ModulesInteraction;
using MilSpace.Tools.GraphicsLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MilSpace.Settings
{
    public static class SettingsManager
    {
        public static event Action<string> OnRasterChanged;
        public static string RasterLayer;

        private static SolutionSettingsController _solutionSettingsController;
        private static Logger _logger = Logger.GetLoggerEx("MilSpace.Settings");

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

        public static void ShowSelectedGraphics(IEnumerable<GraphicsTypesEnum> selectedGraphics)
        {
            if (selectedGraphics.Contains(GraphicsTypesEnum.Solution))
            {
                ShowGraphicsBySelectedType(GraphicsTypesEnum.Solution);
                return;
            }

            foreach (var graphicsType in selectedGraphics)
            {
                ShowGraphicsBySelectedType(graphicsType);
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

        private static void ShowGraphicsBySelectedType(GraphicsTypesEnum graphicsType)
        {
            switch (graphicsType)
            {
                case GraphicsTypesEnum.Solution:

                    ShowGeocalulatorGraphics();
                    ShowVisibilityGraphics();
                    ShowProfileGraphics();

                    break;

                case GraphicsTypesEnum.Geocalculator:

                    ShowGeocalulatorGraphics();

                    break;

                case GraphicsTypesEnum.Visibility:

                    ShowVisibilityGraphics();

                    break;

                case GraphicsTypesEnum.Profile:

                    ShowProfileGraphics();

                    break;

                case GraphicsTypesEnum.None:

                    return;
            }
        }

        private static void ShowProfileGraphics()
        {
            var profilesModule = ModuleInteraction.Instance.GetModuleInteraction<IProfileInteraction>(out bool changes);

            if (!changes && profilesModule == null)
            {
                MessageBox.Show(LocalizationContext.Instance.ProfileModuleNotConnectedMessage, LocalizationContext.Instance.MessageBoxTitle);
                _logger.WarnEx($"> ShowProfileGraphics Exception: {LocalizationContext.Instance.ProfileModuleNotConnectedMessage}");
                return;
            }

            profilesModule.UpdateGraphics();
        }

        private static void ShowVisibilityGraphics()
        {
            var visibilityModule = ModuleInteraction.Instance.GetModuleInteraction<IVisibilityInteraction>(out bool changes);

            if (!changes && visibilityModule == null)
            {
                MessageBox.Show(LocalizationContext.Instance.VisibilityModuleNotConnectedMessage, LocalizationContext.Instance.MessageBoxTitle);
                _logger.WarnEx($"> ShowVisibilityGraphics Exception: {LocalizationContext.Instance.VisibilityModuleNotConnectedMessage}");
                return;
            }

            visibilityModule.UpdateGraphics();
        }

        private static void ShowGeocalulatorGraphics()
        {
            var geoCalcModule = ModuleInteraction.Instance.GetModuleInteraction<IGeocalculatorInteraction>(out bool changes);

            if (!changes && geoCalcModule == null)
            {
                MessageBox.Show(LocalizationContext.Instance.GeocalcModuleNotConnectedMessage, LocalizationContext.Instance.MessageBoxTitle);
                _logger.WarnEx($"> ShowGeocalulatorGraphics Exception: {LocalizationContext.Instance.GeocalcModuleNotConnectedMessage}");
                return;
            }

            geoCalcModule.UpdateGraphics();
        }
    }
}
