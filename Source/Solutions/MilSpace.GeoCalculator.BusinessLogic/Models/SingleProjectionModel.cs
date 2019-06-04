using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.GeoCalculator.BusinessLogic.Models
{
    public class SingleProjectionModel
    {
        private SingleProjectionModel() { }

        public SingleProjectionModel(int esriWellKnownID, double falseOriginX, double falseOriginY, double units = 1000)
        {
            ESRIWellKnownID = esriWellKnownID;
            FalseOriginX = falseOriginX;
            FalseOriginY = falseOriginY;
            Units = units;
        }

        public int ESRIWellKnownID { get; private set; }
        public double FalseOriginX { get; private set; }
        public double FalseOriginY { get; private set; }
        public double Units { get; private set; }
    }
}
