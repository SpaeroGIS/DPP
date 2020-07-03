using System;

namespace MilSpace.DataAccess.DataTransfer
{
    [Flags]
    public enum VisibilityCalculationResultsEnum
    {
        None = 0,
        
        //Observation points result
        //If this value is in the calculation result number that the point feature class 
        //{taskName}_oo_p should be in the calculation GBD
        ObservationPoints = 1,

        //Image with calculated visibility based on a DEM and parameters of the OPs
        //If this value is in the calculation result number that the raster dataset
        //{taskName}_img should be in the calculation GBD
        VisibilityAreaRaster = 2,

        //Observation Objects result
        //If this value is in the calculation result number that the polygon feature class
        //{taskName}_oo_r should be in the calculation GBD
        ObservationObjects = 4,

        //Polygon(s) taken from the VisibilityAreaRaster results. 
        //If this value is in the calculation result number that the polygon feature class
        //{taskName}_va_r should be in the calculation GBD
        VisibilityAreaPolygons = 8,

        //Table with visibility coverage persentage
        //If this value is in the calculation result number that the table
        //{taskName}_ct should be in the calculation GBD
        CoverageTable = 16,

        //Observation point result
        //If this value is in the calculation result number that the point feature class  
        //{taskName}_{id}_oo_p should be in the calculation GBD.
        // This is an intermediate result. It is used for calculation a visibility area for a single observation point
        ObservationPointSingle = 32,
        
        //Image with calculated visibility for one observation point based on a DEM and parameters of the OP 
        //If this value is in the calculation result number that the raster dataset
        //{taskName}_{id}_imgs should be in the calculation GBD
        VisibilityAreaRasterSingle = 64,

        //Image with calculated visibility based on a DEM and parameters of the OPs 
        //and clipped by observation stations which took part in the calculation
        //If this value is in the calculation result number that the raster dataset
        //{taskName}_imgc should be in the calculation GBD
        VisibilityObservStationClip = 128,

        //Polygon, taken from the VisibilityAreaRasterSingle result
        //If this value is in the calculation result number that the polygon feature class
        //{taskName}_vas_r should be in the calculation GBD
        VisibilityAreaPolygonSingle = 256,

        //Image. The visibility result VisibilityAreaRaster trimmed by VisibilityAreaPolygons. 
        //It make the image smaller and take just valueable area.
        //If this value is in the calculation result number that the raster dataset
        //{taskName}_imgt should be in the calculation GBD
        VisibilityAreasTrimmedByPoly = 512,

        //Image. The same as VisibilityAreasTrimmedByPoly but relates to a single observation point
        //If this value is in the calculation result number that the raster dataset
        //{taskName}_[id]_imgt should be in the calculation GBD
        VisibilityAreaTrimmedByPolySingle = 1024,

        //Image with calculated visibility based on a DEM and parameters of the OPs
        //and clipped by observation station which took part in the calculation
        //If this value is in the calculation result number that the raster dataset
        //{taskName}_{id}_imgc should be in the calculation GBD
        VisibilityObservStationClipSingle = 4096,

        //Polydon(a) of a potential visibility area of the OPs which took part in the calculation
        //If this value is in the calculation result number that the raster dataset
        //{taskName}_pva_r should be in the calculation GBD
        VisibilityAreasPotential = 8192,

        //Polydon of a potential visibility area of the OP which took part in the calculation
        //If this value is in the calculation result number that the raster dataset
        //{taskName}_{id}_pvas_r should be in the calculation GBD
        VisibilityAreaPotentialSingle = 16384,

        //Table with best parameters for observer point
        //If this value is in the calculation result number that the table
        //{taskName}_bp should be in the calculation GBD
        BestParametersTable = 32768,

        //Image with calculated visibility based on a DEM and parameters of the OPs
        //If this value is in the calculation result number that the raster dataset
        //{taskName}_img should be in the calculation GBD
        VisibilityRastertPotentialArea = 98304,

        //Image with calculated visibility based on a DEM and parameters of the OPs
        //If this value is in the calculation result number that the raster dataset
        //{taskName}_img should be in the calculation GBD
        VisibilityRastertPotentialAreaSingle = 196608
    }

    public enum LayerPositionsEnum
    {
        Above,
        Below
    }

    public class VisibilityTask : VisibilityCalcResults
    {
        public DateTime? Started { get; internal set; }
        public DateTime? Finished { get; internal set; }
        public VisibilityCalcResults GetVisibilityResults(bool shared)
        {
            return this.ToVisibilityResults(shared);
        }
        public string TaskLog { get; set; }
    }
}
