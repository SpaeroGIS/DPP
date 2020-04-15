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
        internal GeoCalculatorDataAccess()
        {
            log.InfoEx(
                $"Initialise GeoCalculatorDataAccess with connection: " +
                $"{MilSpaceConfiguration.ConnectionProperty.WorkingDBConnection}"
                );
        }

        public override string ConnectionString => MilSpaceConfiguration.ConnectionProperty.WorkingDBConnection;

        public IEnumerable<GeoCalcPoint> GetUserPoints()
        {
            try
            {
                var sessions = context.GeoCalcSessionPoints.Where(s => s.userName.Equals(Environment.UserName)).OrderBy(t => t.PointNumber);
                return sessions.Select(s => s.Get());
            }
            catch (Exception ex)
            {
                log.WarnEx($"Unexpected exception:{ex.Message}");
            }

            return null;
        }

        public void UpdateUserPoints(IEnumerable<GeoCalcPoint> points)
        {
            log.InfoEx($"UpdateUserPoints. Count {points.Count()}");

            foreach (var point in points)
            {
                try
                {
                    log.InfoEx($"UpdateUserPoints. Processing Id: {point.Id}");
                    var pointEntity = context.GeoCalcSessionPoints.FirstOrDefault(entity => entity.id == point.Id);

                    if(pointEntity != null)
                    {
                        pointEntity.Update(point);
                        Submit();

                        log.InfoEx($"GeoCalcPoint {point.Id} was successfully updated");
                    }
                    else
                    {
                        pointEntity = point.Get();
                        context.GeoCalcSessionPoints.InsertOnSubmit(pointEntity);
                        Submit();

                        log.InfoEx($"GeoCalcPoint {point.Id} was successfully added");
                    }
                }
                catch(Exception ex)
                {
                    log.WarnEx($"Unexpected exception:{ex.Message}");
                }
            }
        }

        public void SaveUserPoints(IEnumerable<GeoCalcPoint> points)
        {
            try
            {
                ClearUserPoints();

                context.GeoCalcSessionPoints.InsertAllOnSubmit(points.Select(point => point.Get()));
                Submit();

                log.WarnEx("GeoCalcPoints was successfully saved");
            }
            catch(Exception ex)
            {
                log.WarnEx($"Unexpected exception:{ex.Message}");
            }
        }

        public void DeleteUserPoint(Guid id)
        {
            try
            {
                var pointEntity = context.GeoCalcSessionPoints.FirstOrDefault(entity => entity.id == id);

                if(pointEntity == null)
                {
                    log.WarnEx($"GeoCalc point with id {id} was not found");
                    return;
                }

                context.GeoCalcSessionPoints.DeleteOnSubmit(pointEntity);
                Submit();

                log.InfoEx($"GeoCalcPoint {id} was removed");
            }
            catch(Exception ex)
            {
                log.WarnEx($"Unexpected exception:{ex.Message}");
            }
        }

        public void ClearUserPoints()
        {
            try
            {
                var userPoints = context.GeoCalcSessionPoints.Where(s => s.userName.Equals(Environment.UserName)).ToArray();
                context.GeoCalcSessionPoints.DeleteAllOnSubmit(userPoints);
                Submit();

                log.InfoEx($"User session was cleared");
            }
            catch(Exception ex)
            {
                log.WarnEx($"Unexpected exception:{ex.Message}");
            }
        }
    }
}
