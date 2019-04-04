using MilSpace.DataAccess.DataTransfer;
using System.Collections.Generic;

namespace MilSpace.DataAccess.Facade
{
    public static class MilSpaceProfileFacade
    {
        public static bool SaveProfileSession(ProfileSession session)
        {
            using (var accessor = new SemanticDataAccess())
            {
                return accessor.SaveSession(session);
            }
        }

        public static IEnumerable<ProfileSession> GetAllProfileSessions()
        {
            using (var accessor = new SemanticDataAccess())
            {
                return accessor.GetAllSessions();
            }
        }

        public static ProfileSession GetProfileSessionById(int sessionId)
        {
            using (var accessor = new SemanticDataAccess())
            {
                return accessor.GetSessionById(sessionId);
            }
        }
    }

}
