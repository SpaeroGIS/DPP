using MilSpace.DataAccess.DataTransfer;
using System.Collections.Generic;

namespace MilSpace.Visibility.ViewController
{
    public interface IObservationPointsView
    {
        void FillObservationPointList(IEnumerable<ObservationPoint> observationPoints, VeluableObservPointFieldsEnum filter);
        void FillVisibilitySessionsList(IEnumerable<VisibilityTask> visibilitySessions, bool isNewSessionAdded);
        void FillVisibilitySessionsTree(IEnumerable<VisibilityTask> visibilitySessions, bool isNewSessionAdded);
        void FillObservationObjectsList(IEnumerable<ObservationObject> observationObjects);
        void ChangeRecord(int id, ObservationPoint observationPoint);
        void AddRecord(ObservationPoint observationPoint);
        string ObservationPointsFeatureClass { get; }
       // string ObservationStationFeatureClass { get; }
        VeluableObservPointFieldsEnum GetFilter { get; }
        IEnumerable<string> GetTypes { get; }
        IEnumerable<string> GetAffiliation { get; }


    }
}
