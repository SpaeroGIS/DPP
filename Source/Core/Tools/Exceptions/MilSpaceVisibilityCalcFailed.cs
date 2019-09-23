using MilSpace.Core;
using MilSpace.Core.Exceptions;

namespace MilSpace.Tools.Exceptions
{
    public class MilSpaceVisibilityCalcFailedException : MilSpaceException
    {
        private static string messageTempleate = "There was the error on calculating visibility";

        public MilSpaceVisibilityCalcFailedException() 
        {
            errorMessage = messageTempleate;
        }
    }
}
