using System;

namespace MilSpace.DataAccess.DataTransfer
{
    [Flags]
    public enum VisibilityCalculationresultsEnum 
    {
        None = 0,
        ObservationPoints = 1,
        VisibilityAreaRaster = 2,
        ObservationStations = 4,
        VisibilityAreaPolygons = 8,
        CoverageTable = 16,
        ObservationPointSingle = 32,
        VisibilityAreaRasterSingle = 64,
        VisibilityObservStationClip = 128,
        VisibilityAreaPolygonSingle = 256,
        VisibilityAreasTrimmedByPoly = 512,
        VisibilityAreaTrimmedByPolySingle = 1024,
        VisibilityAreaPolygonForObjects = 2048,
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
