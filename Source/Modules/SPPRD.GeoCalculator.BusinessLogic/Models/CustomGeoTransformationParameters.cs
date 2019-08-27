using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.GeoCalculator.BusinessLogic.Models
{
    public struct CustomGeoTransformationParameters
    {    
        public double XAxisTranslation;
        public double YAxisTranslation;
        public double ZAxisTranslation;
        public double XAxisRotation;
        public double YAxisRotation;
        public double ZAxisRotation;
        public double ScaleDifference;
    }
}
