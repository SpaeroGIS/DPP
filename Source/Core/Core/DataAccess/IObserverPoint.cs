using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Core.DataAccess
{
    public interface IObserverPoint
    {
        int Objectid { get; set; }

        string Title { get; set; }
        double? X { get; set; }

        double? Y { get; set; }

        /// <summary>
        /// Height above surface, default - 0
        /// Cannot be less then 0.
        /// </summary>
        double? RelativeHeight { get; set; }

        /// <summary>
        /// This value defines the start angle of the horizontal scan range. The value should be specified in degrees from 0 to 360, 
        ///with 0 oriented to north. The default value is 0.
        ///It can be a field in the input observer features dataset or a numerical value.By default, a numerical field 
        ///AZIMUTH1 is used if it exists in the input observer features attribute table.
        ///You may overwrite it by specifying another numerical field or constant.
        ///Shold be used as the parameter horizontal_start_angle in the Visibility calculation
        ///Deafult value - 0. Can not be more less then 0 and more 360 
        /// </summary>
        double? AzimuthStart { get; set; }

        /// <summary>
        /// This value defines the end angle of the horizontal scan range. The value should be specified in degrees from 0 to 360, 
        /// with 0 oriented to north. The default value is 360.
        /// It can be a field in the input observer features dataset or a numerical value. By default, a numerical field 
        /// AZIMUTH2 is used if it exists in the input observer features attribute table. 
        /// You may overwrite it by specifying another numerical field or constant.
        ///Shold be used as the parameter horizontal_end_angle in the Visibility calculation
        ///Deafult value - 360. Can not be more less then 0 and more 360 
        /// </summary>
        double? AzimuthEnd { get; set; }

        /// <summary>
        /// This value defines the upper vertical angle limit of the scan above a horizontal plane.
        /// The value should be specified in degrees from 0 to 90, which can be integer or floating point.
        /// It can be a field in the input observer features dataset or a numerical value. By default, a numerical field VERT1 is used 
        /// if it exists in the input observer features attribute table. You may overwrite it by specifying another numerical field or constant.
        /// Shold be used as the parameter vertical_upper_angle in the Visibility calculation
        /// Default value - 0
        /// </summary>
        double? AngelMaxH { get; set; }

        /// <summary>
        /// This value defines the lower vertical angle limit of the scan below a horizontal plane. 
        /// The value should be specified in degrees from -90 to 0, which can be integer or floating point.
        ///It can be a field in the input observer features dataset or a numerical value.By default, a numerical field VERT2 is used 
        ///if it exists in the input observer features attribute table.You may overwrite it by specifying another numerical field or constant.
        /// Shold be used as the parameter vertical_lower_angle in the Visibility calculation. 
        /// Default value - 90
        /// </summary>
         double? AngelMinH { get; set; }
         double? InnerRadius { get; set; }
         double? OuterRadius { get; set; }
    }
}
