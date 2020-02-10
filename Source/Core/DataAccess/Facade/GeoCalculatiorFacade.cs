using MilSpace.DataAccess.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MilSpace.DataAccess.Facade
{
    public static class GeoCalculatiorFacade
    {
        public static IEnumerable<GeoCalcPoint> GetUserSessionPoints()
        {
            using (var accessor = new GeoCalculatorDataAccess())
            {
                var res = accessor.GetUserPoints();
                return res.ToArray();
            }
        }

        public static void SaveUserSessionPoints(IEnumerable<GeoCalcPoint> points)
        {
            using (var accessor = new GeoCalculatorDataAccess())
            {
                accessor.SaveUserPoints(points);
            }
        }

        public static void UpdateUserSessionPoints(IEnumerable<GeoCalcPoint> points)
        {
            using(var accessor = new GeoCalculatorDataAccess())
            {
                accessor.UpdateUserPoints(points);
            }
        }

        public static void DeleteUserSessionPoint(Guid pointId)
        {
            using(var accessor = new GeoCalculatorDataAccess())
            {
                accessor.DeleteUserPoint(pointId);
            }
        }

        public static void ClearUserSessionPoints()
        {
            using(var accessor = new GeoCalculatorDataAccess())
            {
                accessor.ClearUserPoints();
            }
        }
    }
}
