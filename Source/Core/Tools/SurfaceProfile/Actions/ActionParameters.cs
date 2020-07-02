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
        public static string FilteringPointsIds = "PntfltrIds";
        public static string FilteringStationsIds = "StnsfltrIds";
        public static string Calculationresults = "calcresults";
        public static string Session = "session";
        public static string VisibilityPercent = "visibilityPercent";
        public static string ShowAllResults = "showAllResults";
    }
}
