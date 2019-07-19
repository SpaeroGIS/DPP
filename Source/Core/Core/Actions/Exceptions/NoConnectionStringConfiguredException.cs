namespace MilSpace.Core.Actions.Exceptions
{
    internal class NoConnectionStringConfiguredException : SpaeroException
    {

        internal NoConnectionStringConfiguredException()
            : base()
        {
            errorMessage = "No connection strings configured";
        }
    }
}
