using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Tools.Sentinel
{
    public class SentinelProduct
    {
        public string Uuid; // "70dea7e6-01f9-476a-8707-5e81a45b89fb",
        public int Id;
        public string Identifier { get; set; }
        public DateTime DateTime;
        public string Instrument;
        public string Footprint;
        public string JTSfootprint;
        public string PassDirection;
        public int RelativeOrbit;
        public int OrbitNumber;
        public int SliceNumber;
        public string Wkt;


    }

    public class ImportSentinelData : SentinelProduct
    {
        public IPolygon FootprintPoly;
        public IPolygon WktPoly;
        public IPolygon JTSfootprintPoly;
    }

    public class Summary
    {
        DateTime DateTime;
        string Filename;
        string Identifier;
        string Instrument;
        string Satellite;
        double Size;
    }
}

   
