using MilSpace.Configurations;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.Definition;
using MilSpace.DataAccess.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.DataAccess.Facade
{
    internal class VisibilityDataAccess : DataAccessor<MilSpaceVisibilityContext>, IDisposable
    {
        public override string ConnectionString => MilSpaceConfiguration.ConnectionProperty.WorkingDBConnection;

        public bool SaveVisibilitySession(VisibilitySession visibilitySession)
        {
            try
            {
                if (!context.MilSp_VisibilitySessions.Any(session => session.Id == visibilitySession.Id))
                {
                    var sessionEntity = visibilitySession.Get();
                    context.MilSp_VisibilitySessions.InsertOnSubmit(sessionEntity);

                    Submit();
                    return true;
                }

                log.WarnEx($"Session with the same id already exists");
            }
            catch (MilSpaceDataException ex)
            {
                log.WarnEx(ex.Message);

                if (ex.InnerException != null)
                {
                    log.WarnEx(ex.InnerException.Message);
                }
            }
            catch (Exception ex)
            {
                log.WarnEx($"Unexpected exception:{ex.Message}");
            }

            return false;
        }

        public bool UpdateVisibilitySession(VisibilitySession visibilitySession)
        {
            try
            {
                var sessionEntity = context.MilSp_VisibilitySessions.FirstOrDefault(session => session.Id == visibilitySession.Id);

                if (sessionEntity != null)
                {
                    sessionEntity.Update(visibilitySession);

                    Submit();
                    return true;
                }

                log.WarnEx($"Session not found");
            }
            catch (MilSpaceDataException ex)
            {
                log.WarnEx(ex.Message);

                if (ex.InnerException != null)
                {
                    log.WarnEx(ex.InnerException.Message);
                }
            }
            catch (Exception ex)
            {
                log.WarnEx($"Unexpected exception:{ex.Message}");
            }

            return false;
        }

        public bool DeleteVisibilitySession(string id)
        {
            try
            {
                var sessionEntity = context.MilSp_VisibilitySessions.FirstOrDefault(session => session.Id == id);

                if (sessionEntity != null)
                {
                    context.MilSp_VisibilitySessions.DeleteOnSubmit(sessionEntity);

                    Submit();
                    return true;
                }

                log.WarnEx($"Session not found");
            }
            catch (Exception ex)
            {
                log.WarnEx($"Unexpected exception:{ex.Message}");
            }

            return false;
        }

        public IEnumerable<VisibilitySession> GetAllVisibilitySessions()
        {
            try
            {
                var sessions = context.MilSp_VisibilitySessions.Select(s => s.Get());
                return sessions;
            }
            catch (Exception ex)
            {
                log.WarnEx($"Unexpected exception:{ex.Message}");
            }

            return null;
        }

        public IEnumerable<ObservationPoint> GetObservationPoints()
        {
            try
            {
                var result = context.VisiblilityObservPoints.Select(op => op.Get());
                log.InfoEx($"Get all Observation point ({result.Count()}). user {Environment.UserName}");
                return result;
            }
            catch (Exception ex)
            {
                log.WarnEx($"Unexpected exception:{ex.Message}");
            }
            return null;
        }

        public bool SaveObservationPoint(ObservationPoint observPoint)
        {
            bool result = true;
            try
            {
                var bdObservPoint = context.VisiblilityObservPoints.FirstOrDefault(p => p.OBJECTID == observPoint.Objectid);
                if (bdObservPoint != null)
                {
                    bdObservPoint.Update(observPoint);
                    if (Submit())
                    {
                        log.InfoEx($"Observation Point Row with ObjectId '{observPoint.Objectid}' was saved");
                    }
                }
            }
            catch (MilSpaceDataException ex)
            {
                log.WarnEx(ex.Message);
                if (ex.InnerException != null)
                {
                    log.WarnEx(ex.InnerException.Message);
                }

                result = false;
            }
            catch (Exception ex)
            {
                log.WarnEx($"Unexpected exception:{ex.Message}");
                result = false;
            }
            return result;

        }

        public IEnumerable<ObservationObject> GetAllObservationObjectsByUser()
        {
            try
            {
                var result = context.VisiblilityObservationObjects.Where(oo => oo.soper == Environment.UserName).Select(op => op.Get() );
                log.InfoEx($"Get all Observation objefcts ({result.Count()}). user {Environment.UserName}");
                return result;
            }
            catch (Exception ex)
            {
                log.WarnEx($"Unexpected exception:{ex.Message}");
            }
            return null;
        }

    }
}
