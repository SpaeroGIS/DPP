namespace MilSpace.Core.Exceptions
{
    public class MilSpaceProfileLackOfParameterException : MilSpaceException
    {
        private static string messageTempleate = "The parameter {0} cannot have value {1}";

        public MilSpaceProfileLackOfParameterException( string parameterName, int value)
        {
            errorMessage = messageTempleate.InvariantFormat(parameterName, value);
        }
        public MilSpaceProfileLackOfParameterException(string parameterName, double value)
        {
            errorMessage = messageTempleate.InvariantFormat(parameterName, value);
        }

        public MilSpaceProfileLackOfParameterException(string parameterName, string value)
        {
            errorMessage = messageTempleate.InvariantFormat(parameterName, value);
        }

    }
}
