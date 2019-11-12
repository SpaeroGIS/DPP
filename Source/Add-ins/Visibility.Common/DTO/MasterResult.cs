﻿using MilSpace.DataAccess.DataTransfer;
using System.Collections.Generic;

namespace MilSpace.Visibility.DTO
{
    internal class MasterResult
    {
        public IEnumerable<int>  ObservPointIDs{ get; set; }
        public IEnumerable<int> ObservObjectIDs { get; set; }
        public string RasterLayerName { get; set; }
        public bool SumFieldOfView { get; set; }
        public bool Table { get; set; }
        public VisibilityCalculationresultsEnum VisibilityCalculationResults { get; set; }

        public VisibilityCalcTypeEnum CalculationType;
    }
}
