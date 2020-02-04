using ESRI.ArcGIS.Geometry;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.GeoCalculator
{
    public class GeoCalculatorController
    {
        internal void SetView(IGeoCalculatorView view)
        {
            View = view;
        }

        internal IGeoCalculatorView View { get; private set; }


        public void ExportToLayer(Dictionary<int, IPoint> points)
        {
            var fClassName = $"GCP_{DateTime.Now.ToString("yyyyMMddHHmmss")}_P";

            var featureClass = GdbAccess.Instance.AddCalcPointsFeature(points, fClassName, EsriTools.Wgs84Spatialreference);
            ArcMapHelper.AddFeatureClassToMap(featureClass);
        }

    }
}
