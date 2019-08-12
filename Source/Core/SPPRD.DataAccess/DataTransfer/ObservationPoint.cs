using System;

namespace MilSpace.DataAccess.DataTransfer
{
    public class ObservationPoint
    {
        public int Objectid { get; internal set; }

        public string Id;

         public string Title;

         public string Type;

         public string Group;

         public string Affiliation;

         public int? Share;

         public double? X;

         public double? Y;

         public double? RelativeHeihgt;

         public double? AzimuthB;

         public double? AzimuthE;

         public double? AngelMinH;

         public double? AngelMaxH;

         public double? AzimuthMainAxis;

         public double? AngelFrameH;

         public double? AngelFrameV;

         public double? AngelCameraRotationH;

         public double? AngelCameraRotationV;

         public DateTime? Dto;

         public string Operator;
    }
}
