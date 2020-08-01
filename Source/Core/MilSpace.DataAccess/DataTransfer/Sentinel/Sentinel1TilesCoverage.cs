using MilSpace.Core.Geometry;
using System;

namespace MilSpace.DataAccess.DataTransfer.Sentinel
{
    public class SentinelTilesCoverage
    {
        public int IdRow;
        public string QuaziTileName;
        public DateTime Dto;
        public string DEMFilePath;
        public string SceneName;
        public int Status;
        public string Operator;
        public string Wkt;
        public IWktGeometry Geometry => WktGeometry.Get(Wkt);
    }
}
