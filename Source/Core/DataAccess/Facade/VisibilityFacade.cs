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

        public static VisibilitySession AddVisibilitySession(VisibilitySession visibilitySession)
        {
            using(var accessor = new VisibilityDataAccess())
            {
                var res = accessor.AddVisibilitySession(visibilitySession);
                return res;
            }
        }

        public static VisibilitySession FinishVisibilitySession(VisibilitySession visibilitySession)
        {
            using (var accessor = new VisibilityDataAccess())
            {
                visibilitySession.Finished = DateTime.Now;
                var res = accessor.UpdateVisibilitySession(visibilitySession);
                return res;
            }
        }

        public static VisibilitySession StarthVisibilitySession(VisibilitySession visibilitySession)
        {
            using (var accessor = new VisibilityDataAccess())
            {
                visibilitySession.Started = DateTime.Now;
                var res = accessor.UpdateVisibilitySession(visibilitySession);
                return res;
            }
        }

        public static VisibilitySession UpdateVisibilitySession(VisibilitySession visibilitySession)
        {
            using(var accessor = new VisibilityDataAccess())
            {
                var res = accessor.UpdateVisibilitySession(visibilitySession);
                return res;
            }
        }

        public static bool DeleteVisibilitySession(string id)
        {
            using(var accessor = new VisibilityDataAccess())
            {
                var res = accessor.DeleteVisibilitySession(id);
                return res;
            }
        }

        public static IEnumerable<VisibilitySession> GetAllVisibilitySessions(bool forCurrentUser = false)
        {
            using(var accessor = new VisibilityDataAccess())
            {
                var res =  forCurrentUser ? accessor.GetUserVisibilitySessions() : accessor.GetAllVisibilitySessions();
                return res.ToArray();
            }
        }

        public static IEnumerable<VisibilitySession> GetFinishedVisibilitySessions()
        {
            using (var accessor = new VisibilityDataAccess())
            {
                var res = accessor.GetAllVisibilitySessions(true);
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
            var result = VisibilitySession.GetEsriDataTypeByVisibilityresyltType(resultType);
            return GdbAccess.Instance.CheckDatasetExistanceInCalcWorkspace(resultName, result);
        }
    }
}
