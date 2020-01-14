using MilSpace.DataAccess.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Visibility.ViewController
{
    public interface IOservPointsView
    {
        void SetController(ObservationPointsController controller);

        void FillObservationPointList(IEnumerable<ObservationPoint> observationPoints, ValuableObservPointFieldsEnum filter);

        ValuableObservPointFieldsEnum GetFilter { get; }


    }
}
