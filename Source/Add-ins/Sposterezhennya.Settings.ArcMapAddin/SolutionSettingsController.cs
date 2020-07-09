using MilSpace.Core.DataAccess;
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
    }
}
