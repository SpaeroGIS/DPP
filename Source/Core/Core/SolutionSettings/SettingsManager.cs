using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Core.SolutionSettings
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
    }
}
