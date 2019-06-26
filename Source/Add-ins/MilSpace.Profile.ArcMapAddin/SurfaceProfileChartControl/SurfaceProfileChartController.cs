using System;
using System.Collections.Generic;
using System.Linq;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer;
using ESRI.ArcGIS.Display;
using System.Drawing;
using MilSpace.DataAccess;
using System.Text;

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

        private Dictionary<int, bool> _linesStraightnesses = new Dictionary<int, bool>();
        private Dictionary<int, List<ProfileSurface>> _linesSegments = new Dictionary<int, List<ProfileSurface>>();

        internal delegate void ProfileGrapchClickedDelegate(GraphProfileClickedArgs e);
        internal delegate void ProfileChangeInvisiblesZonesDelegate(GroupedLines profileLines, int sessionId,
                                                                        bool update, List<int> linesIds = null);

        internal delegate void DeleteProfileDelegate(int sessionId, int lineId);
        internal delegate void SelectedProfileChangedDelegate(GroupedLines oldSelectedLines, GroupedLines newSelectedLines, int profileId);
        internal delegate void GetIntersectionLinesDelegate(ProfileLine selectedLine, ProfileSession profileSession);

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

            if(_profileSession.ProfileLines != null && _profileSession.ProfileLines.Count() > 0)
            {
                for(int i = 0; i < _profileSession.ProfileLines.Count(); i++)
                {
                    _profileSession.ProfileLines[i].SessionId = _profileSession.SessionId;
                }
            }
        }

        internal SurfaceProfileChartController GetCurrentController(SurfaceProfileChart currentChart, MilSpaceProfileGraphsController graphsController)
        {
            currentChart.SetCurrentChart(graphsController);
            return currentChart.GetController();
        }

        internal void SetCurrentChart(MilSpaceProfileGraphsController graphsController)
        {
            _graphsController = graphsController;

            if(_profileSession.ProfileLines != null && _profileSession.ProfileLines.Count() == 1)
            {
                InvokeSelectedProfile(1);
            }
        }

        internal SurfaceProfileChart CreateProfileChart(double observerHeight)
        {
            _defaultObserverHeight = observerHeight;

            _surfaceProfileChart = new SurfaceProfileChart(this);

            if(_profileSession.DefinitionType != ProfileSettingsTypeEnum.Composed)
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
            if(_profileSession.ProfileLines != null)
            {
                _surfaceProfileChart.InitializeGraph(_profileSession);
            }
        }

        internal void LoadSerie()
        {
            _surfaceProfileChart.AddSerie(_profileSession.ProfileSurfaces.Last());
        }

        internal string GetProfileNameForLabel(ref int currentSessionId, int lineId)
        {
            if(_profileSession.ProfileLines == null || _profileSession.ProfileLines.Count() == 0)
            {
                return _profileSession.SessionName;
            }

            var sessionId = _profileSession.ProfileLines.First(line => lineId == line.Id).SessionId;

            if(sessionId != currentSessionId)
            {
                currentSessionId = sessionId;
                var sessionName = _graphsController.GetProfileNameById(sessionId);

                if(!String.IsNullOrEmpty(sessionName))
                {
                    return sessionName;
                }
            }

            return String.Empty;
        }

        internal string GetProfileName(int lineId)
        {
            if(_profileSession.ProfileLines == null || _profileSession.ProfileLines.Count() == 0)
            {
                return _profileSession.SessionName;
            }

            var sessionId = _profileSession.ProfileLines.First(line => lineId == line.Id).SessionId;

            return _graphsController.GetProfileNameById(sessionId);
        }

        internal void AddProfileToExistedGraph()
        {
            _graphsController.InvokeAddProfile();
        }

        internal void AddExtremePoints(ProfileSurface profileSurface = null)
        {
            if(profileSurface == null)
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

        internal void AddInvisibleZones(List<Color> visibleColors, List<Color> invisibleColors,
                                            ProfileSurface[] profileSurfaces = null)
        {
            var i = 0;

            if(profileSurfaces == null)
            {
                profileSurfaces = _profileSession.ProfileSurfaces;
            }

            List<int> linesIds = new List<int>();

            foreach(var surface in profileSurfaces)
            {
                linesIds.Add(surface.LineId);
            }

            foreach(var profileSessionProfileLine in profileSurfaces)
            {
                var observerHeight = _surfaceProfileChart.ProfilesProperties
                                                         .First(property => property.LineId == profileSessionProfileLine.LineId)
                                                         .ObserverHeight;

                AddInvisibleZone(observerHeight, profileSessionProfileLine,
                                         visibleColors[i], invisibleColors[i], false, linesIds);
                i++;
            }
        }

        internal void AddInvisibleZone(double observerHeight, ProfileSurface profileSurface,
                                        Color visibleColor, Color invisibleColor, bool update = true,
                                        List<int> linesIds = null)
        {
            var invisibleSurface = new ProfileSurface();
            var invisibleSurfacePoints = new List<ProfileSurfacePoint>();
            var groupedLinesSegments = new List<ProfileLine>();
            var isPrimitive = true;
            var surfaceSegments = GetLineSegments(profileSurface.LineId);

            if(surfaceSegments == null)
            {
                surfaceSegments = new List<ProfileSurface> { profileSurface };
                isPrimitive = false;
            }

            foreach(var segment in surfaceSegments)
            {
                var firstPointDistance = segment.ProfileSurfacePoints[0].Distance;
                var sightLineKoef = 0.0;
                var isInvisibleZone = false;
                var observerFullHeight = observerHeight + segment.ProfileSurfacePoints[0].Z;
                var invisibleSurfaceSegment = new ProfileSurface();
                var invisiblePoints = new List<ProfileSurfacePoint>();

                invisibleSurfaceSegment.LineId = profileSurface.LineId;

                for(var i = 0; i < segment.ProfileSurfacePoints.Length; i++)
                {
                    if(!isInvisibleZone)
                    {
                        if(i < segment.ProfileSurfacePoints.Length - 1)
                        {
                            if(CalcAngleOfVisibility(observerFullHeight, segment.ProfileSurfacePoints[i],
                                segment.ProfileSurfacePoints[i + 1], firstPointDistance) < 0)
                            {
                                var firstInvisiblePoint = segment.ProfileSurfacePoints[i + 1];

                                if(!invisiblePoints.Exists(point => point == firstInvisiblePoint))
                                {
                                    invisiblePoints.Add(firstInvisiblePoint);
                                }

                                isInvisibleZone = true;
                                sightLineKoef = (firstInvisiblePoint.Z - observerFullHeight) / (firstInvisiblePoint.Distance - firstPointDistance);
                                i++;
                            }
                        }
                    }
                    else
                    {
                        if(FindY(observerFullHeight, sightLineKoef, segment.ProfileSurfacePoints[i].Distance - firstPointDistance)
                            < segment.ProfileSurfacePoints[i].Z)
                        {
                            isInvisibleZone = false;
                            invisiblePoints.Add(segment.ProfileSurfacePoints[i]);

                            i++;
                        }
                        else
                        {
                            invisiblePoints.Add(segment.ProfileSurfacePoints[i]);
                        }
                    }
                }

                invisibleSurfaceSegment.ProfileSurfacePoints = invisiblePoints.ToArray();
                groupedLinesSegments.AddRange(GetLines(segment, invisibleSurfaceSegment, _profileSession.SessionId));

                invisibleSurfacePoints.AddRange(invisiblePoints);
            }

            invisibleSurface.ProfileSurfacePoints = invisibleSurfacePoints.ToArray();
            invisibleSurface.LineId = profileSurface.LineId;

            _surfaceProfileChart.AddInvisibleLine(invisibleSurface);
            CalcProfilesVisiblePercents(invisibleSurface, profileSurface);

            var profileLines = new GroupedLines
            {
                LineId = profileSurface.LineId,
                Lines = groupedLinesSegments,
                VisibleColor = ColorToEsriRgb(visibleColor),
                InvisibleColor = ColorToEsriRgb(invisibleColor)
            };

            profileLines.Polylines = ProfileLinesConverter
                                        .ConvertLineToEsriPolyline(profileLines.Lines,
                                                                     ArcMap.Document.FocusMap.SpatialReference);
            if(isPrimitive)
            {
                var spatialReference = _profileSession.ProfileLines.First(line => line.Id == profileLines.LineId).SpatialReference;
                profileLines.Vertices = surfaceSegments.Select(surface =>
                {
                    var point = surface.ProfileSurfacePoints.First();

                    return new ProfilePoint { X = point.X, Y = point.Y, SpatialReference = spatialReference };
                }
                ).ToList();

                profileLines.IsPrimitive = true;
            }

            if(_profileSession.Segments.Count < profileSurface.LineId - 1)
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
            foreach(var profileSessionProfileLine in _profileSession.ProfileLines)
            {
                SetProfileProperty(profileSessionProfileLine);
            }
        }

        internal void AddProfileProperty()
        {
            SetProfileProperty(_profileSession.ProfileLines.Last());
        }

        internal bool IsLineStraight(int lineId)
        {
            if(_linesStraightnesses.Keys.Contains(lineId))
            {
                return _linesStraightnesses[lineId];
            }
            else
            {
                var result = true;

                if(_profileSession.ProfileSurfaces.First(surface => surface.LineId == lineId)
                               .ProfileSurfacePoints.Where(point => point.isVertex == true)
                               .Count() > 2)
                {
                    result = false;
                }

                _linesStraightnesses.Add(lineId, result);
                return result;
            }

        }

        internal List<ProfileSurface> GetLineSegments(int lineId)
        {
            if(_linesSegments.Count != 0 && _linesSegments.Keys.FirstOrDefault(key => key == lineId) != 0)
            {
                return _linesSegments[lineId];
            }

            return null;
        }

        internal void InvokeOnProfileGraphClicked(double wgs94X, double wgs94Y)
        {
            var point = new GraphProfileClickedArgs(wgs94X, wgs94Y);
            point.ProfilePoint.SpatialReference = EsriTools.Wgs84Spatialreference;
            OnProfileGraphClicked?.Invoke(point);
        }

        internal void InvokeProfileRemoved(int profileId)
        {
            if(_linesIntersections != null)
            {
                var intrersections = _linesIntersections.FirstOrDefault(line => line.LineId == profileId);

                if(intrersections != null)
                {
                    _linesIntersections.Remove(_linesIntersections.FirstOrDefault(line => line.LineId == profileId));
                }
            }
            ProfileRemoved?.Invoke(_profileSession.SessionId, profileId);
        }

        internal void InvokeGraphRedrawn(int lineId, Color visibleColor)
        {
            _profileSession.Segments.First(segment => segment.LineId == lineId).VisibleColor = ColorToEsriRgb(visibleColor);

            InvisibleZonesChanged?.Invoke(_profileSession.Segments.First(segment => segment.LineId == lineId),
                                                _profileSession.SessionId, true);
        }

        internal void InvokeGetIntersectionLine(int lineId)
        {
            IntersectionLinesDrawing?.Invoke(_profileSession.ProfileLines.First(line => line.Id == lineId), _profileSession);
        }

        internal void InvokeSelectedProfile(int selectedLineId)
        {
            if(_profileSession.Segments == null)
            {
                return;
            }

            var oldSelectedLine = _profileSession.Segments.Find(segment => segment.IsSelected == true);

            if(selectedLineId == -1)
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

            if(oldSelectedLine != null)
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
            if(_profileSession.ProfileLines == null)
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

            if(lineIntersection != null)
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
            if(_linesIntersections.Count == 0)
            {
                return;
            }

            var intersectionLines = _linesIntersections.FirstOrDefault(line => line.LineId == lineId);

            if(intersectionLines == null || intersectionLines.Intersections.Count == 0)
            {
                _surfaceProfileChart.ClearIntersectionLines();
                return;
            }

            var lastPoint = _profileSession.ProfileSurfaces.First(surface => surface.LineId == lineId)
                                            .ProfileSurfacePoints.Last()
                                            .Distance;

            var preparedLines = PrepareIntersectionsToDrawing(intersectionLines.Intersections, lastPoint);

            _surfaceProfileChart.DrawIntersections(preparedLines, lastPoint);

        }

        internal string GetProfilePropertiesText(int lineId)
        {
            var profilePropertiesText = new StringBuilder();
            var profileProperty = _surfaceProfileChart.ProfilesProperties.FirstOrDefault(property => property.LineId == lineId);

            if(profileProperty == null)
            {
                return String.Empty;
            }

            var header = "Line number; Azimuth; Point of view; Profile length; Max height; Height difference; Min height; Max angle; Min angle; Visibility percent";
            profilePropertiesText.AppendLine(header);

            var properties = $"{profileProperty.LineId};{profileProperty.Azimuth};{profileProperty.ObserverHeight};{profileProperty.PathLength};" +
                             $"{profileProperty.MaxHeight};{profileProperty.MaxHeight - profileProperty.MinHeight};{profileProperty.MinHeight};" +
                             $"{profileProperty.MaxAngle};{profileProperty.MinAngle};{profileProperty.VisiblePercent}";

            profilePropertiesText.AppendLine(properties);

            return profilePropertiesText.ToString();
        }

        internal string GetProfilePointsPropertiesText(int lineId)
        {
            var pointsPropertiesText = new StringBuilder();
            var points = _profileSession.ProfileSurfaces.FirstOrDefault(surface => surface.LineId == lineId).ProfileSurfacePoints;

            if(points == null)
            {
                return String.Empty;
            }

            var header = "Number; X; Y; Z; Distance; Vertex; Visible; Intersections";
            var trueText = "Yes";
            var falseText = "No";

            pointsPropertiesText.AppendLine(header);

            for(int i = 0; i < points.Count(); i++)
            {
                pointsPropertiesText.Append($"{i};{points[i].X};{points[i].Y};{points[i].Z};{points[i].Distance};");

                var vertex = points[i].isVertex ? trueText : falseText;
                var visible = _surfaceProfileChart.IsPointVisible(lineId, i) ? trueText : falseText;

                pointsPropertiesText.AppendLine($"{vertex};{visible};{points[i].Layers.ToString()}");
            }

            return pointsPropertiesText.ToString();
        }

        internal void SetSurfaceSegments(List<ProfileSurface> segments)
        {
            if(_linesSegments.Keys.FirstOrDefault(key => key == segments[0].LineId) == 0)
            {
                _linesSegments.Add(segments[0].LineId, segments);
            }
        }


        internal void AddVertexPointsToLine(List<ProfileSurface> segments, double observerHeight)
        {
            _surfaceProfileChart.AddVertexPoint(segments[0].ProfileSurfacePoints.First(), true, segments[0].LineId, observerHeight);

            for(int i = 0; i < segments.Count(); i++)
            {
                var isVisible = true;
                var points = segments[i].ProfileSurfacePoints;

                var minHeightPoint = (points[0].Z > points.Last().Z) ? points.Last().Z : points[0].Z;
                minHeightPoint += observerHeight;

                for(int j = 0; j < points.Count() - 1; j++)
                {
                    if(points[j].Z < minHeightPoint && points[j + 1].Z < minHeightPoint)
                    {
                        continue;
                    }

                    var vertex = new ProfileSurfacePoint { Z = points[0].Z + observerHeight, Distance = points[0].Distance };
                    var vertexNext = new ProfileSurfacePoint { Z = points.Last().Z + observerHeight, Distance = points.Last().Distance };

                    if(IsLinesIntersected(vertex, vertexNext, points[j], points[j + 1]))
                    {
                        isVisible = false;
                        break;
                    }
                }

                _surfaceProfileChart.AddVertexPoint(points.Last(), isVisible, segments[i].LineId, observerHeight);
            }
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
            if(intersectionLines.Count == 1 && intersectionLines[0].Lines.Count == 1)
            {
                return intersectionLines;
            }

            var orderedIntersectionLines = GetOrderedIntersectionsLines(intersectionLines);
            var preparedIntersectionLines = new List<IntersectionLine>();
            var accuracy = 0.000001;

            var prevLine = new IntersectionLine();
            prevLine = orderedIntersectionLines.First();

            foreach(var intersectionLine in orderedIntersectionLines)
            {
                if(intersectionLine == orderedIntersectionLines.First())
                {
                    if(intersectionLine.PointFromDistance != 0)
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

                if(intersectionLine.PointFromDistance < prevLine.PointToDistance
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
                else if(intersectionLine.PointFromDistance >= prevLine.PointToDistance
                            || Math.Abs(intersectionLine.PointFromDistance - prevLine.PointFromDistance) < accuracy)
                {
                    preparedIntersectionLines.Add(prevLine);

                    if(Math.Abs(intersectionLine.PointFromDistance - prevLine.PointToDistance) > accuracy)
                    {
                        var line = new IntersectionLine()
                        {
                            PointFromDistance = prevLine.PointToDistance,
                            PointToDistance = intersectionLine.PointFromDistance,
                            LayerType = LayersEnum.NotIntersect
                        };

                        preparedIntersectionLines.Add(line);
                    }

                    if(intersectionLine == orderedIntersectionLines.Last())
                    {
                        preparedIntersectionLines.Add(intersectionLine);

                        if(Math.Abs(intersectionLine.PointToDistance - lastPoint) > accuracy)
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

                if(intersectionLine.PointToDistance < prevLine.PointToDistance
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

            foreach(var intersectionLine in lines)
            {
                var orderedLines = intersectionLine.Lines;

                foreach(var line in orderedLines)
                {
                    orderedIntersectionLines.Add(line);
                }
            }

            return orderedIntersectionLines.OrderBy(line => line.PointFromDistance).ToList();
        }

        private List<IntersectionsInLayer> GroupLinesByLayers(List<IntersectionLine> lines)
        {
            var preparedLines = new List<IntersectionsInLayer>();

            foreach(var line in lines)
            {
                var lineLayer = preparedLines.FirstOrDefault(layer => layer.Type == line.LayerType);

                if(lineLayer == null)
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

            return preparedLines.OrderByDescending(line => line.Type).ToList();
        }

        private List<ProfileSurfacePoint> FindExtremePoints(ProfileSurface profileSurface = null)
        {
            List<ProfileSurfacePoint> extremePoints = new List<ProfileSurfacePoint>();

            if(profileSurface == null)
            {
                foreach(var profileSessionProfileLine in _profileSession.ProfileLines)
                {
                    var segments = GetLineSegments(profileSessionProfileLine.Id);
                    if(segments == null)
                    {
                        var profileSurfacePoints = _profileSession.ProfileSurfaces.FirstOrDefault(surface =>
                      surface.LineId == profileSessionProfileLine.Id).ProfileSurfacePoints;

                        extremePoints.Add(profileSurfacePoints.Last());
                    }
                    else
                    {
                        var observerHeight = _surfaceProfileChart.ProfilesProperties
                                                        .First(property => property.LineId == profileSessionProfileLine.Id)
                                                        .ObserverHeight;

                        AddVertexPointsToLine(segments, observerHeight);
                    }
                }
            }
            else
            {
                var profileSurfacePoints = profileSurface.ProfileSurfacePoints;

                var segments = GetLineSegments(profileSurface.LineId);
                if(segments == null)
                {
                    extremePoints.Add(profileSurfacePoints.Last());
                }
                else
                {
                    var observerHeight = _surfaceProfileChart.ProfilesProperties
                                                        .First(property => property.LineId == profileSurface.LineId)
                                                        .ObserverHeight;

                    AddVertexPointsToLine(segments, observerHeight);
                }
            }

            return extremePoints;
        }

        private static double FindLength(ProfileSurfacePoint[] profileSurfacePoints)
        {
            double result = 0;

            for(var i = 0; i < profileSurfacePoints.Length - 1; i++)
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

            if(invisibleSurface.ProfileSurfacePoints.Count() == 0)
            {
                _surfaceProfileChart.ProfilesProperties
                                    .First(property => property.LineId == invisibleSurface.LineId).VisiblePercent = 100;

                return;
            }

            for(int i = 0; i < allSurface.ProfileSurfacePoints.Count() - 1; i++)
            {
                if(allSurface.ProfileSurfacePoints[i] == invisibleSurface.ProfileSurfacePoints[j])
                {
                    invisibleLegth += CalcVectorLength(allSurface.ProfileSurfacePoints[i],
                   allSurface.ProfileSurfacePoints[i + 1]);
                    j++;
                }

                if(j >= invisibleSurface.ProfileSurfacePoints.Count())
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

            for(var i = 0; i < profileSurfacePoints.Length - 2; i++)
            {
                if(CalcAngleOfDeviation(profileSurfacePoints[i], profileSurfacePoints[i + 1]) !=
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

        private static bool IsLinesIntersected(ProfileSurfacePoint firstStartPoint, ProfileSurfacePoint firstEndPoint,
                                                        ProfileSurfacePoint secondStartPoint, ProfileSurfacePoint secondEndPoint)
        {
            const double epsilon = 1e-10;

            var firstLineVectorDiff = PointsDiff(firstEndPoint, firstStartPoint);
            var secondLineVectorDiff = PointsDiff(secondEndPoint, secondStartPoint);
            var startPointsDiff = PointsDiff(secondStartPoint, firstStartPoint);

            var diffCross = VectorCross(firstLineVectorDiff, secondLineVectorDiff);
            var startPointsCrossToFirst = VectorCross(startPointsDiff, firstLineVectorDiff);

            if(Math.Abs(diffCross) < epsilon && Math.Abs(startPointsCrossToFirst) < epsilon)
            {
                var firstLineProduct = PointsProduct(startPointsDiff, firstLineVectorDiff);
                var secondLineProduct = PointsProduct(PointsDiff(firstStartPoint, secondStartPoint), secondLineVectorDiff);

                if((firstLineProduct >= 0 && firstLineProduct <= VectorCross(firstLineVectorDiff, firstLineVectorDiff))
                        || (secondLineProduct >= 0 && secondLineProduct <= VectorCross(secondLineVectorDiff, secondLineVectorDiff)))
                {
                    return true;
                }

                return false;
            }
            var t = VectorCross(startPointsDiff, secondLineVectorDiff) / diffCross;
            var u = startPointsCrossToFirst / diffCross;

            if(!(Math.Abs(diffCross) < epsilon) && (0 <= t && t <= 1) && (0 <= u && u <= 1))
            {
                return true;
            }

            return false;
        }

        private static ProfileSurfacePoint PointsDiff(ProfileSurfacePoint leftPoint, ProfileSurfacePoint rightPoint)
        {
            return new ProfileSurfacePoint { Distance = rightPoint.Distance - leftPoint.Distance, Z = rightPoint.Z - leftPoint.Z };
        }

        private static double PointsProduct(ProfileSurfacePoint leftPoint, ProfileSurfacePoint rightPoint)
        {
            return rightPoint.Distance * leftPoint.Distance + rightPoint.Z * leftPoint.Z;
        }

        private static double VectorCross(ProfileSurfacePoint leftPoint, ProfileSurfacePoint rightPoint)
        {
            return leftPoint.Distance * rightPoint.Z - leftPoint.Z * rightPoint.Distance;
        }

        private static double CalcAngleOfVisibility(double observerHeight, ProfileSurfacePoint leftPoint,
                                                     ProfileSurfacePoint rightPoint, double firstPointDistance)
        {
            var sightLineKoef = (rightPoint.Z - observerHeight) / (rightPoint.Distance - firstPointDistance);
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
            if(angle > 90)
            {
                return 90 - angle;
            }

            if(angle < -90)
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

        private static List<ProfileLine> GetLines(ProfileSurface allSurface, ProfileSurface invisibleSurface, int sessionId)
        {
            var profileLines = new List<ProfileLine>();

            var lineId = 1;

            var j = 0;

            var isInvisiblePointsFinished = false;

            if(invisibleSurface.ProfileSurfacePoints.Count() == 0)
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

            for(int i = 0; i < allSurface.ProfileSurfacePoints.Count(); i++)
            {
                if(!isInvisiblePointsFinished && allSurface.ProfileSurfacePoints[i] == invisibleSurface.ProfileSurfacePoints[j])
                {
                    if(profileInvisiblePoints.Count == 0)
                    {
                        StartOfLineHandler(ref profileInvisibleLine, lineId, allSurface, i);
                        lineId++;

                        if(profileVisiblePoints.Count > 0)
                        {
                            profileLines.Add(EndOfLineHandler(profileVisibleLine, allSurface, i));

                            profileVisibleLine = new ProfileLine
                            {
                                Visible = true
                            };

                            profileVisiblePoints = new List<ProfileSurfacePoint>();
                        }
                    }

                    if(i == allSurface.ProfileSurfacePoints.Count() - 1)
                    {
                        profileLines.Add(EndOfLineHandler(profileInvisibleLine, allSurface, i));

                        profileInvisibleLine = new ProfileLine
                        {
                            Visible = false
                        };

                        profileInvisiblePoints = new List<ProfileSurfacePoint>();
                    }

                    if(j == invisibleSurface.ProfileSurfacePoints.Count() - 1)
                    {
                        isInvisiblePointsFinished = true;
                    }

                    profileInvisiblePoints.Add(allSurface.ProfileSurfacePoints[i]);
                    j++;
                }
                else
                {
                    if(profileVisiblePoints.Count == 0)
                    {
                        StartOfLineHandler(ref profileVisibleLine, lineId, allSurface, i);

                        lineId++;

                        if(profileInvisiblePoints.Count > 0)
                        {
                            profileLines.Add(EndOfLineHandler(profileInvisibleLine, allSurface, i));

                            profileInvisibleLine = new ProfileLine
                            {
                                Visible = false
                            };

                            profileInvisiblePoints = new List<ProfileSurfacePoint>();
                        }
                    }

                    if(i == allSurface.ProfileSurfacePoints.Count() - 1)
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

            return profileLines;
        }

        private static void StartOfLineHandler(ref ProfileLine startLine,
                                                int startLineId, ProfileSurface allSurfaces, int pointIndex)
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
