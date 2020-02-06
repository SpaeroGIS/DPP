using MilSpace.Configurations;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.DataAccess.Facade
{
    internal class GeoCalculatorDataAccess : DataAccessor<MilSpaceGeoCalcContext>, IDisposable
    {
        public override string ConnectionString => MilSpaceConfiguration.ConnectionProperty.WorkingDBConnection;

        public IEnumerable<GeoCalcPoint> GetUserPoints()
        {
            try
            {
                var sessions = context.GeoCalcSessionPoints.Where(s => s.userName.Equals(Environment.UserName)).OrderByDescending(t => t.PointNumber);
                return sessions.Select(s => s.Get());
            }
            catch (Exception ex)
            {
                log.WarnEx($"Unexpected exception:{ex.Message}");
            }

            return null;
        }
    }
}
