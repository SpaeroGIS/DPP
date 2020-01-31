using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MilSpace.Core.ModulesInteraction;

namespace MilSpace.Profile.Interaction
{
    public class ProfileInteraction : IProfileInteraction
    {
        private MilSpaceProfileCalsController _controller;

        internal ProfileInteraction(MilSpaceProfileCalsController controller)
        {
            _controller = controller;
        }
    }
}
