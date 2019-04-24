using System;
using System.Collections.Generic;
using System.Linq;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer;


namespace MilSpace.Profile.SurfaceProfileChartControl
{
    public class SurfaceProfileChartController
    {
        private SurfaceProfileChart _surfaceProfileChart;
        private ProfileSession _profileSession;
        private MilSpaceProfileGraphsController _graphsController;

        private List<ProfileSurfacePoint> _extremePoints = new List<ProfileSurfacePoint>();

        internal delegate void ProfileGrapchClickedDelegate(GraphProfileClickedArgs e);

        internal event ProfileGrapchClickedDelegate OnProfileGraphClicked;


        public SurfaceProfileChartController()
        {
        }

        internal void SetSession(ProfileSession profileSession)
        {
            _profileSession = profileSession;
        }

        internal void SetCurrentChart(SurfaceProfileChart currentChart, MilSpaceProfileGraphsController graphsController)
        {
            _graphsController = graphsController;
            _surfaceProfileChart = currentChart;
        }

        internal SurfaceProfileChart CreateProfileChart()
        {
            _surfaceProfileChart = new SurfaceProfileChart(this);
            _surfaceProfileChart.InitializeGraph();

            return _surfaceProfileChart;
        }


        internal void AddProfile()
        {

        }

        internal void RemoveCurrentTab()
        {
            _graphsController.RemoveTab();
        }

        internal void LoadSeries()
        {
            _surfaceProfileChart.InitializeProfile(_profileSession);
        }

        internal void AddExtremePoints()
        {
            _extremePoints = FindExtremePoints();

            _surfaceProfileChart.SetExtremePoints(_extremePoints);
        }

        internal void AddInvisibleZones(List<double> observersHeights, ProfileSurface[] profileSurfaces = null)
        {
            if (profileSurfaces == null)
            {
                profileSurfaces = _profileSession.ProfileSurfaces;
            }

            foreach (var profileSessionProfileLine in profileSurfaces)
            {
                var profileSurfacePoints = profileSessionProfileLine.ProfileSurfacePoints;

                AddInvisibleZone(observersHeights[profileSessionProfileLine.LineId - 1], profileSessionProfileLine);
            }
        }

        internal void AddInvisibleZone(double observerHeight, ProfileSurface profileSurface)
        {
            var invisibleSurface = new ProfileSurface();
            var invisiblePoints = new List<ProfileSurfacePoint>();

            var sightLineKoef = 0.0;
            var isInvisibleZone = false;

            invisibleSurface.LineId = profileSurface.LineId;

            for (var i = 0; i < profileSurface.ProfileSurfacePoints.Length; i++)
            {
                if (!isInvisibleZone)
                {
                    if (i < profileSurface.ProfileSurfacePoints.Length - 1)
                    {
                        if (CalcAngleOfVisibility(observerHeight, profileSurface.ProfileSurfacePoints[i],
                            profileSurface.ProfileSurfacePoints[i + 1]) < 0)
                        {
                            invisiblePoints.Add(profileSurface.ProfileSurfacePoints[i + 1]);
                            isInvisibleZone = true;
                            sightLineKoef = (profileSurface.ProfileSurfacePoints[i + 1].Z - observerHeight)
                                / (profileSurface.ProfileSurfacePoints[i + 1].Distance);
                            i++;

                        }
                    }

                }
                else
                {
                    if (FindY(observerHeight, sightLineKoef, profileSurface.ProfileSurfacePoints[i].Distance)
                        < profileSurface.ProfileSurfacePoints[i].Z)
                    {
                        isInvisibleZone = false;
                        invisiblePoints.Add(profileSurface.ProfileSurfacePoints[i]);
                        i++;
                    }
                    else
                    {
                        invisiblePoints.Add(profileSurface.ProfileSurfacePoints[i]);
                    }
                }

            }

            invisibleSurface.ProfileSurfacePoints = invisiblePoints.ToArray();
            _surfaceProfileChart.AddInvisibleLine(invisibleSurface);
            CalcProfilesVisiblePercents(invisibleSurface);
        }

        internal void SetProfilesProperties()
        {

            foreach (var profileSessionProfileLine in _profileSession.ProfileLines)
            {
                var profileProperty = new ProfileProperties();
                profileProperty.LineId = profileSessionProfileLine.Id;

                var profileSurfacePoints = _profileSession.ProfileSurfaces.FirstOrDefault(surface =>
                        surface.LineId == profileSessionProfileLine.Id).ProfileSurfacePoints;

                profileProperty.MaxHeight = profileSurfacePoints.Max(point => point.Z);
                profileProperty.MinHeight = profileSurfacePoints.Min(point => point.Z);

                var angles = FindAngles(profileSurfacePoints);

                profileProperty.MaxAngle = angles.Exists(angle => angle > 0) ? Math.Abs(angles.Where(angle => angle > 0).Max()) : 0;
                profileProperty.MinAngle = angles.Exists(angle => angle < 0) ? Math.Abs(angles.Where(angle => angle < 0).Min()) : 0;

                profileProperty.PathLength = FindLength(profileSurfacePoints);

                profileProperty.Azimuth = FindAzimuth(RadiansToDegrees(profileSessionProfileLine.Angel));

                _surfaceProfileChart.ProfilesProperties.Add(profileProperty);
            }

        }

