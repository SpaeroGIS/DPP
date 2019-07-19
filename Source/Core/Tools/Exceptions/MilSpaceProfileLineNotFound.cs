using MilSpace.Core;
using MilSpace.Core.Exceptions;

namespace MilSpace.Tools.Exceptions
{
    public class MilSpaceProfileLineNotFound : MilSpaceException
    {
        private static string messageTempleate = "The line with OID {0} was not found found in the  {1}";

        public MilSpaceProfileLineNotFound(int lineObjectId, string featureClassname) 
        {
            errorMessage = messageTempleate.InvariantFormat(lineObjectId, featureClassname);
        }
    }
}
