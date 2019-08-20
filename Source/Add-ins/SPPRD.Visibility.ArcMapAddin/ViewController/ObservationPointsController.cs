using ESRI.ArcGIS.Carto;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Visibility.ViewController
{
    public class ObservationPointsController
    {
        IObservationPointsView view;
        private static readonly string observPointFeature = "MilSp_Visible_ObservPoints";

        public ObservationPointsController()
        { }

        internal void SetView(IObservationPointsView view)
        {
            this.view = view;
        }

        internal void OnCreateFeature(ESRI.ArcGIS.Geodatabase.IObject obj)
        {
            throw new NotImplementedException();
        }

        internal void UpdateObservationPointsList()
        {
            view.FillObservationPointList(VisibilityFacade.GetAllObservationPoints(), view.GetFilter);
        }

        internal IEnumerable<ObservationPoint> GetAllBoservationPoints()
        {
            return VisibilityFacade.GetAllObservationPoints();
        }


        public IEnumerable<string> GetObservationPointTypes()
        {
            return Enum.GetNames(typeof(ObservationPointTypesEnum));
        }

        public IEnumerable<string> GetObservationPointMobilityTypes()
        {
            return Enum.GetNames(typeof(ObservationPointMobilityTypesEnum));
        }

        public bool IsObservPointsExists(IActiveView view)
        {
            var layers = view.FocusMap.Layers;
            var layer = layers.Next();

            while (layer != null)
            {
                if (layer is IFeatureLayer fl && fl.FeatureClass.AliasName.Equals(observPointFeature, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }

                layer = layers.Next();
            }

            return false;
        }
    }
}
