using MilSpace.Configurations;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.Definition;
using MilSpace.DataAccess.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MilSpace.DataAccess.Facade
{
    public static class VisibilityFacade
    {
        public static IEnumerable<ObservationPoint> GetAllObservationPoints()
        {
            using (var accessor = new SemanticDataAccess())
            {
                return accessor.GetObservationPoints().ToArray();
            }
        }

    }
}
