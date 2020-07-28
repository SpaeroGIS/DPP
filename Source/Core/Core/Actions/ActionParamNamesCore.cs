using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MilSpace.Core.Actions
{
    public partial class ActionParamNamesCore
    {
        /// <summary>
        /// Action idetity
        /// </summary>
        public const string Action = "a";

        /// <summary>
        /// Point to start
        /// </summary>
        public const string StartPoint = "ps";

        /// <summary>
        /// point x
        /// </summary>
        public const string PointX = "px";

        /// <summary>
        /// point y
        /// </summary>
        public const string PointY = "py";

        /// <summary>
        /// point collection
        /// </summary>
        public const string PointCollection = "pc";

        /// <summary>
        /// View width
        /// </summary>
        public static string ViewWidth = "vw";

        /// <summary>
        /// View Height 
        /// </summary>
        public static string ViewHeight = "vh";

        /// <summary>
        /// Feature class
        /// </summary>
        public const string FeatureClass = "fc";

        /// <summary>
        /// Feature class extended
        /// </summary>
        public const string FeatureClassX = "fcx";

        /// <summary>
        /// Field names
        /// </summary>
        public const string FieldName = "fldnm";

        /// <summary>
        /// Files name
        /// </summary>
        public const string Files = "fls";


        /// <summary>
        /// Boundary
        /// </summary>
        public const string Boundary = "bnd";

        /// <summary>
        /// Language
        /// </summary>
        public const string Language = "l";

        /// <summary>
        /// Search criterion
        /// </summary>
        public const string SearchCriterion = "sc";

        /// <summary>
        /// Search criterion type (Contains, StartWith etc.)
        /// </summary>
        public const string SearchCriterionType = "sct";

        /// <summary>
        /// Search Object type (Region, District, Settelment  etc.)
        /// </summary>
        public const string SearchObjectType = "so";

        /// <summary>
        /// Targed ConnectionString
        /// </summary>
        public const string ConnectionString = "cst";

        /// <summary>
        /// Targed ConnectionString
        /// </summary>
        public const string IdValue = "id";

        /// <summary>
        /// Return type
        /// </summary>
        public const string ReturnType = "t";

        /// <summary>
        /// Name for callback function to return jsonp
        /// </summary>
        public const string CallbackFoJSONP = "jsoncallback";

        /// <summary>
        /// Request Id
        /// </summary>
        public const string RequestId = "rid";

        /// <summary>
        /// Url
        /// </summary>
        public const string Url = "url";

        /// <summary>
        /// Key
        /// </summary>
        public const string Key = "key";

        /// <summary>
        /// Key
        /// </summary>
        public const string ReindexTable = "table";

        /// <summary>
        /// Width
        /// </summary>
        public const string Width = "wdth";

        /// <summary>
        /// Height
        /// </summary>
        public const string Height = "hght";

        /// <summary>
        /// Date
        /// </summary>
        public const string Date = "dt";

        public const string DateEnd = "enddt";


        /// <summary>
        /// Path to File
        /// </summary>
        public static string PathToFile = "pfl";

        /// <summary>
        /// Path to Folder
        /// </summary>
        public static string PathToFolder = "pfld";

        /// <summary>
        /// Path to File
        /// </summary>
        public static string DataWorkSpace = "dws";


        /// <summary>
        /// Data value
        /// </summary>
        public static string DataValue = "dv";

        public static string WorkingDirectory = "workingDir";

        public static string ErrorDataReceivedDelegate = "onErrorDelegate";

        public static string OutputDataReceivedDelegate = "onOutputDelegate";

    }
}
