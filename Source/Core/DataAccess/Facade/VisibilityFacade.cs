using MilSpace.DataAccess.DataTransfer;
using System.Collections.Generic;
using System.Linq;

namespace MilSpace.DataAccess.Facade
{
    public static class VisibilityZonesFacade
    {
        public static IEnumerable<ObservationPoint> GetAllObservationPoints()
        {
            using (var accessor = new SemanticDataAccess())
            {
                var res = accessor.GetObservationPoints();


                return res.ToArray();
            }
        }

        public static bool SaveObservationPoint(ObservationPoint observPoint)
        {
            using (var accessor = new SemanticDataAccess())
            {
                var res = accessor.SaveObservationPoint(observPoint);
                return res;                
            }
        }
    }
}
