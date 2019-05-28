using System;
using System.Collections.Generic;
using System.Linq;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer;
using ESRI.ArcGIS.Display;
using System.Drawing;
using MilSpace.DataAccess;

namespace MilSpace.Profile.SurfaceProfileChartControl
{
    public class SurfaceProfileChartController
    {
        private SurfaceProfileChart _surfaceProfileChart;
        private ProfileSession _profileSession;
        private MilSpaceProfileGraphsController _graphsController;
        private double _defaultObserverHeight;

        private List<ProfileSurfacePoint> _extremePoints = new List<ProfileSurfacePoint>();
        private List<LineIntersections> _linesIntersections = new List<LineIntersections>();

        internal delegate void ProfileGrapchClickedDelegate(GraphProfileClickedArgs e);
        internal delegate void ProfileChangeInvisiblesZonesDelegate(GroupedLines profileLines, int sessionId,
                                                                        bool update, List<int> linesIds = null);

        internal delegate void DeleteProfileDelegate(int sessionId, int lineId);
        internal delegate void SelectedProfileChangedDelegate(GroupedLines oldSelectedLines, GroupedLines newSelectedLines, int profileId);
        internal delegate void GetIntersectionLinesDelegate(ProfileSession profileSession, int lineId = 0);

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

        internal SurfaceProfileChartController GetCurrentController(SurfaceProfileChart currentChart, MilSpaceProfileGraphsController graphsController)
        {
            currentChart.SetCurrentChart(graphsController);
            return currentChart.GetController();
        }

        internal void  SetCurrentChart(MilSpaceProfileGraphsController graphsController)
        {
            _graphsController = graphsController;

            if (_profileSession.ProfileLines != null && _profileSession.ProfileLines.Count() == 1)
            {
                InvokeSelectedProfile(1);
            }
        }

        internal SurfaceProfileChart CreateProfileChart(double observerHeight)
        {
            _defaultObserverHeight = observerHeight;

            _surfaceProfileChart = new SurfaceProfileChart(this);

            if (_profileSession.DefinitionType != ProfileSettingsTypeEnum.Composed)
            {
                _surfaceProfileChart.InitializeGraph();
            }
            else
            {
                _surfaceProfileChart.IsGraphEmptyHandler(true);
            }

            return _surfaceProfileChart;
        }

        internal void RemoveCurrentTab()
        {
            _graphsController.RemoveTab();
        }

        internal void LoadSeries()
        {
            if (_profileSession.ProfileLines != null)
            {
                _surfaceProfileChart.InitializeGraph(_profileSession);
            }
        }

        internal void LoadSerie()
        {
            _surfaceProfileChart.AddSerie(_profileSession.ProfileSurfaces.Last());
        }

        internal void AddExtremePoints(ProfileSurface profileSurface = null)
        {
            if (profileSurface == null)
            {
                _extremePoints = FindExtremePoints();
                _surfaceProfileChart.SetExtremePoints(_extremePoints);
            }
            else
            {
                _extremePoints = FindExtremePoints(profileSurface);
                _surfaceProfileChart.SetExtremePoints(_extremePoints, profileSurface.LineId);
            }
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
                SetProfileProperty(profileSessionProfileLine);
            }
        }

