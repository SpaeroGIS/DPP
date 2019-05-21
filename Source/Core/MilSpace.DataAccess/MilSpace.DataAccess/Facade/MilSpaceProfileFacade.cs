using MilSpace.DataAccess.DataTransfer;
using System.Collections.Generic;
using System.Linq;

namespace MilSpace.DataAccess.Facade
{
    public static class MilSpaceProfileFacade
    {
        public static bool SaveProfileSession(ProfileSession session)
        {
            using (var accessor = new SemanticDataAccess())
            {
                return accessor.SaveProfileSession(session);
            }
        }

        public static IEnumerable<ProfileSession> GetUserProfileSessions()
        {
            using (var accessor = new SemanticDataAccess())
            {
                return accessor.GetAllSessionsFoUser().ToArray();
            }
        }

        public static ProfileSession GetProfileSessionById(int sessionId)
        {
            using (var accessor = new SemanticDataAccess())
            {
                return accessor.GetProfileSessionById(sessionId);
            }
        }

        public static bool DeleteUserSessions(int sessionId)
        {
            using (var accessor = new SemanticDataAccess())
            {
                return accessor.DeleteUserSession(sessionId);
            }
        }

        public static bool CanEraseProfileSessions(int sessionId)
        {
            using (var accessor = new SemanticDataAccess())
            {
                return accessor.CanEraseProfileSession(sessionId);
            }
        }
        public static bool EraseProfileSessions(int sessionId)
        {
            using (var accessor = new SemanticDataAccess())
            {
                return accessor.EraseProfileSession(sessionId);
            }
        }
    }

}
