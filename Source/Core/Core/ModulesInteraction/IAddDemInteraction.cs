using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MilSpace.Core.ModulesInteraction
{
    public interface IAddDemInteraction
    {
        bool AddDemToMap(string rasterFileName);

        Form AddDemForm { get; set; }
    }
}
