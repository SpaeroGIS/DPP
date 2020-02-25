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

        public List<ObservObjectsShape> GetObservationObjects()
        {
            return _controller.GetObservObjectsFromModule();
        }
    }
}
