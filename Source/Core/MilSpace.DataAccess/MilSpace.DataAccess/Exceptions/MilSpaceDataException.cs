using MilSpace.Core;
using MilSpace.Core.DataAccess;
using MilSpace.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.DataAccess.Exceptions
{

    public class MilSpaceDataException : MilSpaceException
    {
        private static string messageTempleate = "Cannot perform operation {0} on {1}";

        public MilSpaceDataException(Exception ex) : this("databese", DataOperationsEnum.Access, ex)
        {
        }
        public MilSpaceDataException(DataOperationsEnum operation, Exception ex) : this("databese", operation, ex)
        {
        }
        public MilSpaceDataException(string dataObject, DataOperationsEnum operation, Exception ex) : base(ex)
        {
            errorMessage = messageTempleate.InvariantFormat(operation, dataObject);
        }

    }
}
