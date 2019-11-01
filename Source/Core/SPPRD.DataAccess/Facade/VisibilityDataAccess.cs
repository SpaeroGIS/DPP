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

        public VisibilitySession AddVisibilitySession(VisibilitySession visibilitySession)
        {
            try
            {
                if (!context.MilSp_VisibilitySessions.Any(session => session.Id == visibilitySession.Id))
                {

                    var sessionEntity = visibilitySession.Get();
                    context.MilSp_VisibilitySessions.InsertOnSubmit(sessionEntity);
                    Submit();
                    log.InfoEx($"Session {visibilitySession.Id} was successfully added");
                }
                else
                {
                    log.WarnEx($"Session {visibilitySession.Id} cannot added because of it already exists");
                }


                return context.MilSp_VisibilitySessions.First(session => session.Id == visibilitySession.Id).Get();
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

            return null;
        }

        public VisibilitySession UpdateVisibilitySession(VisibilitySession visibilitySession)
        {
            try
            {
                var sessionEntity = context.MilSp_VisibilitySessions.FirstOrDefault(session => session.Id == visibilitySession.Id);

                if (sessionEntity != null)
                {
                    sessionEntity.Update(visibilitySession);

                    Submit();
                    log.InfoEx($"Session {visibilitySession.Id} was successfully updated");
                    return context.MilSp_VisibilitySessions.First(session => session.Id == visibilitySession.Id).Get();
                }

                log.WarnEx($"Session {visibilitySession.Id} not found");
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

            return null; ;
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

        public IEnumerable<VisibilitySession> GetAllVisibilitySessions(bool finished = false)
        {
            try
            {
                var sessions = finished ? context.MilSp_VisibilitySessions.Where(s => s.Finished.HasValue) : context.MilSp_VisibilitySessions;
                return sessions.Select(s => s.Get());
            }
            catch (Exception ex)
            {
                log.WarnEx($"Unexpected exception:{ex.Message}");
            }

            return null;
        }

        public IEnumerable<VisibilitySession> GetUserVisibilitySessions()
        {
            try
            {
                var sessions = context.MilSp_VisibilitySessions.Where(s => s.UserName.Equals(Environment.UserName));
                return sessions.Select(s => s.Get());
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

        public IEnumerable<ObservationPoint> GetObservationPointsByIds(IEnumerable<int> ids)
        {

            IEnumerable<ObservationPoint> result = null;
            if (ids != null || ids.Count() > 0)
            {

                try
                {
                    //In case of performance issue reimplement it as context.ExecuteQuery with where clause OBJECTID == {ID1} OR OBJECTID == {ID2}.. 
                    result = context.VisiblilityObservPoints.Where(p => ids.Contains(p.OBJECTID)).Select(p => p.Get());
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
            }

            return result;
        }

        public IEnumerable<ObservationObject> GetAllObservationObjects()
        {
            try
            {
                var result = context.VisiblilityObservationObjects.Select(op => op.Get());
                log.InfoEx($"Get all Observation objefcts ({result.Count()}). user {Environment.UserName}");
                return result;
            }
            catch (Exception ex)
            {
                log.WarnEx($"Unexpected exception:{ex.Message}");
            }
            return null;
        }

        public IEnumerable<ObservationObject> GetObservationObjectByIds(IEnumerable<int> ids)
        {

            IEnumerable<ObservationObject> result = null;
            if (ids != null || ids.Count() > 0)
            {

                try
                {
                    //In case of performance issue reimplement it as context.ExecuteQuery with where clause OBJECTID == {ID1} OR OBJECTID == {ID2}.. 
                    result = context.VisiblilityObservationObjects.Where(p => ids.Contains(p.OBJECTID)).Select(p => p.Get());
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
            }

            return result;
        }

    }
}
