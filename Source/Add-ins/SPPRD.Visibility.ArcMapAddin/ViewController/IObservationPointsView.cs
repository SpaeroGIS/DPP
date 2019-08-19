using MilSpace.DataAccess.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Visibility.ViewController
{
    public interface IObservationPointsView
    {

        void FillObservationPointList(IEnumerable<ObservationPoint> observationPoints, VeluableObservPointFieldsEnum filter);

        VeluableObservPointFieldsEnum GetFilter { get; }

        IEnumerable<string> GetTypes { get; }
        IEnumerable<string> GetAffiliation { get; }
    }
}
