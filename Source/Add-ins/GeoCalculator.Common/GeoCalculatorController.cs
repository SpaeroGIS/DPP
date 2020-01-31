using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.GeoCalculator
{
    public class GeoCalculatorController
    {
        internal void SetView(IGeoCalculatorView view)
        {
            View = view;
        }

        internal IGeoCalculatorView View { get; private set; }
    }
}
