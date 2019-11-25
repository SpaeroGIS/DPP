using MilSpace.DataAccess.DataTransfer;
using System.Collections.Generic;

namespace MilSpace.Visibility.ViewController.WizardController
{
    internal interface IWizardView
    {

        void FillObservPointsOnCurrentView(IEnumerable<ObservationPoint> observationPoints);
        void FillObservationPointList(IEnumerable<ObservationPoint> observationPoints);
        void FillObsObj( IEnumerable<ObservationObject> All ,bool useCurrentExtent);
        string ObservationPointsFeatureClass { get; }
        VeluableObservPointFieldsEnum GetFilter { get; }
       
    }
}
