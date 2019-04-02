using MilSpace.Configurations;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.Definition;
using MilSpace.DataAccess.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MilSpace.DataAccess.Facade
{
    internal class SemanticDataAccess : DataAccessor<MilSpaceStorageContext>, IDisposable
    {
        public override string ConnectionString => MilSpaceConfiguration.ConnectionProperty.WorkingDBConnection;


        public bool SaveSession(ProfileSession session)
        {
            bool result = true;
            try
            {
                var profile = context.MilSp_Profiles.FirstOrDefault(p => p.idRow == session.SessionId);
                if (profile != null)
                {
                    profile.ProfileData = session.Serialized;
                }
                else
                {
                    profile = session.Get();
                    context.MilSp_Profiles.InsertOnSubmit(profile);
                }

                Submit();
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

        public ProfileSession GetSessionById(int sessionId)
        {
            try
            {
                var session = context.MilSp_Profiles.FirstOrDefault(s => s.idRow == sessionId);
                if (session != null)
                {
                    return session.Get();
                }

            }
            catch (Exception ex)
            {
                log.WarnEx($"Unexpected exception:{ex.Message}");
            }

            return null;
        }

        public IEnumerable<ProfileSession> GetAllSessions()
        {
            try
            {
                return context.MilSp_Profiles.Select(s => s.Get()).ToArray();
            }
            catch (Exception ex)
            {
                log.WarnEx($"Unexpected exception:{ex.Message}");
            }

            return null;
        }


    }
}
