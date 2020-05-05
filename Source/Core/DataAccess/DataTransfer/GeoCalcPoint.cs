using System;

namespace MilSpace.DataAccess.DataTransfer
{
    public class GeoCalcPoint : ProfilePoint
    {
        public Guid Id;
        public short PointNumber;
        public string UserName;
        public double HRel;
        public double AzimuthB;
        public double AzimuthE;
        public double AnglMinH;
        public double AnglMaxH;
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
