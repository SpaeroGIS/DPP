using ESRI.ArcGIS.Geometry;
using System.Collections.Generic;

namespace MilSpace.DataAccess.DataTransfer
{
    public class WizardResult
    {
        public IEnumerable<int>  ObservPointIDs{ get; set; }
        public IEnumerable<int> ObservObjectIDs { get; set; }
        public string RasterLayerName { get; set; }
        public string RelativeLayerName { get; set; }
        public short ResultLayerTransparency { get; set; }
        public bool SumFieldOfView { get; set; }
        public bool Table { get; set; }
        public VisibilityCalculationResultsEnum VisibilityCalculationResults { get; set; }
        public LayerPositionsEnum ResultLayerPosition { get; set; }
        public ObservationPoint ObservationPoint { get; set; }
        public IGeometry ObservationStation { get; set; } 
        public short VisibilityPercent { get; set; }
        public double FromHeight { get; set; }
        public double ToHeight { get; set; }
        public int Step { get; set; }

        public VisibilityCalcTypeEnum CalculationType;

        public string TaskName;
        public ObservationSetsEnum ObserverPointsSourceType { get; set; }
        public string ObserverPointsLayerName { get; set; }
        public string ObservationObjectLayerName { get; set; }
        public ObservationSetsEnum ObserverObjectsSourceType { get; set; }
        public int Buffer { get; set; }

    }
}
