using MilSpace.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.AddDem.ReliefProcessing
{
    public class PrepareDemController
    {
        IPrepareDemView prepareDemview;
        internal PrepareDemController()
        { }

        public void SetView(IPrepareDemView view)
        {
            prepareDemview = view;
        }

        public void ReadConfiguration()
        {
            if (prepareDemview == null)
            {
                throw new MethodAccessException("prepareDemview cannot be null");
            }
            prepareDemview.SentinelSrtorage = MilSpaceConfiguration.DemStorages.SentinelStorage;
            prepareDemview.SrtmSrtorage = MilSpaceConfiguration.DemStorages.SrtmStorage;
        }
    }
}
