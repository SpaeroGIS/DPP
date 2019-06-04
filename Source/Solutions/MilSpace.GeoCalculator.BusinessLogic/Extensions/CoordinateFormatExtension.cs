using MilSpace.GeoCalculator.BusinessLogic.ReferenceData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.GeoCalculator.BusinessLogic.Extensions
{
    public static class CoordinateFormatExtension
    {
        public static string ToRoundedString(this double inputValue)
        {
            return Math.Round(inputValue, Constants.NumberOfDigitsForDoubleRounding).ToString();
        }

        public static double ToRoundedDouble(this double inputValue)
        {
            return Math.Round(inputValue, Constants.NumberOfDigitsForDoubleRounding);
        }

        public static string ToSeparatedMgrs(this string mgrsRepresentation)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(mgrsRepresentation.Substring(0, 5));
            stringBuilder.Append(' ');
            stringBuilder.Append(mgrsRepresentation.Substring(5, 5));
            stringBuilder.Append(' ');
            stringBuilder.Append(mgrsRepresentation.Substring(10, 5));            
            return stringBuilder.ToString();
        }
    }
}
