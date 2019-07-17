using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.GeoCalculator.BusinessLogic.Models
{
    public class ExtendedPointModel
    {
        public double XCoord { get; set; }
        public double YCoord { get; set; }
        public double WgsXCoord { get; set; }
        public double WgsXCoordDD { get; set; }
        public double WgsYCoord { get; set; }
        public double WgsYCoordDD { get; set; }
        public double PulkovoXCoord { get; set; }
        public double PulkovoXCoordDD { get; set; }
        public double PulkovoYCoord { get; set; }
        public double PulkovoYCoordDD { get; set; }
        public double UkraineXCoord { get; set; }
        public double UkraineXCoordDD { get; set; }
        public double UkraineYCoord { get; set; }
        public double UkraineYCoordDD { get; set; }
        public string MgrsRepresentation { get; set; }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(String.Format(CultureInfo.InvariantCulture, "{0} {1}", XCoord, YCoord));
            stringBuilder.AppendLine(String.Format(CultureInfo.InvariantCulture, "{0} {1}", WgsXCoord, WgsYCoord));
            stringBuilder.AppendLine(String.Format(CultureInfo.InvariantCulture, "{0} {1}", WgsXCoordDD, WgsYCoordDD));
            stringBuilder.AppendLine(String.Format(CultureInfo.InvariantCulture, "{0} {1}", PulkovoXCoord, PulkovoYCoord));
            stringBuilder.AppendLine(String.Format(CultureInfo.InvariantCulture, "{0} {1}", PulkovoXCoordDD, PulkovoYCoordDD));
            stringBuilder.AppendLine(String.Format(CultureInfo.InvariantCulture, "{0} {1}", UkraineXCoord, UkraineYCoord));
            stringBuilder.AppendLine(String.Format(CultureInfo.InvariantCulture, "{0} {1}", UkraineXCoordDD, UkraineYCoordDD));
            stringBuilder.AppendLine($"{MgrsRepresentation}");
            return stringBuilder.ToString();
        }
    }
}
