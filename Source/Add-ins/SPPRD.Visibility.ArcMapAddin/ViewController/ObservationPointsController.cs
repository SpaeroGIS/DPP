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
    }
}
