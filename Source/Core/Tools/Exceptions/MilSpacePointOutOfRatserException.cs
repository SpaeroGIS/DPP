using MilSpace.Core;
using MilSpace.Core.Exceptions;

namespace MilSpace.Tools.Exceptions
{
   public class MilSpacePointOutOfRatserException : MilSpaceException
    {
        private static string messageTempleate = "Point {0} doesn`t locate on the raster layer {1}";

        public MilSpacePointOutOfRatserException(int pointId, string rasterLayerName)
        {
            errorMessage = messageTempleate.InvariantFormat(pointId, rasterLayerName);
        }
    }
}
