using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Profile.DTO
{
    public class ProfileSettings
    {
        public IPolyline[] ProfileLines;

        public string DemLayerName;
        public bool IsReady => ProfileLines != null && ProfileLines.Length > 0 && !string.IsNullOrWhiteSpace(DemLayerName);
    }
}
