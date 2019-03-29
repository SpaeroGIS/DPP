using System;
using System.Collections.Generic;
using System.Linq;
using MilSpace.DataAccess.DataTransfer;


namespace MilSpace.Profile.SurfaceProfileChartControl
{
    public class SurfaceProfileChartController
    {
        private SurfaceProfileChart _surfaceProfileChart;
        private ProfileSession _profileSession;
        private List<ProfileSurfacePoint> _extremePoints = new List<ProfileSurfacePoint>();


        public SurfaceProfileChartController()
        {
        }

        internal void GetSession(ProfileSession profileSession)
        {
            _profileSession = profileSession;
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

        internal void LoadSeries()
        {
            _surfaceProfileChart.InitializeProfile(_profileSession);
        }

        internal void AddExtremePoints()
        {
             _extremePoints = FindExtremePoints();

            _surfaceProfileChart.SetExtremePoints(_extremePoints);
        }

        internal void AddInvisibleZones(double observerHeight)
        {
            foreach (var profileSessionProfileLine in _profileSession.ProfileLines)
            {
                var profileSurfacePoints = _profileSession.ProfileSurfaces.FirstOrDefault(surface =>
                    surface.LineId == profileSessionProfileLine.Id).ProfileSurfacePoints;

                var invisibleSurface = new ProfileSurface();
                var invisiblePoints = new List<ProfileSurfacePoint>();

                double sightLineKoef = 0;
                var isInvisibleZone = false;

                invisibleSurface.LineId = profileSessionProfileLine.Id;

                for (var i = 0; i < profileSurfacePoints.Length; i++)
                {
                    if (!isInvisibleZone)
                    {
                        if (i < profileSurfacePoints.Length - 1)
                        {
                            if (CalcAngleOfVisibility(observerHeight, profileSurfacePoints[i], profileSurfacePoints[i + 1]) < 0)
                            {
                                invisiblePoints.Add(profileSurfacePoints[i + 1]);
                                isInvisibleZone = true;
                                sightLineKoef = (profileSurfacePoints[i + 1].Z - observerHeight) / (profileSurfacePoints[i + 1].Distance);
                                i++;

                            }
                        }

                    }
                    else
                    {
                        if (FindY(observerHeight, sightLineKoef, profileSurfacePoints[i].Distance) < profileSurfacePoints[i].Z)
                        {
                            isInvisibleZone = false;
                            invisiblePoints.Add(profileSurfacePoints[i]);
                            i++;
                        }
                        else
                        {
                            invisiblePoints.Add(profileSurfacePoints[i]);
                        }
                    }

                }

                invisibleSurface.ProfileSurfacePoints = invisiblePoints.ToArray();
                _surfaceProfileChart.AddInvisibleLine(invisibleSurface);
            }
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
                profileProperty.MaxAngle = Math.Abs(angles.Where(angle => angle > 0).Max());
                profileProperty.MinAngle = Math.Abs(angles.Where(angle => angle < 0).Min());

                profileProperty.PathLength = FindLength(profileSurfacePoints);

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

        private double FindLength(ProfileSurfacePoint[] profileSurfacePoints)
        {
            double result = 0;

            for (var i = 0; i < profileSurfacePoints.Length - 1; i++)
            {
                result += CalcVectorLength(profileSurfacePoints[i], profileSurfacePoints[i + 1]);
            }

            return result;
        }

        private List<double> FindAngles(ProfileSurfacePoint[] profileSurfacePoints)
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

        private double CalcVectorLength(ProfileSurfacePoint leftPoint, ProfileSurfacePoint rightPoint)
        {
            var x = rightPoint.Distance - leftPoint.Distance;
            var y = rightPoint.Z - leftPoint.Z;

            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
        }

        private double CalcAngleOfDeviation(ProfileSurfacePoint leftPoint,
            ProfileSurfacePoint rightPoint)
        {
            var angle = Math.Atan(Math.Abs(rightPoint.Z - leftPoint.Z) / (rightPoint.Distance - leftPoint.Distance)) * (180 / Math.PI);
            return (rightPoint.Z < leftPoint.Z) ? angle * (-1) : angle;
        }

        private double CalcAngleOfVisibility(double observerHeight, ProfileSurfacePoint leftPoint,
            ProfileSurfacePoint rightPoint)
        {
            var sightLineKoef = (rightPoint.Z - observerHeight) / (rightPoint.Distance);
            var surfaceLineKoef = (rightPoint.Z - leftPoint.Z) / (rightPoint.Distance - leftPoint.Distance);

            var result = (surfaceLineKoef - sightLineKoef) / (1 + surfaceLineKoef * sightLineKoef);
            return Math.Atan(result);
        }

        private double FindY(double observerHeight, double angleKoef, double x)
        {
            return (angleKoef * x + observerHeight);
        }

    }
}