        internal void AddProfileProperty()
        {
            SetProfileProperty(_profileSession.ProfileLines.Last());
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

        internal void InvokeGetIntersectionLines()
        {
            IntersectionLinesDrawing?.Invoke(_profileSession);
        }

        internal void InvokeGetIntersectionLine(int lineId)
        {
            IntersectionLinesDrawing?.Invoke(_profileSession, lineId);
        }

        internal void InvokeSelectedProfile(int selectedLineId)
        {
            if (_profileSession.Segments == null)
            {
                return;
            }
            
            var oldSelectedLine = _profileSession.Segments.Find(segment => segment.IsSelected == true);

            if (selectedLineId == -1)
            {
                SelectedProfileChanged?.Invoke(oldSelectedLine, null, _profileSession.SessionId);
            }
            else
            {
                var newSelectedLine = _profileSession.Segments.First(segment => segment.LineId == selectedLineId);

                if(oldSelectedLine == newSelectedLine)
                {
                    oldSelectedLine = null;
                }

                SelectedProfileChanged?.Invoke(oldSelectedLine, newSelectedLine, _profileSession.SessionId);
                newSelectedLine.IsSelected = true;
            }

            if (oldSelectedLine != null)
            {
                oldSelectedLine.IsSelected = false;
            }
        }

        internal void AddEmptyGraph()
        {
            _graphsController.AddEmptyGraph();
        }

        internal void AddLineToGraph(ProfileLine profileLine, ProfileSurface profileSurface)
        {
            if (_profileSession.ProfileLines == null)
            {
                InitializeComposedGraph(profileLine, profileSurface);
            }
            else
            {
                AddProfile(profileLine, profileSurface);
            }
        }

        internal void SetIntersectionLines(List<IntersectionsInLayer> intersections, int lineId)
        {
            var lineIntersection = _linesIntersections.FirstOrDefault(line => line.LineId == lineId);

            if (lineIntersection != null)
            {
                _linesIntersections.Remove(lineIntersection);
            }

            _linesIntersections.Add(new LineIntersections()
            {
                Intersections = intersections,
                LineId = lineId
            });
        }

        internal void DrawIntersectionLines(int lineId)
        {
            var intersectionLines = _linesIntersections.FirstOrDefault(line => line.LineId == lineId).Intersections;

            if (intersectionLines == null || intersectionLines.Count == 0)
            {
                _surfaceProfileChart.ClearIntersectionLines();
                return;
            }

            var lastPoint = _profileSession.ProfileSurfaces.First(surface => surface.LineId == lineId)
                                            .ProfileSurfacePoints.Last()
                                            .Distance;

            var preparedLines = PrepareIntersectionsToDrawing(intersectionLines, lastPoint);

            _surfaceProfileChart.DrawIntersections(preparedLines, lastPoint);

        }

        private void SetProfileProperty(ProfileLine profileSessionProfileLine)
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

        private void InitializeComposedGraph(ProfileLine profileLine, ProfileSurface profileSurface)
        {
            profileLine.Id = 1;
            profileSurface.LineId = 1;
            _profileSession.ProfileLines = new ProfileLine[] { profileLine };
            _profileSession.ProfileSurfaces = new ProfileSurface[] { profileSurface };
            _profileSession.SetSegments(profileLine.SpatialReference);

            _surfaceProfileChart.IsGraphEmptyHandler(false);
            _surfaceProfileChart.InitializeGraph();
            _surfaceProfileChart.SetControlSize();
        }

        private void AddProfile(ProfileLine profileLine, ProfileSurface profileSurface)
        {
            var lineId = _profileSession.ProfileLines.Last().Id + 1;

            profileLine.Id = lineId;
            profileSurface.LineId = lineId;

            var profileLines = new List<ProfileLine>();
            var profileSurfaces = new List<ProfileSurface>();

            profileLines.AddRange(_profileSession.ProfileLines);
            profileSurfaces.AddRange(_profileSession.ProfileSurfaces);

            profileLines.Add(profileLine);
            profileSurfaces.Add(profileSurface);

            _profileSession.ProfileLines = profileLines.ToArray();
            _profileSession.ProfileSurfaces = profileSurfaces.ToArray();

            _profileSession.SetSegments(profileLine.SpatialReference, _profileSession.ProfileLines.Last());

            _surfaceProfileChart.InitializeProfile();
            _surfaceProfileChart.SetControlSize();
        }

        private List<IntersectionsInLayer> PrepareIntersectionsToDrawing(List<IntersectionsInLayer> intersectionLines, double lastPoint)
        {
            if (intersectionLines.Count == 1 && intersectionLines[0].Lines.Count == 1)
            {
                return intersectionLines;
            }

            var orderedIntersectionLines = GetOrderedIntersectionsLines(intersectionLines);
            var preparedIntersectionLines = new List<IntersectionLine>();
            var accuracy = 0.0000001;

            var prevLine = new IntersectionLine();
            prevLine = orderedIntersectionLines.First();

            foreach (var intersectionLine in orderedIntersectionLines)
            {
                if (intersectionLine == orderedIntersectionLines.First())
                {
                    if (intersectionLine.PointFromDistance != 0)
                    {
                        var line = new IntersectionLine()
                        {
                            PointFromDistance = 0,
                            PointToDistance = intersectionLine.PointFromDistance,
                            LayerType = LayersEnum.NotIntersect
                        };

                        preparedIntersectionLines.Add(line);
                    }

                    continue;
                }

                if (intersectionLine.PointFromDistance < prevLine.PointToDistance
                        && intersectionLine.PointFromDistance > prevLine.PointFromDistance)
                {
                    var line = new IntersectionLine()
                    {
                        PointFromDistance = prevLine.PointFromDistance,
                        PointToDistance = intersectionLine.PointFromDistance,
                        LayerType = prevLine.LayerType
                    };

                    preparedIntersectionLines.Add(line);
                }
                else if (intersectionLine.PointFromDistance >= prevLine.PointToDistance
                            || Math.Abs(intersectionLine.PointFromDistance - prevLine.PointFromDistance) < accuracy)
                {
                    preparedIntersectionLines.Add(prevLine);

                    if (Math.Abs(intersectionLine.PointFromDistance - prevLine.PointToDistance) > accuracy)
                    {
                        var line = new IntersectionLine()
                        {
                            PointFromDistance = prevLine.PointToDistance,
                            PointToDistance = intersectionLine.PointFromDistance,
                            LayerType = LayersEnum.NotIntersect
                        };

                        preparedIntersectionLines.Add(line);
                    }

                    if (intersectionLine == orderedIntersectionLines.Last())
                    {
                        preparedIntersectionLines.Add(intersectionLine);

                        if (Math.Abs(intersectionLine.PointToDistance - lastPoint) > accuracy)
                        {
                            var emptyLine = new IntersectionLine()
                            {
                                PointFromDistance = intersectionLine.PointToDistance,
                                PointToDistance = lastPoint,
                                LayerType = LayersEnum.NotIntersect
                            };

                            preparedIntersectionLines.Add(emptyLine);
                        }
                    }
                    else
                    {
                        prevLine = new IntersectionLine();
                        prevLine = intersectionLine;
                    }
                }

                if (intersectionLine.PointToDistance < prevLine.PointToDistance
                        && intersectionLine.PointFromDistance > prevLine.PointFromDistance)
                {
                    var line = new IntersectionLine()
                    {
                        PointFromDistance = intersectionLine.PointToDistance,
                        PointToDistance = prevLine.PointToDistance,
                        LayerType = prevLine.LayerType
                    };

                    preparedIntersectionLines.Add(line);
                    preparedIntersectionLines.Add(intersectionLine);
                }
                else
                {
                    prevLine = new IntersectionLine();
                    prevLine = intersectionLine;
                }
            }

            return GroupLinesByLayers(preparedIntersectionLines);
        }

        private List<IntersectionLine> GetOrderedIntersectionsLines(List<IntersectionsInLayer> lines)
        {
            var orderedIntersectionLines = new List<IntersectionLine>();

            var orderedLayers = lines.OrderBy(layer => layer.Lines.Min(line => line.PointFromDistance));

            foreach (var intersectionLine in orderedLayers)
            {
                var orderedLines = intersectionLine.Lines.OrderBy(line => line.PointFromDistance);

                foreach (var line in orderedLines)
                {
                    orderedIntersectionLines.Add(line);
                }
            }

            return orderedIntersectionLines.OrderByDescending(line => line.LayerType).ToList();
        }

        private List<IntersectionsInLayer> GroupLinesByLayers(List<IntersectionLine> lines)
        {
            var preparedLines = new List<IntersectionsInLayer>();

            foreach (var line in lines)
            {
                var lineLayer = preparedLines.FirstOrDefault(layer => layer.Type == line.LayerType);

                if (lineLayer == null)
                {
                    var lineInLayer = new IntersectionsInLayer()
                    {
                        Lines = new List<IntersectionLine>(),
                        Type = line.LayerType
                    };

                    lineInLayer.SetDefaultColor();
                    lineInLayer.Lines.Add(line);
                    preparedLines.Add(lineInLayer);
                }
                else
                {
                    lineLayer.Lines.Add(line);
                }
            }

            return preparedLines;
        }

        private List<ProfileSurfacePoint> FindExtremePoints(ProfileSurface profileSurface = null)
        {
            List<ProfileSurfacePoint> extremePoints = new List<ProfileSurfacePoint>();

            if (profileSurface == null)
            {
                foreach (var profileSessionProfileLine in _profileSession.ProfileLines)
                {
                    var profileSurfacePoints = _profileSession.ProfileSurfaces.FirstOrDefault(surface =>
                       surface.LineId == profileSessionProfileLine.Id).ProfileSurfacePoints;

                    extremePoints.Add(profileSurfacePoints.Last());
                }
            }
            else
            {
                var profileSurfacePoints = profileSurface.ProfileSurfacePoints;

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
