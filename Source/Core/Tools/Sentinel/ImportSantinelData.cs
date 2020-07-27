using ESRI.ArcGIS.Geometry;
using MilSpace.DataAccess.DataTransfer.Sentinel;

namespace MilSpace.Tools.Sentinel
{
    public class ImportSentinelData : SentinelProduct
    {
        public IPolygon FootprintPoly;
        public IPolygon WktPoly;
        public IPolygon JTSfootprintPoly;
    }

    //public class Summary
    //{
    //    DateTime DateTime;
    //    string Filename;
    //    string Identifier;
    //    string Instrument;
    //    string Satellite;
    //    double Size;
    //}
}

   
