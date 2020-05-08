using MilSpace.Core;
using MilSpace.DataAccess.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MilSpace.DataAccess.Facade
{
    public static class GeoCalculatiorFacade
    {
        private static readonly Logger log = Logger.GetLoggerEx("GeoCalculatiorFacade");

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

            log.InfoEx("> UpdateUserSessionPoints START");
            if (points != null)
            {
                try
                {
                    using (var accessor = new GeoCalculatorDataAccess())
                    {
                        accessor.UpdateUserPoints(points);
                    }
                }
                catch(Exception ex)
                {
                    log.InfoEx("> UpdateUserSessionPoints Exception: {0}", ex.Message);
                }
            }
            else
            {
                log.InfoEx("UpdateUserSessionPoints points == null");
            }
            log.InfoEx("> UpdateUserSessionPoints END");
        }

        public static void UpdateUserSessionPoint(GeoCalcPoint point)
        {
            log.InfoEx("> UpdateUserSessionPoint START");
            if (point != null)
            {
                try
                {
                    using (var accessor = new GeoCalculatorDataAccess())
                    {
                        accessor.UpdateUserPoint(point);
                    }
                }
                catch (Exception ex)
                {
                    log.InfoEx("> UpdateUserSessionPoint Exception: {0}", ex.Message);
                }
            }
            else
            {
                log.InfoEx("UpdateUserSessionPoint point == null");
            }
            log.InfoEx("> UpdateUserSessionPoint END");
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
