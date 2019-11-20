using MilSpace.DataAccess.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MilSpace.DataAccess.Facade
{
    public static class VisibilityZonesFacade
    {
        public static IEnumerable<ObservationPoint> GetAllObservationPoints()
        {
            using (var accessor = new VisibilityDataAccess())
            {
                var res = accessor.GetObservationPoints();
                return res.ToArray();
            }
        }

        public static bool SaveObservationPoint(ObservationPoint observPoint)
        {
            using (var accessor = new VisibilityDataAccess())
            {
                var res = accessor.SaveObservationPoint(observPoint);
                return res;                
            }
        }

        public static IEnumerable<ObservationPoint>  GetObservationPointByObjectIds(IEnumerable<int> ids)
        {
            using (var accessor = new VisibilityDataAccess())
            {
                var res = accessor.GetObservationPointsByIds(ids);
                return res.ToArray();
            }
        }
        public static IEnumerable<ObservationObject> GetObservationObjectByObjectIds(IEnumerable<int> ids)
        {
            using (var accessor = new VisibilityDataAccess())
            {
                var res = accessor.GetObservationObjectByIds(ids).ToArray();
                return res;
            }
        }

        public static VisibilityTask AddVisibilitySession(VisibilityTask visibilitySession)
        {
            using(var accessor = new VisibilityDataAccess())
            {
                var res = accessor.AddVisibilityTask(visibilitySession);
                return res;
            }
        }

        public static VisibilityTask FinishVisibilitySession(VisibilityTask visibilitySession)
        {
            using (var accessor = new VisibilityDataAccess())
            {
                visibilitySession.Finished = DateTime.Now;
                var res = accessor.UpdateVisibilityTask(visibilitySession);
                return res;
            }
        }

        public static VisibilityTask StarthVisibilitySession(VisibilityTask visibilitySession)
        {
            using (var accessor = new VisibilityDataAccess())
            {
                visibilitySession.Started = DateTime.Now;
                var res = accessor.UpdateVisibilityTask(visibilitySession);
                return res;
            }
        }

        public static VisibilityTask UpdateVisibilitySession(VisibilityTask visibilitySession)
        {
            using(var accessor = new VisibilityDataAccess())
            {
                var res = accessor.UpdateVisibilityTask(visibilitySession);
                return res;
            }
        }

        public static bool DeleteVisibilitySession(string id)
        {
            using(var accessor = new VisibilityDataAccess())
            {
                var res = accessor.DeleteVisibilityTask(id);
                return res;
            }
        }

        public static IEnumerable<VisibilityTask> GetAllVisibilityTasks(bool forCurrentUser = false)
        {
            using(var accessor = new VisibilityDataAccess())
            {
                var res =  forCurrentUser ? accessor.GetUserVisibilityTasks() : accessor.GetAllVisibilityTasks();
                return res.ToArray();
            }
        }

        public static IEnumerable<VisibilityTask> GetFinishedVisibilityTasks()
        {
            using (var accessor = new VisibilityDataAccess())
            {
                var res = accessor.GetAllVisibilityTasks(true);
                return res.ToArray();
            }
        }

        public static IEnumerable<ObservationObject> GetAllObservationObjects()
        {
            using (var accessor = new VisibilityDataAccess())
            {
                var res = accessor.GetAllObservationObjects();
                return res.ToArray();
            }
        }

        public static bool CheckVisibilityResultEistance(string resultName, VisibilityCalculationresultsEnum resultType)
        {
            var result = VisibilityTask.GetEsriDataTypeByVisibilityresyltType(resultType);
            return GdbAccess.Instance.CheckDatasetExistanceInCalcWorkspace(resultName, result);
        }


        public static VisibilityCalcResults AddVisibilityResults(VisibilityCalcResults visibilityResults)
        {
            using(var accessor = new VisibilityDataAccess())
            {
                var res = accessor.AddVisibilityResults(visibilityResults);
                return res;
            }
        }

        public static bool AddSharedVisibilityResultsToUserSession(VisibilityCalcResults visibilityResults)
        {
            using(var accessor = new VisibilityDataAccess())
            {
                var res = accessor.AddShareddResultsToUserSession(visibilityResults);
                return res;
            }
        }

        public static VisibilityCalcResults UpdateVisibilityResults(VisibilityCalcResults visibilityResults)
        {
            using(var accessor = new VisibilityDataAccess())
            {
                var res = accessor.UpdateVisibilityResults(visibilityResults);
                return res;
            }
        }

        public static bool DeleteVisibilityResults(string id)
        {
            using(var accessor = new VisibilityDataAccess())
            {
                var res = accessor.DeleteVisibilityResults(id);
                return res;
            }
        }

        public static bool DeleteVisibilityResultsFromUserSession(string id)
        {
            using(var accessor = new VisibilityDataAccess())
            {
                var res = accessor.DeleteVisibilityResultsFromUserSession(id);
                return res;
            }
        }

        public static bool IsResultsBelongToUser(string id)
        {
            using(var accessor = new VisibilityDataAccess())
            {
                var res = accessor.IsResultsBelongToUser(id);
                return res;
            }
        }

        public static IEnumerable<VisibilityCalcResults> GetAllVisibilityResults(bool forCurrentUser = false)
        {
            using(var accessor = new VisibilityDataAccess())
            {
                var res = accessor.GetAllVisibilityResults(forCurrentUser);
                return res.ToArray();
            }
        }
    }
}
