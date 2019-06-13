using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MilSpace.GeoCalculator.BusinessLogic.Models
{
    [Serializable]
    [XmlRoot("PointModelsList")]
    public class PointModel
    {
        public string Guid { get; set; }
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
        public string UtmRepresentation { get; set; }

        public string ToString(bool currentMapCoordinatesOnly = false)
        {
            if (currentMapCoordinatesOnly) return $"{XCoord} {YCoord}";

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"{XCoord} {YCoord}");
            stringBuilder.AppendLine($"{WgsXCoord} {WgsYCoord}");
            stringBuilder.AppendLine($"{WgsXCoordDD} {WgsYCoordDD}");
            stringBuilder.AppendLine($"{PulkovoXCoord} {PulkovoYCoord}");
            stringBuilder.AppendLine($"{PulkovoXCoordDD} {PulkovoYCoordDD}");
            stringBuilder.AppendLine($"{UkraineXCoord} {UkraineYCoord}");
            stringBuilder.AppendLine($"{UkraineXCoordDD} {UkraineYCoordDD}");
            stringBuilder.AppendLine($"{MgrsRepresentation}");
            stringBuilder.AppendLine($"{UtmRepresentation}");
            return stringBuilder.ToString();
        }
    }
}
