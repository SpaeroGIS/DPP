using System;

namespace MilSpace.DataAccess.DataTransfer
{
   public class ObservPointArgs
   {
        public string Title { get; set; }
        public ObservationPointMobilityTypesEnum Type { get; set; }
        public ObservationPointTypesEnum Affiliation { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double RelativeHeight { get; set; }
        public double AzimuthB  { get; set; }
        public double AzimuthE { get; set; }
        public double AzimuthMainAxis { get; set; }
        public double AngelMinH { get; set; }
        public double AngelMaxH { get; set; }
        public DateTime Dto { get; set; }
        public string Operator { get; set; }
    }
}
