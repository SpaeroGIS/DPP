using MilSpace.AddDem.ReliefProcessing;
using MilSpace.Core.ModulesInteraction;
using Sposterezhennya.AddDEM.ArcMapAddin.AddInComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sposterezhennya.AddDEM.ArcMapAddin.Interaction
{
    public class AddDemInteraction : IAddDemInteraction
    {
        AddDemController controller;

        PrepareDem prepareDemForm = null;

        public AddDemInteraction(AddDemController controller)
        {
            this.controller = controller;
        }

        public Form AddDemForm { get => prepareDemForm; set => prepareDemForm = value as PrepareDem; }

        public bool AddDemToMap(string rasterFileName)
        {
            return controller.AddRasterToMap(rasterFileName);
        }
    }
}
