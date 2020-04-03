using ESRI.ArcGIS.Geometry;
using MilSpace.DataAccess.DataTransfer;
using System.Collections.Generic;

namespace MilSpace.Visibility.ViewController
{
    public interface IObservationPointsView
    {
        void FillObservationPointList(IEnumerable<ObservationPoint> observationPoints, ValuableObservPointFieldsEnum filter);
        void FillVisibilitySessionsList(IEnumerable<VisibilityTask> visibilitySessions, bool isNewSessionAdded, string newTaskName);
        void FillVisibilityResultsTree(IEnumerable<VisibilityCalcResults> visibilityResults);
        void FillObservationObjectsList(IEnumerable<ObservationObject> observationObjects);
        void FillSelectedOPFields(ObservationPoint point, string layer);
        void AddSelectedOO(IGeometry geometry, string title, string layer);
        void ChangeRecord(int id, ObservationPoint observationPoint);
        void AddRecord(ObservationPoint observationPoint);
        void RemoveSessionFromList(string id);
        string ObservationPointsFeatureClass { get; }
       // string ObservationStationFeatureClass { get; }
        ValuableObservPointFieldsEnum GetFilter { get; }
        IEnumerable<string> GetTypes { get; }
        IEnumerable<string> GetAffiliation { get; }

    }
}