        private List<ProfileSurfacePoint> FindExtremePoints()
        {
            List<ProfileSurfacePoint> extremePoints = new List<ProfileSurfacePoint>();

            extremePoints.Add(_profileSession.ProfileSurfaces[0].ProfileSurfacePoints[0]);

            foreach (var profileSessionProfileLine in _profileSession.ProfileLines)
            {
                var profileSurfacePoints = _profileSession.ProfileSurfaces.FirstOrDefault(surface =>
                   surface.LineId == profileSessionProfileLine.Id).ProfileSurfacePoints;

                extremePoints.Add(profileSurfacePoints.Last());
            }

            return extremePoints;
        }

        private static double FindLength(ProfileSurfacePoint[] profileSurfacePoints)
        {
            double result = 0;

            for (var i = 0; i < profileSurfacePoints.Length - 1; i++)
            {
                result += CalcVectorLength(profileSurfacePoints[i], profileSurfacePoints[i + 1]);
            }

            return result;
        }

        private void CalcProfilesVisiblePercents(ProfileSurface invisibleSurface)
        {
            var profileProperty = _surfaceProfileChart.ProfilesProperties[invisibleSurface.LineId - 1];

            double invisibleLegth = 0;

            for (int i = 1; i < invisibleSurface.ProfileSurfacePoints.Count(); i++)
            {
                invisibleLegth += CalcVectorLength(invisibleSurface.ProfileSurfacePoints[i - 1],
                    invisibleSurface.ProfileSurfacePoints[i]);
            }

            _surfaceProfileChart.ProfilesProperties[invisibleSurface.LineId - 1].VisiblePercent =
                ((profileProperty.PathLength - invisibleLegth) * 100) / profileProperty.PathLength;
        }

        internal void InvokeOnProfileGraphClicked(double wgs94X, double wgs94Y)
        {
            var point = new GraphProfileClickedArgs(wgs94X, wgs94Y);
            point.ProfilePoint.SpatialReference = EsriTools.Wgs84Spatialreference;
            OnProfileGraphClicked?.Invoke(point);
        }

        private static List<double> FindAngles(ProfileSurfacePoint[] profileSurfacePoints)
        {
            var angles = new List<double>();
            List<ProfileSurfacePoint> _triangle = new List<ProfileSurfacePoint>();

            ProfileSurfacePoint deviationPoint = profileSurfacePoints[0];

            for (var i = 0; i < profileSurfacePoints.Length - 2; i++)
            {
                if (CalcAngleOfDeviation(profileSurfacePoints[i], profileSurfacePoints[i + 1]) !=
                    CalcAngleOfDeviation(profileSurfacePoints[i + 1], profileSurfacePoints[i + 2]))
                {
                    angles.Add(CalcAngleOfDeviation(deviationPoint, profileSurfacePoints[i + 1]));
                    deviationPoint = profileSurfacePoints[i + 1];
                }
                else
                {
                    _triangle.Add(profileSurfacePoints[i + 1]);
                }
            }
            return angles;
        }

        private static double CalcVectorLength(ProfileSurfacePoint leftPoint, ProfileSurfacePoint rightPoint)
        {
            var x = rightPoint.Distance - leftPoint.Distance;
            var y = rightPoint.Z - leftPoint.Z;

            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
        }

        private static double CalcAngleOfDeviation(ProfileSurfacePoint leftPoint, ProfileSurfacePoint rightPoint)
        {
            var angle = Math.Atan(Math.Abs(rightPoint.Z - leftPoint.Z) / (rightPoint.Distance - leftPoint.Distance));

            angle = RadiansToDegrees(angle);

            return (rightPoint.Z < leftPoint.Z) ? angle * (-1) : angle;
        }

        private static double CalcAngleOfVisibility(double observerHeight, ProfileSurfacePoint leftPoint,
            ProfileSurfacePoint rightPoint)
        {
            var sightLineKoef = (rightPoint.Z - observerHeight) / (rightPoint.Distance);
            var surfaceLineKoef = (rightPoint.Z - leftPoint.Z) / (rightPoint.Distance - leftPoint.Distance);

            var result = (surfaceLineKoef - sightLineKoef) / (1 + surfaceLineKoef * sightLineKoef);
            return Math.Atan(result);
        }

        private static double FindY(double observerHeight, double angleKoef, double x)
        {
            return (angleKoef * x + observerHeight);
        }

        private static double FindAzimuth(double angle)
        {
            if (angle > 90)
            {
                return 90 - angle;
            }

            if (angle < -90)
            {
                return Math.Abs(angle) - 270;
            }

            return Math.Abs(angle - 90);
        }

        private static double RadiansToDegrees(double radians)
        {
            return radians * 180 / Math.PI;
        }

    }
}
