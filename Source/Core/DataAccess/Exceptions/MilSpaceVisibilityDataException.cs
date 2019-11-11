using MilSpace.Core;
using MilSpace.Core.DataAccess;
using MilSpace.Core.Exceptions;
using System;

namespace MilSpace.DataAccess.Exceptions
{

    public class MilSpaceVisibilityDataException : MilSpaceException
    {
     
        public MilSpaceVisibilityDataException(string message) : base(message)
        {
        }
     
    }
}


