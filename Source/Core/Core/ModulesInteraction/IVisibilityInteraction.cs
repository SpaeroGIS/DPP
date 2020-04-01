using MilSpace.Core.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Core.ModulesInteraction
{
    public interface IVisibilityInteraction
    {
        List<FromLayerPointModel> GetObservationPoints();
        List<FromLayerGeometry> GetObservationObjects();
        string GetObservationPointsFeatureClassName();
        string GetObservationStationFeatureClassName();
    }
}
