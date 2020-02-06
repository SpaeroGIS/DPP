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
                throw new NotImplementedException(); 
            }
        }
    }
}
