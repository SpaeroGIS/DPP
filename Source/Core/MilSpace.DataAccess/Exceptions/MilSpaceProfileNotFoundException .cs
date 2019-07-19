using MilSpace.Core;
using MilSpace.Core.Exceptions;

namespace MilSpace.DataAccess.Exceptions
{

    public class MilSpaceProfileNotFoundException : MilSpaceException
    {
        private static string messageTempleate = "profile Id: {0} ";


        public MilSpaceProfileNotFoundException(int profileId) : base()
        {
            errorMessage = messageTempleate.InvariantFormat(profileId);
        }
    }
}
