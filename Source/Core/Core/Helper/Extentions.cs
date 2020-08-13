using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Core
{
    public static class Extentions
    {
        public static bool TryParceToDouble(this string numericString, out double result)
        {
            string snumericString = numericString.Trim();
            string snumericString2;

            if (snumericString.Contains('.'))
            {
                snumericString2 = snumericString.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            }
            else
            {
                snumericString2 = snumericString.Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            }

            return double.TryParse(snumericString2, out result);
        }

        public static double ParceToDouble(this string numericString)
        {
            string snumericString = numericString.Trim();

            double result = double.NaN;

            double.TryParse(numericString, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
            return result;

            //if (snumericString.Contains('.'))
            //{
            //    snumericString2 = snumericString.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            //}
            //else
            //{
            //    snumericString2 = snumericString.Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            //}


            //double.TryParse(snumericString2, out result);
            //return result;
        }
    }
}
