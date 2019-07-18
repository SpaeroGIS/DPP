using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MilSpace.GeoCalculator.BusinessLogic.Models
{
    [Serializable]
    public class PointModelsList
    {
        [XmlArray("PointList"), XmlArrayItem(typeof(PointModel), ElementName = "Point")]
        public List<PointModel> PointList { get; set; }
    }
}
