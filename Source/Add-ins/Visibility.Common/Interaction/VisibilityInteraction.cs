using MilSpace.Core;
using MilSpace.Core.DataAccess;
using MilSpace.Core.ModulesInteraction;
using MilSpace.Visibility.ViewController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Visibility.Interaction
{
    public class VisibilityInteraction : IVisibilityInteraction
    {
        private ObservationPointsController _controller;

        internal VisibilityInteraction(ObservationPointsController observationPointsController)
        {
            _controller = observationPointsController;
        }

        public List<FromLayerPointModel> GetObservationPoints()
        {
            return _controller.GetObservationPointsFromModule();
        }

        public List<FromLayerGeometry> GetObservationObjects()
        {
            return _controller.GetObservObjectsFromModule();
        }

        public string GetObservationPointsFeatureClassName()
        {
            return _controller.GetObservPointsFromGdbFeatureClassName();
        }

        public string GetObservationStationFeatureClassName()
        {
            return _controller.GetObservObjectsFromGdbFeatureClassName();
        }

        public void UpdateGraphics()
        {
            _controller.UpdateGraphics();
        }
    }
}
