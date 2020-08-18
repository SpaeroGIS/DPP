using MilSpace.Core.ModulesInteraction;
using Sposterezhennya.AddDEM.ArcMapAddin.AddInComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sposterezhennya.AddDEM.ArcMapAddin.Interaction
{
    public class AddDemInteraction : IAddDemInteraction
    {
        AddDemController controller;
        public AddDemInteraction(AddDemController controller)
        {
            this.controller = controller;
        }

        public bool AddDemToMap(string rasterFileName)
        {
            return controller.AddRasterToMap(rasterFileName);
        }
    }
}
