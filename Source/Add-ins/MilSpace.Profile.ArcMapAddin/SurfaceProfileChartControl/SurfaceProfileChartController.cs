using System;
using System.Collections.Generic;
using System.Linq;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer;
using ESRI.ArcGIS.Display;
using System.Drawing;
using MilSpace.DataAccess;
using MilSpace.Core.Tools.Helper;

namespace MilSpace.Profile.SurfaceProfileChartControl
{
    public class SurfaceProfileChartController
    {
        private SurfaceProfileChart _surfaceProfileChart;
        private ProfileSession _profileSession;
        private MilSpaceProfileGraphsController _graphsController;
        private double _defaultObserverHeight;

        private List<ProfileSurfacePoint> _extremePoints = new List<ProfileSurfacePoint>();

        internal delegate void ProfileGrapchClickedDelegate(GraphProfileClickedArgs e);
        internal delegate void ProfileChangeInvisiblesZonesDelegate(GroupedLines profileLines, int sessionId,
                                                                        bool update, List<int> linesIds = null);

        internal delegate void DeleteProfileDelegate(int sessionId, int lineId);
        internal delegate void SelectedProfileChangedDelegate(GroupedLines newSelectedLines, int profileId);
        internal delegate void GetIntersectionLinesDelegate(GroupedLines selectedLines);

        internal event ProfileGrapchClickedDelegate OnProfileGraphClicked;
        internal event ProfileChangeInvisiblesZonesDelegate InvisibleZonesChanged;
        internal event DeleteProfileDelegate ProfileRemoved;
        internal event SelectedProfileChangedDelegate SelectedProfileChanged;
        internal event GetIntersectionLinesDelegate IntersectionLinesDrawing;

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

            if (_profileSession.ProfileLines.Count() == 1)
            {
                _surfaceProfileChart.SelectProfile("1");
            }
        }

