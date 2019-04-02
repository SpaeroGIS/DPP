using MilSpace.Core;
using MilSpace.Core.Exceptions;

namespace MilSpace.Profile
{
    public class ProfileSessionNotCreated : MilSpaceException
    {
        private static string messageTempleate = "The session {0} was not created. Error message:\n {1}";

        public ProfileSessionNotCreated(string sessionNumber, string errorMessage)
        {
            errorMessage = messageTempleate.InvariantFormat(sessionNumber, errorMessage);
        }
    }
}
