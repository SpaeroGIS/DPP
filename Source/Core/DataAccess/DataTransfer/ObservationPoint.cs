using System;
using System.Collections.Generic;

namespace MilSpace.DataAccess.DataTransfer
{
    [Flags]
    public enum VeluableObservPointFieldsEnum : byte
    {
        All = 0,
        Type = 1,
        Affiliation = 2,
        Date = 4
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

        /// <summary>
        /// Height above surface, default - 0
        /// Cannot be less then 0.
        /// </summary>
        public double? RelativeHeight;

        public double AvailableHeightLover;
        public double AvailableHeightUpper;

        /// <summary>
        /// This value defines the start angle of the horizontal scan range. The value should be specified in degrees from 0 to 360, 
        ///with 0 oriented to north. The default value is 0.
        ///It can be a field in the input observer features dataset or a numerical value.By default, a numerical field 
        ///AZIMUTH1 is used if it exists in the input observer features attribute table.
        ///You may overwrite it by specifying another numerical field or constant.
        ///Shold be used as the parameter horizontal_start_angle in the Visibility calculation
        ///Deafult value - 0. Can not be more less then 0 and more 360 
        /// </summary>
        public double? AzimuthStart;

        /// <summary>
        /// This value defines the end angle of the horizontal scan range. The value should be specified in degrees from 0 to 360, 
        /// with 0 oriented to north. The default value is 360.
        /// It can be a field in the input observer features dataset or a numerical value. By default, a numerical field 
        /// AZIMUTH2 is used if it exists in the input observer features attribute table. 
        /// You may overwrite it by specifying another numerical field or constant.
        ///Shold be used as the parameter horizontal_end_angle in the Visibility calculation
        ///Deafult value - 360. Can not be more less then 0 and more 360 
        /// </summary>
        public double? AzimuthEnd;

        /// <summary>
        /// This value defines the upper vertical angle limit of the scan above a horizontal plane.
        /// The value should be specified in degrees from 0 to 90, which can be integer or floating point.
        /// It can be a field in the input observer features dataset or a numerical value. By default, a numerical field VERT1 is used 
        /// if it exists in the input observer features attribute table. You may overwrite it by specifying another numerical field or constant.
        /// Shold be used as the parameter vertical_upper_angle in the Visibility calculation
        /// Default value - 0
        /// </summary>
        public double? AngelMaxH;

        /// <summary>
        /// This value defines the lower vertical angle limit of the scan below a horizontal plane. 
        /// The value should be specified in degrees from -90 to 0, which can be integer or floating point.
        ///It can be a field in the input observer features dataset or a numerical value.By default, a numerical field VERT2 is used 
        ///if it exists in the input observer features attribute table.You may overwrite it by specifying another numerical field or constant.
        /// Shold be used as the parameter vertical_lower_angle in the Visibility calculation. 
        /// Default value - 90
        /// </summary>
        public double? AngelMinH;

        /// <summary>
        /// The main direct of the observation point/station. 
        /// Can be used for rectification of the horisontal angels in Visibility calculation
        /// </summary>
        public double? AzimuthMainAxis;

        /// <summary>
        /// Horisontal angel of the frame in an equipment
        /// Can be used for rectification of the horisontal angels in Visibility calculation
        /// Default value - 0
        /// </summary>
        public double? AngelFrameH;

        /// <summary>
        /// Vertiacl angel of the frame in an equipment
        /// Can be used for rectification of the horisontal angels in Visibility calculation
        /// </summary>
        public double? AngelFrameV;

        /// <summary>
        /// The horizontal ange of attacment of the camera(equipment) relative to the main axis
        /// Can be used for rectification of the horisontal angels in Visibility calculation
        /// </summary>
        public double? AngelCameraRotationH;

        /// <summary>
        /// The Vertical ange of attacment of the camera(equipment) relative to the main axis
        /// Can be used for rectification of the horisontal angels in Visibility calculation
        /// </summary>
        public double? AngelCameraRotationV;

        public ObservationPointTypesEnum ObservationPointType
        {
            get
            {
                ObservationPointTypesEnum temp = ObservationPointTypesEnum.Undefined;
                Enum.TryParse(Affiliation, out temp);
                return temp;
            }
        }


        public double? InnerRadius;
        public double? OuterRadius;
        /// <summary>
        /// The date and time when the item was created
        /// </summary>
        public DateTime? Dto;

        /// <summary>
        /// The date and time when the item was created
        /// </summary>
        public string Operator;

        public string GetItemValue(VeluableObservPointFieldsEnum filter)
        {
            var values = new List<string>();

            values.Add(Title);

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