        internal SurfaceProfileChart CreateProfileChart(double observerHeight)
        {
            _defaultObserverHeight = observerHeight;

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

        internal void AddInvisibleZones(Dictionary<int, double> observersHeights, List<Color> visibleColors, List<Color> invisibleColors,
                                            ProfileSurface[] profileSurfaces = null)
        {
            var i = 0;

            if (profileSurfaces == null)
            {
                profileSurfaces = _profileSession.ProfileSurfaces;
            }

            List<int> linesIds = new List<int>();

            foreach (var surface in profileSurfaces)
            {
                linesIds.Add(surface.LineId);
            }

            foreach (var profileSessionProfileLine in profileSurfaces)
            {
                var profileSurfacePoints = profileSessionProfileLine.ProfileSurfacePoints;

                AddInvisibleZone(observersHeights[profileSessionProfileLine.LineId], profileSessionProfileLine,
                                    visibleColors[i], invisibleColors[i], false, linesIds);
                i++;
            }
        }

        internal void AddInvisibleZone(double observerHeight, ProfileSurface profileSurface,
                                        Color visibleColor, Color invisibleColor, bool update = true,
                                        List<int> linesIds = null)
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
                            var firstInvisiblePoint = profileSurface.ProfileSurfacePoints[i + 1];

                            invisiblePoints.Add(firstInvisiblePoint);

                            isInvisibleZone = true;
                            sightLineKoef = (firstInvisiblePoint.Z - observerHeight) / (firstInvisiblePoint.Distance);
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
            CalcProfilesVisiblePercents(invisibleSurface, profileSurface);

            var profileLines = GetLines(profileSurface, invisibleSurface, _profileSession.SessionId);
            profileLines.VisibleColor = ColorToEsriRgb(visibleColor);
            profileLines.InvisibleColor = ColorToEsriRgb(invisibleColor);
            profileLines.Polylines = ProfileLinesConverter
                                        .ConvertLineToEsriPolyline(profileLines.Lines,
                                                                     ArcMap.Document.FocusMap.SpatialReference);

            if (_profileSession.Segments.Count < profileSurface.LineId - 1)
            {
                _profileSession.Segments.Add(profileLines);
            }
            else
            {
                profileLines.IsSelected = _profileSession.Segments[profileSurface.LineId - 1].IsSelected;
                _profileSession.Segments[profileSurface.LineId - 1] = profileLines;
            }

            InvisibleZonesChanged?.Invoke(profileLines, _profileSession.SessionId, update, linesIds);
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

                profileProperty.Azimuth = profileSessionProfileLine.Azimuth;

                profileProperty.ObserverHeight = _defaultObserverHeight;

                _surfaceProfileChart.ProfilesProperties.Add(profileProperty);
            }

        }

        internal void InvokeOnProfileGraphClicked(double wgs94X, double wgs94Y)
        {
            var point = new GraphProfileClickedArgs(wgs94X, wgs94Y);
            point.ProfilePoint.SpatialReference = EsriTools.Wgs84Spatialreference;
            OnProfileGraphClicked?.Invoke(point);
        }

        internal void InvokeProfileRemoved(int profileId)
        {
            ProfileRemoved?.Invoke(_profileSession.SessionId, profileId);
        }

        internal void InvokeGraphRedrawn(int lineId, Color visibleColor)
        {
            _profileSession.Segments.First(segment => segment.LineId == lineId).VisibleColor = ColorToEsriRgb(visibleColor);

            InvisibleZonesChanged?.Invoke(_profileSession.Segments.First(segment => segment.LineId == lineId),
                                                _profileSession.SessionId, true);
        }

        internal void InvokeGetIntersectionLines(int lineId)
        {
            IntersectionLinesDrawing?.Invoke(_profileSession.Segments.First(segment => segment.LineId == lineId));
        }

        internal void InvokeSelectedProfile(int selectedLineId)
        {
            SelectedProfileChanged?.Invoke(_profileSession.Segments.First(segment => segment.LineId == selectedLineId), _profileSession.SessionId);
            _profileSession.Segments.First(segment => segment.LineId == selectedLineId).IsSelected = true;
        }

        internal void DrawIntersectionLines(List<IntersectionLines> intersectionLines)
        {
            var lastPoint = _profileSession.ProfileSurfaces.First(surface => surface.LineId == intersectionLines[0].LineId)
                                            .ProfileSurfacePoints.Last()
                                            .Distance;

            _surfaceProfileChart.AddIntersectionsLine(lastPoint);

            var preparedLines = PrepareIntersectionsToDrawing(intersectionLines);

            foreach (var intersectionLine in preparedLines)
            {
                _surfaceProfileChart.SetIntersections(intersectionLine.Lines, intersectionLine.LineColor, lastPoint);
            }
        }

        private List<IntersectionLines> PrepareIntersectionsToDrawing(List<IntersectionLines> intersectionLines)
        {
            var preparedLines = new List<IntersectionLines>();
            var nonIntersectLine = new ProfileLine();
            var intersectionLinesDictionary = new Dictionary<LayersEnum, ProfileLine>();
            var preparedIntersectionLinesDictionary = new Dictionary<LayersEnum, ProfileLine>();

            var orderedLayers = intersectionLines.OrderBy(layer => layer.Lines.Min(line => line.PointFrom.X));

            foreach (var intersectionLine in orderedLayers)
            {
                var orderedLines = intersectionLine.Lines.OrderBy(line => line.PointFrom.X);

                foreach (var line in orderedLines)
                {
                    intersectionLinesDictionary.Add(intersectionLine.Type, line);
                }
            }

            double pointTo = intersectionLinesDictionary.Values.First().PointTo.X;
            var prevValue = new ProfileLine();
            prevValue = intersectionLinesDictionary.Values.First();
            var prevValueType = intersectionLinesDictionary.Keys.First();

            foreach (var value in intersectionLinesDictionary.Values)
            {
                if (value == intersectionLinesDictionary.Values.First())
                {
                    if (value.PointFrom.X != 0)
                    {
                        var line = new ProfileLine()
                        {
                            PointFrom = new ProfilePoint() { X = 0, Y = value.PointFrom.Y },
                            PointTo = value.PointFrom
                        };

                        preparedIntersectionLinesDictionary.Add(LayersEnum.NotIntersect, line);
                    }

                    continue;
                }

                if (value.PointFrom.X < pointTo)
                {
                    var line = new ProfileLine()
                    {
                        PointFrom = prevValue.PointFrom,
                        PointTo = value.PointFrom
                    };

                    preparedIntersectionLinesDictionary.Add(prevValueType, line);
                }

                if (value.PointTo.X < pointTo)
                {
                    var line = new ProfileLine()
                    {
                        PointFrom = value.PointTo,
                        PointTo = prevValue.PointTo
                    };

                    preparedIntersectionLinesDictionary.Add(prevValueType, line);
                    preparedIntersectionLinesDictionary.Add(intersectionLinesDictionary.First(dictionary => dictionary.Value == value).Key, value);
                }
                else
                {
                    prevValue = new ProfileLine();
                    prevValue = value;
                    prevValueType = intersectionLinesDictionary.First(dictionary => dictionary.Value == value).Key;
                    pointTo = prevValue.PointTo.X;
                }

                if (value.PointFrom.X > pointTo)
                {
                    var line = new ProfileLine()
                    {
                        PointFrom = prevValue.PointTo,
                        PointTo = value.PointFrom
                    };

                    preparedIntersectionLinesDictionary.Add(LayersEnum.NotIntersect, line);

                    prevValue = new ProfileLine();
                    prevValue = value;
                    prevValueType = intersectionLinesDictionary.First(dictionary => dictionary.Value == value).Key;
                    pointTo = prevValue.PointTo.X;
                }
            }

            foreach(var preparedLineLayer in preparedIntersectionLinesDictionary.Keys)
            {
                var preparedIntersectionLines = new IntersectionLines()
                    {
                        LineId = intersectionLines[0].LineId,
                       // Lines = preparedIntersectionLinesDictionary.Where(value => value.Key == preparedLineLayer),
                        Type = preparedLineLayer
                    };

                preparedIntersectionLines.SetDefaultColor();
                preparedLines.Add(preparedIntersectionLines);
            }
            ////****/////
            //var points = _profileSession.ProfileSurfaces.First(surface => surface.LineId == intersectionLines[0].LineId).ProfileSurfacePoints;

            //foreach (var point in points)
            //{
            //    var dPoint = point.Distance;
            //    var buffLines = new List<IntersectionLines>();

            //    foreach (var intersectionLine in intersectionLines)
            //    {
            //        var line = intersectionLine.Lines.FirstOrDefault(intersection => dPoint <= intersection.PointTo.X && dPoint >= intersection.PointFrom.X);

            //        if (line != null)
            //        {
            //            var buffIntersectionLine = new IntersectionLines();
            //            buffIntersectionLine = intersectionLine;
            //            buffIntersectionLine.Lines.Clear();
            //            buffIntersectionLine.Lines.Add(line);
            //            buffLines.Add(buffIntersectionLine);
            //        }
            //    }

            //    if (buffLines.Count == 1)
            //    {
            //        var buffTypeLines = preparedLines.FirstOrDefault(preparedLine => preparedLine.Type == buffLines[0].Type);
            //        if (buffTypeLines != null) { buffTypeLines.Lines.AddRange(buffLines[0].Lines); }
            //        else { preparedLines.Add(buffLines[0]); }
            //    }
            //    else if (buffLines.Count > 1)
            //    {
            //        var cutLines = CutLines(buffLines);

            //        foreach(var cutLine in cutLines)
            //        {
            //            var buffTypeLines = preparedLines.FirstOrDefault(preparedLine => preparedLine.Type == cutLine.Type);
            //            if (buffTypeLines != null) { buffTypeLines.Lines.AddRange(cutLine.Lines); }
            //            else { preparedLines.Add(cutLine); }
            //        }
            //    }
            //    else
            //    {
            //        var buffTypeLines = preparedLines.FirstOrDefault(preparedLine => preparedLine.Type == LayersEnum.NotIntersect);

            //        if (buffTypeLines == null)
            //        {
            //            var notIntersectionLines = new IntersectionLines
            //            {
            //                LineColor = Color.White,
            //                LineId = intersectionLines[0].LineId,
            //                Type = LayersEnum.NotIntersect,
            //                Lines = new List<ProfileLine>()
            //            };

            //            preparedLines.Add(notIntersectionLines);
            //        }

            //        if (nonIntersectLine.PointFrom == null)
            //        {
            //            nonIntersectLine.PointFrom = new ProfilePoint()
            //            {
            //                X = point.X,
            //                Y = point.Y
            //            };
            //        }
            //        else
            //        {
            //            nonIntersectLine.PointTo = new ProfilePoint()
            //            {
            //                X = point.X,
            //                Y = point.Y
            //            };

            //            buffTypeLines.Lines.Add(nonIntersectLine);
            //            nonIntersectLine = new ProfileLine();
            //        }
            //    }
            //}

            return preparedLines;
        }

        private List<IntersectionLines> CutLines(List<IntersectionLines> lines)
        {
            var cutLines = new List<IntersectionLines>();
            lines.OrderBy(line => line.Lines[0].Length);

            cutLines.Add(lines[0]);

            for (int i = 0; i < lines.Count - 1; i++)
            {
                var minLine = lines[i].Lines[0];
                var maxLine = lines[i + 1].Lines[0];

                var cutLine = new IntersectionLines();
                cutLine = lines[i + 1];
                cutLine.Lines.Clear();

                var leftLine = new ProfileLine();
                leftLine = maxLine;
                leftLine.PointFrom.X = maxLine.PointFrom.X;
                leftLine.PointTo.X = minLine.PointFrom.X - 0.1;

                var rightLine = new ProfileLine();
                rightLine = maxLine;
                rightLine.PointFrom.X = minLine.PointTo.X + 0.1;
                rightLine.PointTo.X = maxLine.PointTo.X;

                if (maxLine.PointFrom.X > minLine.PointFrom.X)
                {
                    cutLine.Lines.Add(rightLine);
                }
                else if (maxLine.PointTo.X < minLine.PointTo.X)
                {
                    cutLine.Lines.Add(leftLine);
                }
                else
                {
                    cutLine.Lines.Add(leftLine);
                    cutLine.Lines.Add(rightLine);
                }

                cutLines.Add(cutLine);
            }

            return cutLines;
        }

        private List<ProfileSurfacePoint> FindExtremePoints()
        {
            List<ProfileSurfacePoint> extremePoints = new List<ProfileSurfacePoint>();

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

        private void CalcProfilesVisiblePercents(ProfileSurface invisibleSurface, ProfileSurface allSurface)
        {
            var profileProperty
                    = _surfaceProfileChart
                       .ProfilesProperties
                       .First(property => property.LineId == invisibleSurface.LineId);

            double invisibleLegth = 0;
            int j = 0;

            if (invisibleSurface.ProfileSurfacePoints.Count() == 0)
            {
                _surfaceProfileChart.ProfilesProperties
                                    .First(property => property.LineId == invisibleSurface.LineId).VisiblePercent = 100;

                return;
            }

            for (int i = 0; i < allSurface.ProfileSurfacePoints.Count() - 1; i++)
            {
                if (allSurface.ProfileSurfacePoints[i] == invisibleSurface.ProfileSurfacePoints[j])
                {
                    invisibleLegth += CalcVectorLength(allSurface.ProfileSurfacePoints[i],
                   allSurface.ProfileSurfacePoints[i + 1]);
                    j++;
                }

                if (j >= invisibleSurface.ProfileSurfacePoints.Count())
                {
                    i = allSurface.ProfileSurfacePoints.Count() - 1;
                }
            }

            _surfaceProfileChart.ProfilesProperties
                                .First(property => property.LineId == invisibleSurface.LineId).VisiblePercent =
                ((profileProperty.PathLength - invisibleLegth) * 100) / profileProperty.PathLength;
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

        private static RgbColor ColorToEsriRgb(Color rgb)
        {
            return new RgbColor
            {
                Blue = rgb.B,
                Red = rgb.R,
                Green = rgb.G
            };
        }

        private static GroupedLines GetLines(ProfileSurface allSurface, ProfileSurface invisibleSurface, int sessionId)
        {
            var profileLines = new List<ProfileLine>();

            var lineId = 1;

            var j = 0;

            var isInvisiblePointsFinished = false;

            if (invisibleSurface.ProfileSurfacePoints.Count() == 0)
            {
                isInvisiblePointsFinished = true;
            }

            var profileVisibleLine = new ProfileLine
            {
                Visible = true
            };

            var profileInvisibleLine = new ProfileLine
            {
                Visible = false
            };

            var profileVisiblePoints = new List<ProfileSurfacePoint>();
            var profileInvisiblePoints = new List<ProfileSurfacePoint>();

            for (int i = 0; i < allSurface.ProfileSurfacePoints.Count(); i++)
            {
                if (!isInvisiblePointsFinished && allSurface.ProfileSurfacePoints[i] == invisibleSurface.ProfileSurfacePoints[j])
                {
                    if (profileInvisiblePoints.Count == 0)
                    {
                        StartOfLineHandler(ref profileInvisibleLine, lineId, allSurface, i);
                        lineId++;

                        if (profileVisiblePoints.Count > 0)
                        {
                            profileLines.Add(EndOfLineHandler(profileVisibleLine, allSurface, i));

                            profileVisibleLine = new ProfileLine
                            {
                                Visible = true
                            };

                            profileVisiblePoints = new List<ProfileSurfacePoint>();
                        }
                    }

                    if (i == allSurface.ProfileSurfacePoints.Count() - 1)
                    {
                        profileLines.Add(EndOfLineHandler(profileInvisibleLine, allSurface, i));

                        profileInvisibleLine = new ProfileLine
                        {
                            Visible = false
                        };

                        profileInvisiblePoints = new List<ProfileSurfacePoint>();
                    }

                    if (j == invisibleSurface.ProfileSurfacePoints.Count() - 1)
                    {
                        isInvisiblePointsFinished = true;
                    }

                    profileInvisiblePoints.Add(allSurface.ProfileSurfacePoints[i]);
                    j++;
                }
                else
                {
                    if (profileVisiblePoints.Count == 0)
                    {
                        StartOfLineHandler(ref profileVisibleLine, lineId, allSurface, i);

                        lineId++;

                        if (profileInvisiblePoints.Count > 0)
                        {
                            profileLines.Add(EndOfLineHandler(profileInvisibleLine, allSurface, i));

                            profileInvisibleLine = new ProfileLine
                            {
                                Visible = false
                            };

                            profileInvisiblePoints = new List<ProfileSurfacePoint>();
                        }
                    }

                    if (i == allSurface.ProfileSurfacePoints.Count() - 1)
                    {
                        profileLines.Add(EndOfLineHandler(profileVisibleLine, allSurface, i));

                        profileVisibleLine = new ProfileLine
                        {
                            Visible = true
                        };

                        profileVisiblePoints = new List<ProfileSurfacePoint>();
                    }

                    profileVisiblePoints.Add(allSurface.ProfileSurfacePoints[i]);
                }
            }

            return new GroupedLines
            {
                Lines = profileLines,
                LineId = allSurface.LineId
            };
        }

        private static void StartOfLineHandler(ref ProfileLine startLine,
                                                int startLineId, ProfileSurface allSurfaces,
                                                 int pointIndex)
        {
            startLine.PointFrom = new ProfilePoint
            {
                X = allSurfaces.ProfileSurfacePoints[pointIndex].X,
                Y = allSurfaces.ProfileSurfacePoints[pointIndex].Y
            };

            startLine.Id = startLineId;
        }

        private static ProfileLine EndOfLineHandler(ProfileLine endLine, ProfileSurface allSurfaces, int pointIndex)
        {
            endLine.PointTo = new ProfilePoint
            {
                X = allSurfaces.ProfileSurfacePoints[pointIndex].X,
                Y = allSurfaces.ProfileSurfacePoints[pointIndex].Y
            };

            return endLine;
        }

    }
}
