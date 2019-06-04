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
    }
}
