using MilSpace.Configurations;
using MilSpace.Core.DataAccess;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.Definition;
using MilSpace.DataAccess.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MilSpace.DataAccess.Facade
{
    internal class VisibilityDataAccess : DataAccessor<MilSpaceVisibilityContext>, IDisposable
    {
        public override string ConnectionString => MilSpaceConfiguration.ConnectionProperty.WorkingDBConnection;

        public VisibilityTask AddVisibilityTask(VisibilityTask visibilitySession)
        {
            try
            {
                if (!context.MilSp_VisibilityTasks.Any(session => session.Id == visibilitySession.Id))
                {

                    var sessionEntity = visibilitySession.Get();
                    context.MilSp_VisibilityTasks.InsertOnSubmit(sessionEntity);
                    Submit();
                    log.InfoEx($"Session {visibilitySession.Id} was successfully added");
                }
                else
                {
                    log.WarnEx($"Session {visibilitySession.Id} cannot added because of it already exists");
                }


                return context.MilSp_VisibilityTasks.First(session => session.Id == visibilitySession.Id).Get();
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

        public VisibilityTask UpdateVisibilityTask(VisibilityTask visibilitySession)
        {
            try
            {
                var sessionEntity = context.MilSp_VisibilityTasks.FirstOrDefault(session => session.Id.Trim() == visibilitySession.Id);

                if (sessionEntity != null)
                {
                    sessionEntity.Update(visibilitySession);

                    Submit();
                    log.InfoEx($"Session {visibilitySession.Id} was successfully updated");
                    return context.MilSp_VisibilityTasks.First(session => session.Id.Trim() == visibilitySession.Id).Get();
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

        public bool DeleteVisibilityTask(string id)
        {
            try
            {
                var sessionEntity = context.MilSp_VisibilityTasks.FirstOrDefault(session => session.Id.Trim() == id);

                if (sessionEntity != null)
                {
                    context.MilSp_VisibilityTasks.DeleteOnSubmit(sessionEntity);

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

        public IEnumerable<VisibilityTask> GetAllVisibilityTasks(bool finished = false)
        {
            try
            {
                var sessions = finished ? context.MilSp_VisibilityTasks.Where(s => s.Finished.HasValue) : context.MilSp_VisibilityTasks;
                return sessions.Select(s => s.Get());
            }
            catch (Exception ex)
            {
                log.WarnEx($"Unexpected exception:{ex.Message}");
            }

            return null;
        }

        public IEnumerable<VisibilityTask> GetUserVisibilityTasks()
        {
            try
            {
                var sessions = context.MilSp_VisibilityTasks.Where(s => s.UserName.Equals(Environment.UserName));
                return sessions.Select(s => s.Get());
            }
            catch (Exception ex)
            {
                log.WarnEx($"Unexpected exception:{ex.Message}");
            }

            return null;
        }

        public VisibilityCalcResults AddVisibilityResults(VisibilityCalcResults visibilityResults)
        {
            try
            {
                if (!context.MilSp_VisiblityResults.Any(res => res.Id == visibilityResults.Id))
                {
                    var resultsEntity = visibilityResults.Get();
                    context.MilSp_VisiblityResults.InsertOnSubmit(resultsEntity);
                    if (!AddResultToUserSession(visibilityResults))
                    {
                        throw new MilSpaceDataException("VisibilityUserSession", DataOperationsEnum.Insert);
                    }
                    Submit();
                    log.InfoEx($"Session {visibilityResults.Id} was successfully added");
                }
                else
                {
                    log.WarnEx($"Session {visibilityResults.Id} cannot added because of it already exists");
                }


                return context.MilSp_VisiblityResults.First(session => session.Id == visibilityResults.Id).Get();
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

        public VisibilityCalcResults UpdateVisibilityResults(VisibilityCalcResults visibilityResults)
        {
            try
            {
                var resultsEntity = context.MilSp_VisiblityResults.FirstOrDefault(res => res.Id.Trim() == visibilityResults.Id);

                if (resultsEntity != null)
                {
                    resultsEntity.Update(visibilityResults);

                    Submit();
                    log.InfoEx($"Visibility results {visibilityResults.Id} was successfully updated");
                    return context.MilSp_VisiblityResults.First(session => session.Id.Trim() == visibilityResults.Id).Get();
                }

                log.WarnEx($"Visibility results {visibilityResults.Id} not found");
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

        public bool AddShareddResultsToUserSession(VisibilityCalcResults visibilityResults)
        {
            visibilityResults.UserName = Environment.UserName;
            return AddResultToUserSession(visibilityResults);
        }

        private bool AddResultToUserSession(VisibilityCalcResults visibilityResults)
        {
            try
            {
                if (!context.MilSp_VisibilityUserSessions.Any(r => r.userName == visibilityResults.UserName && r.visibilityResultId == visibilityResults.Id))
                {
                    var sessionValue = new MilSp_VisibilityUserSession { userName = visibilityResults.UserName, visibilityResultId = visibilityResults.Id};
                    context.MilSp_VisibilityUserSessions.InsertOnSubmit(sessionValue);
                    Submit();
                }
                return true;
            }

            catch (Exception ex)
            {
                log.WarnEx($"Unexpected exception:{ex.Message}");
            }

            return false;
        }
        public bool DeleteVisibilityResults(string id)
        {
            try
            {
                var resultEntity = context.MilSp_VisiblityResults.FirstOrDefault(res => res.Id.Trim() == id);

                if (resultEntity != null)
                {
                    context.MilSp_VisiblityResults.DeleteOnSubmit(resultEntity);

                    Submit();

                    if(!DeleteVisibilityResultsFromAllUsersSessions(id))
                    {
                        return false;
                    }

                    return true;
                }

                log.WarnEx($"Visibility results not found");
            }
            catch (Exception ex)
            {
                log.WarnEx($"Unexpected exception:{ex.Message}");
            }

            return false;
        }

        public bool DeleteVisibilityResultsFromAllUsersSessions(string id)
        {
            try
            {
                var resultEntity = context.MilSp_VisibilityUserSessions.Where(res => res.visibilityResultId.Trim() == id).ToArray();

                if(resultEntity != null)
                {
                    foreach(var entity in resultEntity)
                    {
                        context.MilSp_VisibilityUserSessions.DeleteOnSubmit(entity);

                        Submit();
                    }
                    return true;
                }

                log.WarnEx($"Visibility results not found");
            }
            catch(Exception ex)
            {
                log.WarnEx($"Unexpected exception:{ex.Message}");
            }

            return false;
        }

        public bool DeleteVisibilityResultsFromUserSession(string id)
        {
            try
            {
                var resultEntity = context.MilSp_VisibilityUserSessions.FirstOrDefault(res => res.visibilityResultId.Trim() == id && res.userName.Trim().Equals(Environment.UserName));

                if(resultEntity != null)
                {
                    context.MilSp_VisibilityUserSessions.DeleteOnSubmit(resultEntity);

                    Submit();
                    return true;
                }

                log.WarnEx($"Visibility results not found");
            }
            catch(Exception ex)
            {
                log.WarnEx($"Unexpected exception:{ex.Message}");
            }

            return false;
        }

        public bool IsResultsBelongToUser(string id)
        {
            try
            {
                var resultEntity = context.MilSp_VisiblityResults.FirstOrDefault(res => res.Id.Trim() == id);

                if(resultEntity != null)
                {
                    return resultEntity.UserName.Trim().Equals(Environment.UserName);
                }

                log.WarnEx($"Visibility results not found");
            }
            catch(Exception ex)
            {
                log.WarnEx($"Unexpected exception:{ex.Message}");
            }

            return false;
        }

        public IEnumerable<VisibilityCalcResults> GetAllVisibilityResults(bool onlyUsersResults)
        {
            try
            {
                IEnumerable<MilSp_VisiblityResults> results = null;

                if (onlyUsersResults)
                {
                    results = context.MilSp_VisibilityUserSessions.Where(s => s.userName.Trim().Equals(Environment.UserName)).Select(r => r.MilSp_VisiblityResults);
                }
                else
                {
                    results = context.MilSp_VisiblityResults.Where(r => r.UserName.Trim().Equals(Environment.UserName) || r.shared);
                }
                        
                return results.Select(s => s.Get());
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

        public bool SaveObservationObject(ObservationObject observObject)
        {
            bool result = true;
            try
            {
                var bdObservObject = context.VisiblilityObservationObjects.FirstOrDefault(p => p.OBJECTID == observObject.ObjectId);
                if (bdObservObject != null)
                {
                    bdObservObject.Update(observObject);
                    if (Submit())
                    {
                        log.InfoEx($"Observation Point Row with ObjectId '{observObject.ObjectId}' was saved");
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
