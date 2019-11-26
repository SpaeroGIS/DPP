using System;

namespace MilSpace.DataAccess.DataTransfer
{
    [Flags]
    public enum VisibilityCalculationResultsEnum 
    {
        None = 0,
        ObservationPoints = 1,
        VisibilityAreaRaster = 2,
        ObservationObjects = 4,
        VisibilityAreaPolygons = 8,
        CoverageTable = 16,
        ObservationPointSingle = 32,
        VisibilityAreaRasterSingle = 64,
        VisibilityObservStationClip = 128,
        VisibilityAreaPolygonSingle = 256,
        VisibilityAreasTrimmedByPoly = 512,
        VisibilityAreaTrimmedByPolySingle = 1024,
        VisibilityObservStationClipSingle = 4096,
        VisibilityAreasPotential = 8192,
        VisibilityAreaPotentialSingle = 16384
    }

    public enum LayerPositionsEnum
    {
        Above,
        Below
    }

    public class VisibilityTask: VisibilityCalcResults
    {
        public DateTime? Started { get; internal set; }
        public DateTime? Finished { get; internal set; }
        public VisibilityCalcResults GetVisibilityResults(bool shared)
        {
           return this.ToVisibilityResults(shared);
        }
    }
}
