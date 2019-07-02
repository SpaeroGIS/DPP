using MilSpace.GeoCalculator.BusinessLogic.ReferenceData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MilSpace.GeoCalculator.BusinessLogic.Extensions
{
    public static class CoordinateFormatExtension
    {
        public static string ToRoundedString(this double inputValue)
        {
            return Math.Round(inputValue, Constants.NumberOfDigitsForDoubleRounding).ToString(CultureInfo.InvariantCulture);
        }

        public static double ToRoundedDouble(this double inputValue)
        {
            return Math.Round(inputValue, Constants.NumberOfDigitsForDoubleRounding);
        }

        public static string ToIntegerString(this double inputValue)
        {
            return Math.Truncate(inputValue).ToString(CultureInfo.InvariantCulture);
        }

        public static double ToInteger(this double inputValue)
        {
            return Math.Truncate(inputValue);
        }

        public static bool ToDoubleInvariantCulture(this string inputValue, out double result)
        {
            return double.TryParse(inputValue, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
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

        public static string ToNormalizedString(this string stringRepresentation)
        {
            var specialCharactersRemoved = Regex.Replace(stringRepresentation, "[^0-9A-Za-z ,.]", " ");
            return specialCharactersRemoved.Trim();
        }
    }
}
