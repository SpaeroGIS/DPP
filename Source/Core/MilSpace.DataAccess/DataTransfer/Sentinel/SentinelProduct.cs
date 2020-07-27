using System;

namespace MilSpace.DataAccess.DataTransfer.Sentinel
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
        public string Satellite;
        public string Mode;
        public string ProductType;
        public DateTime Dto;
        public string Operator;
        public Tile RelatedTile;

    }
}

