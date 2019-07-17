using System;
using System.Collections.Generic;
using System.Globalization;
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
        public int? Number { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }              

        public string ToString(bool coordinatesOnly = false)
        {
            if (coordinatesOnly) return $"{Longitude.ToString(CultureInfo.InvariantCulture)} {Latitude.ToString(CultureInfo.InvariantCulture)}";

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"{(Number == null ? 0 : Number.Value)}");
            stringBuilder.AppendLine($"{Longitude.ToString(CultureInfo.InvariantCulture)} {Latitude.ToString(CultureInfo.InvariantCulture)}");                        
            return stringBuilder.ToString();
        }
    }
}
