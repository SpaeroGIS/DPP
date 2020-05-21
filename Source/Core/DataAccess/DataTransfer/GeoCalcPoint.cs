using MilSpace.Core.DataAccess;
using System;

namespace MilSpace.DataAccess.DataTransfer
{
    public class GeoCalcPoint : IObserverPoint
    {
        public Guid GuidId;
        public short PointNumber;
        public string Title
        {
            get
            {
                return PointNumber.ToString();
            }

            set { }
        }
        public int Objectid
        {
            get
            {
                return PointNumber;
            }

            set { }
        }

        public double? X { get; set; }
        public double? Y { get; set; }

        public string UserName;
        public double? RelativeHeight { get; set; }
        public double? AzimuthStart { get; set; }
        public double? AzimuthEnd { get; set; }
        public double? AngelMaxH { get; set; }
        public double? AngelMinH { get; set; }
        public double AzimuthMainAxis;
        public double AngFrameH;
        public double AnglFrameV;
        public double AnglCameraRotationH;
        public double AnglCameraRotationV;
        public double AvailableHeightUpper;
        public double AvailableHeightLover;
        public double InnerRadius;
        public double OuterRadius;
    }
}
