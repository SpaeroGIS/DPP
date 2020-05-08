using ESRI.ArcGIS.Geometry;
using MilSpace.Core;
using MilSpace.Core.DataAccess;
using MilSpace.Core.ModulesInteraction;
using MilSpace.DataAccess.DataTransfer;
using System.Collections.Generic;

namespace MilSpace.GeoCalculator.Interaction
{
    public class GeoCalculatorInteraction : IGeocalculatorInteraction
    {
        GeoCalculatorController _controller;
        private static Logger _log = Logger.GetLoggerEx("MilSpace.GeoCalculator.GeoCalculatorInteraction");

        internal GeoCalculatorInteraction(GeoCalculatorController controller)
        {
            _controller = controller;
        }

        public Dictionary<int, IPoint> GetPoints()
        {
           return _controller.GetPointsList();
        }

        public IObserverPoint[] GetGeoCalcPoints()
        {
            return _controller.GetGeoCalcPoints();
        }

        public void UpdateGeoCalcPoint(IObserverPoint geoCalcPoint)
        {
            _controller.UpdatePoint(geoCalcPoint as GeoCalcPoint);
        }
    }
}
