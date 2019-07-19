using System;
using System.Collections.Generic;
using System.Linq;
using MilSpace.Core.DataAccess;

namespace MilSpace.Core.Actions.Exceptions
{
    public class SpaeroDataException : SpaeroException
    {
        private static string messageTempleate = "Cannot perform operation {0} on {1}";

        public SpaeroDataException(Exception ex) : this("databese", DataOperationsEnum.Access, ex)
        {
        }
        public SpaeroDataException(DataOperationsEnum operation, Exception ex) : this("databese", operation, ex)
        {
        }
        public SpaeroDataException(string dataObject, DataOperationsEnum operation, Exception ex) : base(ex)
        {
            errorMessage = messageTempleate.InvariantFormat(operation, dataObject);
        }

    }
}
