using MilSpace.Core.ModulesInteraction;

namespace MilSpace.Profile.Interaction
{
    public class ProfileInteraction : IProfileInteraction
    {
        private MilSpaceProfileCalcsController _controller;

        internal ProfileInteraction(MilSpaceProfileCalcsController controller)
        {
            _controller = controller;
        }

        public void UpdateGraphics()
        {
            _controller.ShowAllProfiles();
        }
    }
}
