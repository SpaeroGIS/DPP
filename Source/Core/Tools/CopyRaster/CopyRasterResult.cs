using MilSpace.Core.Actions.Base;

namespace MilSpace.Tools.CopyRaster
{
    public class CopyRasterResult : ActionResultBase<CopyRasterResultDefinition>
    {
        public CopyRasterResult()
        {
            Result = new CopyRasterResultDefinition();
        }

        public override CopyRasterResultDefinition Result
        { get; set; }
    }
}
