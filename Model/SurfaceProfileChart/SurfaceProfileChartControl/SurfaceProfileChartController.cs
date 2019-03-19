using System.Linq;
using MilSpace.DataAccess.DataTransfer;

namespace SurfaceProfileChart.SurfaceProfileChartControl
{
    internal class SurfaceProfileChartController
    {
        private SurfaceProfileChart _surfaceProfileChart;
        private ProfileSession _profileSession;

        public SurfaceProfileChartController(SurfaceProfileChart surfaceProfileChart)
        {
            _surfaceProfileChart = surfaceProfileChart;
            _profileSession = DataPreparator.Get();
        }

        public void LoadSeries()
        {
            _surfaceProfileChart.InitializeProfile(_profileSession);
        }

        public void AddInvisibleZones()
        {
            //todo handle invisible zones search

            //test example 
            ProfileSurface surface = new ProfileSurface
            {
                LineId = _profileSession.ProfileSurfaces[0].LineId,
                ProfileSurfacePoints = _profileSession.ProfileSurfaces[0].ProfileSurfacePoints
                    .Where(point => point.Z < 150).ToArray()
            };
            //end test example

            _surfaceProfileChart.AddInvisibleLine(surface);
        }

    }
}
