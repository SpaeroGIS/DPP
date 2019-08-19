using System;
using System.Collections.Generic;

namespace MilSpace.DataAccess.DataTransfer
{
    [Flags]
    public enum VeluableObservPointFieldsEnum : byte
    {
        All = 0,
        Id = 1,
        Type = 2,
        Affiliation = 4,
        Date = 8
    }

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

        public string GetItemValue(VeluableObservPointFieldsEnum filter)
        {
            var values = new List<string>();

            values.Add(Title);

            if ((filter & VeluableObservPointFieldsEnum.Id) == VeluableObservPointFieldsEnum.Id)
            {
                values.Add(Id);
            }
            if ((filter & VeluableObservPointFieldsEnum.Affiliation) == VeluableObservPointFieldsEnum.Affiliation)
            {
                values.Add(Affiliation);
            }
            if ((filter & VeluableObservPointFieldsEnum.Type) == VeluableObservPointFieldsEnum.Type)
            {
                values.Add(Type);
            }
            if ((filter & VeluableObservPointFieldsEnum.Date) == VeluableObservPointFieldsEnum.Date)
            {
                values.Add(Dto?.ToString("dd-MM-yyyy"));
            }

            return string.Join(" | ", values.ToArray());
        }
    }
}
