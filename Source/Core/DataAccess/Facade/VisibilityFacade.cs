using MilSpace.DataAccess.DataTransfer;
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
                return res;
            }
        }
        public static IEnumerable<ObservationObject> GetObservationObjectByObjectIds(IEnumerable<int> ids)
        {
            using (var accessor = new VisibilityDataAccess())
            {
                var res = accessor.GetObservationObjectByIds(ids);
                return res;
            }
        }

        public static bool SaveVisibilitySession(VisibilitySession visibilitySession)
        {
            using(var accessor = new VisibilityDataAccess())
            {
                var res = accessor.SaveVisibilitySession(visibilitySession);
                return res;
            }
        }

        public static bool UpdateVisibilitySession(VisibilitySession visibilitySession)
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

        public static IEnumerable<VisibilitySession> GetAllVisibilitySessions()
        {
            using(var accessor = new VisibilityDataAccess())
            {
                var res = accessor.GetAllVisibilitySessions();
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
    }
}
