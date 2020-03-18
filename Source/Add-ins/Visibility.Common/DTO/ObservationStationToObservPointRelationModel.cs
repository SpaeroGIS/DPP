using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Visibility.DTO
{
    internal class ObservationStationToObservPointRelationModel
    {
        public int Id;
        public string Title;
        public double Azimuth;
        public IPolyline Polyline;
        public CoverageTypesEnum CoverageType;
    }

    internal enum CoverageTypesEnum
    {
        Full,
        Partly,
        None
    }
}
