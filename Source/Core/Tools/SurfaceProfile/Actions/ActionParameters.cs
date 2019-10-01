using MilSpace.Core.Actions;

namespace MilSpace.Tools.SurfaceProfile.Actions
{
    public class ActionParameters : ActionParamNamesCore
    {
        /// <summary>
        /// The source DEM which is used for calculation
        /// </summary>
        public static string ProfileSource = "prfs";
        public static string ProfileOutTable = "prfot";
        public static string OutputSourceName = "outsrcname";
        public static string FilteringIds = "filterringIds";
    }
}
