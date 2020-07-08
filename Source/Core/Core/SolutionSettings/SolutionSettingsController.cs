using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Core.SolutionSettings
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
    }
}
