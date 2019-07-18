using ESRI.ArcGIS.Geometry;
using MilSpace.Core;
using MilSpace.DataAccess.DataTransfer;

namespace MilSpace.Profile.DTO
{
    public class ProfileSettings
    {
        private const string  azimuthTempoleate = "{0}:{1}";
        public IPolyline[] ProfileLines;

        public string DemLayerName;
        public bool IsReady => ProfileLines != null && ProfileLines.Length > 0 && !string.IsNullOrWhiteSpace(DemLayerName);

        public ProfileSettingsTypeEnum Type;
        public double Azimuth1;
        public double Azimuth2;

        public string AzimuthToStore
        {
            get
            {
                if (Type == ProfileSettingsTypeEnum.Fun)
                {
                    return azimuthTempoleate.InvariantFormat(Azimuth1, Azimuth2);
                }
                return null;
            }
        }

    }
}
