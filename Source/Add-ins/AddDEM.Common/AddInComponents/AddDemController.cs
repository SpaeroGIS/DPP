using MilSpace.AddDem.ReliefProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sposterezhennya.AddDEM.ArcMapAddin.AddInComponents
{
    public  class AddDemController
    {
        private IAddDemView view;
        public AddDemController()
        {
        }

        public void RegisterView(IAddDemView view)
        {
            this.view = view;
        }

        public void OpenDemCalcForm()
        {
            var demCalcForm = new PrepareDem();
            demCalcForm.ShowDialog();
        }

    }
}
