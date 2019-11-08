using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MilSpace.DataAccess.DataTransfer;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using MilSpace.Profile.Localization;

namespace MilSpace.Profile.SurfaceProfileChartControl
{
    public partial class SurfaceProfileChart : UserControl
    {
        private SurfaceProfileChartController _controller;
        private bool _isObserverHeightIgnore = false;
        private bool _isCommentDisplay = false;
        private int _maxObserverHeightIndex = 0;
        private CheckBox _checkBoxHeader;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<ProfileProperties> ProfilesProperties { get; set; }
        public bool Current { get; set; }
        public int SelectedLineId { get; set; }

        public SurfaceProfileChart(SurfaceProfileChartController controller)
        {
            Current = false;
            SelectedLineId = -1;

            _controller = controller;
            ProfilesProperties = new List<ProfileProperties>();

            InitializeComponent();
            LocalizeControls();

            SetTableView();
            SetDetailsView();
            SetControlSize();

            profilePropertiesTable.RowTemplate.Height = 18;
            profileChart.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        }

        #region Initialization

        internal void InitializeGraph()
        {
            _controller.LoadSeries();
            _controller.SetProfilesProperties();
            SetProfileView();

            _controller.AddInvisibleZones(GetAllColors(true), GetAllColors(false));
            _controller.AddExtremePoints();

            FillPropertiesTable();
            AddCheckBoxHeader();

            if(ProfilesProperties.Count == 1)
            {
                SelectProfile("1");
            }
        }

        internal void InitializeProfile()
        {
            _controller.LoadSerie();
            _controller.AddProfileProperty();

            SetProfileView();

            var heights = new Dictionary<int, double>();

            for(int i = 0; i < ProfilesProperties.Count; i++)
            {
                heights.Add(ProfilesProperties[i].LineId, ProfilesProperties[i].ObserverHeight);
            }

            ProfileSurface profileSurface = GetSurfacesFromChart().Last();
            _controller.AddInvisibleZone(heights[profileSurface.LineId], profileSurface, profileChart.Series.Last().Color,
                                                                        profileChart.Series.Last().BackSecondaryColor, false);

            _controller.AddExtremePoints(profileSurface);
            AddPropertyRow(ProfilesProperties.Last());
            UpdateIntersectionsLine();
        }

        internal void IsGraphEmptyHandler(bool isEmpty)
        {
            foreach(ToolBarButton button in graphToolBar.Buttons)
            {
                button.Enabled = !isEmpty;
            }

            graphToolBar.Buttons["deletePageGraphToolBarBtn"].Enabled = true;
            graphToolBar.Buttons["addProfileGraphToolBarButton"].Enabled = true;
            propertiesPanel.Enabled = !isEmpty;
        }

        internal void InitializeGraph(ProfileSession profileSession)
        {
            profileChart.Series.Clear();

            foreach(var line in profileSession.ProfileLines)
            {
                var profileSurface =
                    profileSession.ProfileSurfaces.First(surface => surface.LineId == line.Id);

                AddSerie(profileSurface);
            }

            profileNameLabel.Text = $"{LocalizationConstants.SelectedProfileNameText} {profileSession.SessionName}";
            profileNameLabel.Tag = profileSession.SessionId;
        }

        internal void AddSerie(ProfileSurface profileSurface)
        {
            profileChart.Series.Add(new Series
            {
                ChartType = SeriesChartType.Line,
                Color = Color.ForestGreen,
                BackSecondaryColor = Color.Red,
                Name = profileSurface.LineId.ToString(),
                YValuesPerPoint = 1,
                IsVisibleInLegend = false,
                //TODO: Create  a local var to store profileSurface
                Tag = profileSurface
            });

            int i = 1;
            var segments = new List<ProfileSurface>();
            var points = new List<ProfileSurfacePoint>();

            foreach(var point in profileSurface.ProfileSurfacePoints)
            {
                profileChart.Series.Last().Points.AddXY(point.Distance, point.Z);
                points.Add(point);

                if(point.isVertex && point != profileSurface.ProfileSurfacePoints.First() && point != profileSurface.ProfileSurfacePoints.Last())
                {
                    profileChart.Series.Last().Points.Last().MarkerStyle = MarkerStyle.Circle;
                    profileChart.Series.Last().Points.Last().MarkerColor = Color.DeepSkyBlue;
                    profileChart.Series.Last().Points.Last().Name = $"Vertex{i}";

                    i++;

                    segments.Add(new ProfileSurface
                    {
                        LineId = profileSurface.LineId,
                        ProfileSurfacePoints = points.ToArray()
                    });

                    points = new List<ProfileSurfacePoint>();
                    points.Add(point);
                }
            }

            if(segments.Count > 0)
            {
                segments.Add(new ProfileSurface
                {
                    LineId = profileSurface.LineId,
                    ProfileSurfacePoints = points.ToArray()
                });

                _controller.SetSurfaceSegments(segments);
            }

            profileChart.Series.Last().Points.First().MarkerStyle = MarkerStyle.Circle;
            profileChart.Series.Last().Points.Last().MarkerStyle = MarkerStyle.Circle;
        }

        internal void AddInvisibleLine(ProfileSurface surface)
        {
            foreach(var point in surface.ProfileSurfacePoints)
            {
                profileChart.Series[surface.LineId.ToString()]
                        .Points
                        .FirstOrDefault(linePoint => (linePoint.XValue.Equals(point.Distance)))
                        .Color = profileChart.Series[surface.LineId.ToString()].BackSecondaryColor;
            }
        }

        internal void SetExtremePoints(List<ProfileSurfacePoint> extremePoints, int id = 0)
        {
            if(extremePoints.Count == 0)
            {
                return;
            }

            if(id == 0)
            {
                for(var i = 0; i < extremePoints.Count; i++)
                {
                    var observerPoint = new ProfileSurfacePoint
                    {
                        Distance = 0,
                        Z = GetObserverPointFullHeight(ProfilesProperties[i].LineId)
                    };

                    AddExtremePoint(observerPoint, extremePoints[i], i + 1);
                }
            }
            else
            {
                var observerPoint = new ProfileSurfacePoint
                {
                    Distance = 0,
                    Z = GetObserverPointFullHeight(id)
                };

                AddExtremePoint(observerPoint, extremePoints[0], id);
            }

        }

        internal void AddExtremePoint(ProfileSurfacePoint observerPoint, ProfileSurfacePoint observationPoint, int order)
        {
            CreateExtremePointsSerie(order);

            profileChart.Series[$"ExtremePointsLine{order}"].Points.AddXY(observerPoint.Distance, observerPoint.Z);
            profileChart.Series[$"ExtremePointsLine{order}"].Points[0].MarkerStyle = MarkerStyle.Circle;

            profileChart.Series[$"ExtremePointsLine{order}"].Points.AddXY(observationPoint.Distance, observationPoint.Z);

            if(profileChart.Series[$"{order}"].Points.Last().Color == profileChart.Series[$"{order}"].BackSecondaryColor)
            {
                profileChart.Series[$"ExtremePointsLine{order}"].Points[1].MarkerColor = Color.Red;
            }
            profileChart.Series[$"ExtremePointsLine{order}"].Points[1].MarkerStyle = MarkerStyle.Circle;
        }

        internal void AddVertexPoint(ProfileSurfacePoint point, bool isVisible, int lineId, double observerHeight)
        {
            var vertexPoint = new DataPoint(point.Distance, point.Z + observerHeight)
            {
                MarkerStyle = MarkerStyle.Circle,
            };

            if(!isVisible)
            {
                vertexPoint.MarkerColor = Color.Red;
            }

            CreateExtremePointsSerie(lineId);
            profileChart.Series[$"ExtremePointsLine{lineId}"].Points.Add(vertexPoint);
        }

        private void LocalizeControls()
        {
            observerHeightLabel.Text = LocalizationConstants.ObserverHeightText;
            visibleLineColorLabel.Text = LocalizationConstants.VisibleLineColorText;
            invisibleLineColorLabel.Text = LocalizationConstants.InvisibleLineColorText;

            copyStripMenuItem.Text = LocalizationConstants.CopyStripMenuItemText;
            toolTip.SetToolTip(profileDetailsListView, LocalizationConstants.ProfileDetailsToolTip);
            toolTip.SetToolTip(changeAllObserversHeightsButton, LocalizationConstants.ChangeAllObserversHeightsToolTip);

            graphToolBar.Buttons["displayProfileSignatureGraphToolBarBtn"].ToolTipText = LocalizationConstants.DisplayProfileSignatureToolTip;
            graphToolBar.Buttons["deleteSelectedProfileGraphToolBarBtn"].ToolTipText = LocalizationConstants.DeleteSelectedProfileToolTip;
            graphToolBar.Buttons["addProfileGraphToolBarButton"].ToolTipText = LocalizationConstants.AddProfileGraphToolTip;
            graphToolBar.Buttons["panToSelectedProfileGraphToolBarBtn"].ToolTipText = LocalizationConstants.PanToSelectedProfileToolTip;
            graphToolBar.Buttons["panGraphToolBarBtn"].ToolTipText = LocalizationConstants.PanToProfilesSetToolTip;
            graphToolBar.Buttons["showAllProfilesGraphToolBarBtn"].ToolTipText = LocalizationConstants.ShowAllProfilesToolTip;
            graphToolBar.Buttons["observerHeightIgnoreGraphToolBarBtn"].ToolTipText = LocalizationConstants.ObserverHeightIgnoreToolTip;
            graphToolBar.Buttons["zoomInGraphToolBarBtn"].ToolTipText = LocalizationConstants.ZoomInToolTip;
            graphToolBar.Buttons["zoomOutGraphToolBarBtn"].ToolTipText = LocalizationConstants.ZoomOutToolTip;
            graphToolBar.Buttons["addPageGraphToolBarBtn"].ToolTipText = LocalizationConstants.AddPageToolTip;
            graphToolBar.Buttons["deletePageGraphToolBarBtn"].ToolTipText = LocalizationConstants.DeletePageToolTip;
            graphToolBar.Buttons["saveGraphToolBarBtn"].ToolTipText = LocalizationConstants.SaveToolTip;
            graphToolBar.Buttons["updateIntersectionsLinesGraphToolBarBtn"].ToolTipText = LocalizationConstants.UpdateIntersectionsLinesToolTip;

            //profilePropertiesTable
            profilePropertiesTable.Columns["IsVisibleCol"].ToolTipText = LocalizationConstants.ProfilePropertiesIsVisibleColToolTip;
            profilePropertiesTable.Columns["IsVisibleCol"].HeaderText = LocalizationConstants.ProfilePropertiesIsVisibleColHeader;  

            profilePropertiesTable.Columns["ProfileNumberCol"].ToolTipText = LocalizationConstants.ProfilePropertiesProfileNumberColToolTip;
            profilePropertiesTable.Columns["ProfileNumberCol"].HeaderText = LocalizationConstants.ProfilePropertiesProfileNumberColHeader;

            profilePropertiesTable.Columns["AzimuthCol"].ToolTipText = LocalizationConstants.ProfilePropertiesAzimuthColToolTip;
            profilePropertiesTable.Columns["AzimuthCol"].HeaderText = LocalizationConstants.ProfilePropertiesAzimuthColHeader;

            profilePropertiesTable.Columns["ObserverHeightCol"].ToolTipText = LocalizationConstants.ProfilePropertiesObserverHeightColToolTip;
            profilePropertiesTable.Columns["ObserverHeightCol"].HeaderText = LocalizationConstants.ProfilePropertiesObserverHeightColHeader;

            profilePropertiesTable.Columns["ProfileLengthCol"].ToolTipText = LocalizationConstants.ProfilePropertiesProfileLengthColToolTip;
            profilePropertiesTable.Columns["ProfileLengthCol"].HeaderText = LocalizationConstants.ProfilePropertiesProfileLengthColHeader;

            profilePropertiesTable.Columns["MinHeightCol"].ToolTipText = LocalizationConstants.ProfilePropertiesMinHeightColToolTip;
            profilePropertiesTable.Columns["MinHeightCol"].HeaderText = LocalizationConstants.ProfilePropertiesMinHeightColHeader;

            profilePropertiesTable.Columns["MaxHeightCol"].ToolTipText = LocalizationConstants.ProfilePropertiesMaxHeightColToolTip;
            profilePropertiesTable.Columns["MaxHeightCol"].HeaderText = LocalizationConstants.ProfilePropertiesMaxHeightColHeader;

            profilePropertiesTable.Columns["HeightDifferenceCol"].ToolTipText = LocalizationConstants.ProfilePropertiesHeightDifferenceColToolTip;
            profilePropertiesTable.Columns["HeightDifferenceCol"].HeaderText = LocalizationConstants.ProfilePropertiesHeightDifferenceColHeader;

            profilePropertiesTable.Columns["DescendingAngleCol"].ToolTipText = LocalizationConstants.ProfilePropertiesDescendingAngleColToolTip;
            profilePropertiesTable.Columns["DescendingAngleCol"].HeaderText = LocalizationConstants.ProfilePropertiesDescendingAngleColHeader;

            profilePropertiesTable.Columns["AscendingAngleCol"].ToolTipText = LocalizationConstants.ProfilePropertiesAscendingAngleColToolTip;
            profilePropertiesTable.Columns["AscendingAngleCol"].HeaderText = LocalizationConstants.ProfilePropertiesAscendingAngleColHeader;

            profilePropertiesTable.Columns["VisiblePercentCol"].ToolTipText = LocalizationConstants.ProfilePropertiesVisiblePercentColToolTip;
            profilePropertiesTable.Columns["VisiblePercentCol"].HeaderText = LocalizationConstants.ProfilePropertiesVisiblePercentColHeader;
        }

        private void CreateExtremePointsSerie(int lineId)
        {
            if(profileChart.Series.FindByName($"ExtremePointsLine{lineId}") != null)
            {
                return;
            }

            profileChart.Series.Add(new Series
            {
                ChartType = SeriesChartType.Line,
                Color = Color.DarkGray,
                Name = $"ExtremePointsLine{lineId}",
                YValuesPerPoint = 1,
                IsVisibleInLegend = false
            });

            profileChart.Series[$"ExtremePointsLine{lineId}"].BorderDashStyle = ChartDashStyle.Dash;
        }

        private void FillPropertiesTable(List<ProfileProperties> properties = null)
        {
            if(properties == null)
            {
                properties = ProfilesProperties;
            }

            profilePropertiesTable.Rows.Clear();

            foreach(var profileProperties in properties)
            {
                AddPropertyRow(profileProperties);
            }
        }

        private void AddPropertyRow(ProfileProperties properties)
        {
            var serie = profileChart.Series[properties.LineId.ToString()];
            profilePropertiesTable.Rows.Add(
                        serie.Enabled,
                        properties.LineId,
                        properties.Azimuth == double.MinValue ? "" : Math.Round(properties.Azimuth, 0).ToString(),
                        Math.Round(properties.ObserverHeight, 0),
                        Math.Round(properties.PathLength, 0),
                        Math.Round(properties.MinHeight, 0),
                        Math.Round(properties.MaxHeight, 0),
                        Math.Round(properties.MaxHeight - properties.MinHeight, 0),
                        Math.Round(properties.MinAngle, 0),
                        Math.Round(properties.MaxAngle, 0),
                        Math.Round(properties.VisiblePercent, 0));
        }

        private void ShowDetails()
        {
            var surface = (ProfileSurface)profileChart.Series[SelectedLineId.ToString()].Tag;
            var property = ProfilesProperties.First(profileProperty => profileProperty.LineId == SelectedLineId);
            var session = _controller.GetProfileSessionForLine(SelectedLineId);

            var sharedText = session.Shared ? LocalizationConstants.ProfilesSetSharedText : LocalizationConstants.ProfilesSetNotSharedText;

             profileDetailsListView.Items.Clear();

            profileDetailsListView.Items.Add(CreateNewItem(LocalizationConstants.ProfileDetailsStateText, $"{sharedText}"));
            profileDetailsListView.Items
                                    .Add(CreateNewItem(LocalizationConstants.ProfileDetailsEndPointsText,
                                        $"{Math.Round(surface.ProfileSurfacePoints[0].X, 5)};"
                                        + $"{Math.Round(surface.ProfileSurfacePoints[0].Y, 5)}"
                                        + $" - {Math.Round(surface.ProfileSurfacePoints.Last().X, 5)};"
                                        + $"{Math.Round(surface.ProfileSurfacePoints.Last().Y, 5)}"));
            profileDetailsListView.Items
                                    .Add(CreateNewItem(LocalizationConstants.ProfileDetailsAzimuthText,
                                    property.Azimuth == double.MinValue ? "" :
                                    $"{Math.Round(property.Azimuth, 1)}"));
            profileDetailsListView.Items
                                    .Add(CreateNewItem(LocalizationConstants.ProfileDetailsLengthText,
                                            $"{Math.Round(property.PathLength, 0)}"));
            profileDetailsListView.Items
                                    .Add(CreateNewItem(LocalizationConstants.ProfileDetailsHeightText,
                                            $"{Math.Round(property.MinHeight, 0)}"
                                            + $"-{Math.Round(property.MaxHeight, 0)}"));
            profileDetailsListView.Items
                                    .Add(CreateNewItem(LocalizationConstants.ProfileDetailsAnglesText,
                                        $"{Math.Round(property.MaxAngle, 1)}"
                                        + $"-{Math.Round(property.MinAngle, 1)}"));
            profileDetailsListView.Items
                                    .Add(CreateNewItem(LocalizationConstants.ProfileDetailsVisibilityText,
                                            $"{Math.Round(property.VisiblePercent, 2)}"));
            profileDetailsListView.Items
                                    .Add(CreateNewItem($"{session.CreatedBy}", $"{session.CreatedOn}"));
        }

        #endregion

        #region Intersections

        internal void AddIntersectionsLine(double startPoint, double lastPoint, Color color, string layerName)
        {
            string name = $"Intersections {layerName}";

            var layerIntersectionLine = profileChart.Series.FindByName(name);
            int i = 1;

            while(profileChart.Series.FindByName(name) != null)
            {
                name = $"Intersections {layerName}{i}";
                i++;
            }
            Series intersectionLine = new Series()
            {
                ChartType = SeriesChartType.StepLine,
                Color = color,
                Name = name,
                BorderWidth = 9,
                MarkerStyle = MarkerStyle.Square,
                MarkerSize = 8,
                YValuesPerPoint = 1,
                IsVisibleInLegend = false,
            };

            profileChart.Series.Add(intersectionLine);

            AddIntersectionPoint(profileChart.Series[name].Color, startPoint, name);
            AddIntersectionPoint(profileChart.Series[name].Color, lastPoint, name);
        }
        internal void DrawIntersections(List<IntersectionsInLayer> intersectionLines, double maxDistance)
        {
            foreach(var layer in intersectionLines)
            {
                foreach(var line in layer.Lines)
                {
                    AddIntersectionsLine(line.PointFromDistance, line.PointToDistance, layer.LineColor, line.LayerType.ToString());
                }
            }
        }

        internal void ClearIntersectionLines()
        {
            var intersectionLines = new List<Series>();
            intersectionLines = profileChart.Series.Where(serie => serie.Name.Contains("Intersections")).ToList();

            if(intersectionLines == null) { return; }

            foreach(var serie in intersectionLines)
            {
                profileChart.Series.Remove(serie);
            }
        }

        private void AddIntersectionPoint(Color color, double x, string serieName)
        {
            var yValue = new double[1] { profileChart.ChartAreas["Default"].AxisY.Minimum
                                            + profileChart.Series[serieName].BorderWidth/4};

            var point = new DataPoint()
            {
                Color = color,
                XValue = x,
                YValues = yValue,
                MarkerStyle = MarkerStyle.Square,
                MarkerBorderColor = color,
                MarkerBorderWidth = 1
            };

            profileChart.Series[serieName].Points.Add(point);
        }

        private void ShowIntersectionLines()
        {
            var intersectionLines = new List<Series>();
            intersectionLines = profileChart.Series.Where(serie => serie.Name.Contains("Intersections")).ToList();

            if(intersectionLines == null) { return; }

            foreach(var serie in intersectionLines)
            {
                serie.Enabled = true;
            }
        }

        private void HideIntersectionLines()
        {
            var intersectionLines = new List<Series>();
            intersectionLines = profileChart.Series.Where(serie => serie.Name.Contains("Intersections")).ToList();

            if(intersectionLines == null) { return; }

            foreach(var serie in intersectionLines)
            {
                serie.Enabled = false;
            }
        }

        private void GetIntersection(int lineId)
        {
            _controller.InvokeGetIntersectionLine(lineId);
        }

        #endregion

        #region SetComponentsView

        internal void SetControlSize()
        {
            SetPropertiesContainersSize();

            graphPanel.Width = Width - propertiesPanel.Width;
            graphPanel.Height = Height;

            profileChart.Width = graphPanel.Width - graphPanel.Padding.Right;
            profileChart.Height = graphPanel.Height - graphToolBar.Height;
        }

        private void SetPropertiesContainersSize()
        {
            propertiesPanel.Width = profilePropertiesTable.Width
                                                + propertiesPanel.Padding.Left
                                                + propertiesPanel.Padding.Right;

            propertiesSplitContainer.Width = propertiesPanel.Width
                                                - propertiesPanel.Padding.Left
                                                - propertiesPanel.Padding.Right;

            propertiesSettingsPanel.Width = propertiesSplitContainer.Width;
        }

        private void SetProfileView()
        {
            profileChart.ChartAreas["Default"].AxisX.LabelStyle.Format = "#";
            profileChart.ChartAreas["Default"].AxisY.LabelStyle.Format = "#";

            profileChart.ChartAreas["Default"].Position = new ElementPosition(0, 4, 96, 90);
            profileChart.ChartAreas["Default"].InnerPlotPosition = new ElementPosition(4, 0, 96, 95);

            profileChart.ChartAreas["Default"].AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;

            profileChart.Size = this.Size;

            SetAxisView();
        }

        private void SetTableView()
        {
            var fontPixelSize = profilePropertiesTable.Font.Size * 1.3;

            var twoPositionWidth = (int)Math.Round((fontPixelSize * 2 + 0.4), 0);
            var threePositionWidth = (int)Math.Round((fontPixelSize * 3), 0);
            var fourPoristionWidth = (int)Math.Round((fontPixelSize * 4), 0);
            var fivePoristionWidth = (int)Math.Round((fontPixelSize * 5), 0);

            profilePropertiesTable.Columns["ProfileNumberCol"].Width = twoPositionWidth;
            profilePropertiesTable.Columns["ProfileLengthCol"].Width = fivePoristionWidth;
            profilePropertiesTable.Columns["AzimuthCol"].Width = fourPoristionWidth;
            profilePropertiesTable.Columns["ObserverHeightCol"].Width = fourPoristionWidth;
            profilePropertiesTable.Columns["MinHeightCol"].Width = threePositionWidth;
            profilePropertiesTable.Columns["MaxHeightCol"].Width = threePositionWidth;
            profilePropertiesTable.Columns["HeightDifferenceCol"].Width = twoPositionWidth;
            profilePropertiesTable.Columns["DescendingAngleCol"].Width = twoPositionWidth;
            profilePropertiesTable.Columns["AscendingAngleCol"].Width = twoPositionWidth;
            profilePropertiesTable.Columns["VisiblePercentCol"].Width = threePositionWidth;

            var tableWidth = 0;

            foreach(DataGridViewColumn column in profilePropertiesTable.Columns)
            {
                tableWidth += column.Width;
            }

            profilePropertiesTable.Width = tableWidth + 10;
            profilePropertiesTable.Columns["ProfileLengthCol"].AutoSizeMode
                                                    = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void SetDetailsView()
        {
            profileDetailsListView.View = View.Details;

            profileDetailsListView.Columns.Add("Attribute", (int)(profileDetailsListView.Width * 0.32));
            profileDetailsListView.Columns.Add("Value", (profileDetailsListView.Width - profileDetailsListView.Columns[0].Width - 25));

            profileDetailsListView.HeaderStyle = ColumnHeaderStyle.None;
        }

        private void SetAxisView()
        {
            SetAxisSizes();

            SetAxisInterval(profileChart.ChartAreas["Default"].AxisY);
            SetAxisInterval(profileChart.ChartAreas["Default"].AxisX);
        }

        private void SetAxisSizes()
        {
            var serie = GetProfiles();

            if(ProfilesProperties.Count == 1)
            {
                var height = ProfilesProperties[0].MaxHeight;

                if(!_isObserverHeightIgnore)
                {
                    height += ProfilesProperties[0].ObserverHeight;
                }

                profileChart.ChartAreas["Default"].AxisY.Maximum = height + height / 10;
                profileChart.ChartAreas["Default"].AxisY.Minimum = ProfilesProperties[0].MinHeight
                    - height / 10;

                profileChart.ChartAreas["Default"].AxisX.Maximum = serie[0].Points.Last().XValue
                  + serie[0].Points.Last().XValue / 10;
            }
            else
            {
                double maxHeight = ProfilesProperties.Max(profileProperties => profileProperties.MaxHeight);

                if(!_isObserverHeightIgnore)
                {
                    maxHeight += ProfilesProperties.Max(property => property.ObserverHeight);
                }

                double minHeight = ProfilesProperties.Min(profileProperties => profileProperties.MinHeight);
                double absHeight = maxHeight - minHeight;

                profileChart.ChartAreas["Default"].AxisY.Maximum = maxHeight + absHeight / 10;
                profileChart.ChartAreas["Default"].AxisY.Minimum = minHeight - absHeight / 10;


                double maxWidth = serie.Max(profile => profile.Points.Last().XValue);

                profileChart.ChartAreas["Default"].AxisX.Maximum = maxWidth + maxWidth / 30;
            }

            SetAxisYMinValue();
        }

        private void SetAxisYMinValue()
        {
            var size = profileChart.ChartAreas["Default"].AxisY.Maximum
                             - profileChart.ChartAreas["Default"].AxisY.Minimum;

            if(size < 10)
            {
                profileChart.ChartAreas["Default"].AxisY.Minimum -=
               (profileChart.ChartAreas["Default"].AxisY.Minimum % 2);

                return;
            }

            if(profileChart.ChartAreas["Default"].AxisY.Minimum % 10 > 5)
            {
                profileChart.ChartAreas["Default"].AxisY.Minimum -=
               (profileChart.ChartAreas["Default"].AxisY.Minimum % 5);
            }
            else
            {
                profileChart.ChartAreas["Default"].AxisY.Minimum -=
               (profileChart.ChartAreas["Default"].AxisY.Minimum % 10);
            }
        }

        private void SetAxisInterval(Axis axis)
        {
            var axisSize = Math.Truncate(axis.Maximum - axis.Minimum);
            int intervalSize;

            if(axisSize <= 5) { axis.Interval = 1; return; }
            if(axisSize <= 10) { axis.Interval = 2; return; }
            if(axisSize <= 20) { axis.Interval = 5; return; }

            if(axisSize <= 50)
            {
                intervalSize = (int)Math.Truncate(axisSize / (axisSize / 10));
            }
            else
            {
                intervalSize = (int)Math.Truncate(axisSize / 5);
            }

            int intervalKoef;

            if(intervalSize <= 50) { intervalKoef = 10; }
            else if(intervalSize <= 250) { intervalKoef = 50; }
            else if(intervalSize <= 500) { intervalKoef = 100; }
            else if(intervalSize <= 1000) { intervalKoef = 200; }
            else if(intervalSize <= 2500) { intervalKoef = 500; }
            else { intervalKoef = 1000; }

            axis.Interval = intervalSize - (intervalSize % intervalKoef);
        }

        private void AddCheckBoxHeader()
        {
            Rectangle checkBoxColumnRectangle = profilePropertiesTable.GetCellDisplayRectangle(0, -1, true);

            _checkBoxHeader = new CheckBox();

            _checkBoxHeader.Name = "checkBoxHeader";
            _checkBoxHeader.BackColor = profilePropertiesTable.ColumnHeadersDefaultCellStyle.BackColor;
            _checkBoxHeader.Size = new Size(15, 15);
            _checkBoxHeader.CheckState = CheckState.Checked;

            _checkBoxHeader.Location = new Point(checkBoxColumnRectangle.X + 4, checkBoxColumnRectangle.Y + 4);

            _checkBoxHeader.CheckedChanged += new EventHandler(CheckBoxHeader_CheckedChanged);

            profilePropertiesTable.Controls.Add(_checkBoxHeader);
        }

        #endregion

        #region Updates

        private void UpdateIntersectionsLine()
        {
            var lines = profileChart.Series.Where(serie => serie.Name.Contains("Intersections")).ToList();

            if(lines.Count > 0)
            {
                var yValue = profileChart.ChartAreas["Default"].AxisY.Minimum
                                            + lines[0].BorderWidth / 4;

                foreach(var line in lines)
                {
                    foreach(var point in line.Points)
                    {
                        point.SetValueY(yValue);
                    }
                }
            }
        }

        private void UpdateExtremePoints(SeriesCollection series)
        {
            for(int i = 0; i < GetProfiles().Count(); i++)
            {
                UpdateProfileExtremePoints(ProfilesProperties[i].LineId);
            }
        }

        private void UpdateProfileExtremePoints(int lineId)
        {
            var segments = _controller.GetLineSegments(lineId);
            if(segments != null)
            {
                profileChart.Series[$"ExtremePointsLine{lineId}"].Points.Clear();
                _controller.AddVertexPointsToLine(segments, ProfilesProperties.First(property => property.LineId == lineId).ObserverHeight);
            }
            else
            {
                var serieName = profileChart.Series[lineId.ToString()].Name;
                profileChart.Series[$"ExtremePointsLine{serieName}"].Points[0].SetValueY(GetObserverPointFullHeight(lineId));

                if(profileChart.Series[$"{serieName}"].Points.Last().Color == profileChart.Series[$"{serieName}"].BackSecondaryColor)
                {
                    profileChart.Series[$"ExtremePointsLine{serieName}"].Points[1].MarkerColor = Color.Red;
                }
                else
                {
                    profileChart.Series[$"ExtremePointsLine{serieName}"].Points[1].MarkerColor = Color.DarkGray;
                }
            }

            if(!_isObserverHeightIgnore)
            {
                double diff = GetObserverPointFullHeight(lineId) - profileChart.ChartAreas["Default"].AxisY.Maximum;

                if(diff > 0)
                {
                    double newHeight = profileChart.ChartAreas["Default"].AxisY.Maximum + diff;
                    profileChart.ChartAreas["Default"].AxisY.Maximum =
                                            newHeight + (newHeight - profileChart.ChartAreas["Default"].AxisY.Minimum) / 10;
                }
                else
                {
                    SetAxisSizes();
                }

                SetAxisInterval(profileChart.ChartAreas["Default"].AxisY);
            }
        }

        private void UpdateProfiles()
        {
            var profiles = GetProfiles();

            foreach(Series serie in profiles)
            {
                UpdateProfile(serie.Name);
            }
        }

        private void UpdateProfile(string lineId)
        {
            foreach(var point in profileChart.Series[lineId].Points)
            {
                if(!point.Name.Contains("Vertex"))
                {
                    point.Color = profileChart.Series[lineId].Color;
                }
            }
        }

        private void UpdateProfileWithNewColor()
        {
            foreach(var point in profileChart.Series[SelectedLineId.ToString()].Points)
            {
                if(point.Color != profileChart.Series[SelectedLineId.ToString()].BackSecondaryColor)
                {
                    point.Color = profileChart.Series[SelectedLineId.ToString()].Color;
                }
            }
        }

        private void UpdateTableWithNewObserverHeigth(DataGridViewRowCollection rows)
        {
            for(int i = 0; i < profilePropertiesTable.Rows.Count; i++)
            {
                UpdateSelectedRowWithNewObserverHeigth(i);
            }
        }

        private void UpdateSelectedRowWithNewObserverHeigth(int index)
        {
            var lineId = Convert.ToInt32(profilePropertiesTable.Rows[index].Cells["ProfileNumberCol"].Value);
            var property = ProfilesProperties.First(profileProperty => profileProperty.LineId == lineId);

            profilePropertiesTable.Rows[index].Cells["ObserverHeightCol"].Value
                                         = Math.Round(property.ObserverHeight, 0);

            profilePropertiesTable.Rows[index].Cells["VisiblePercentCol"].Value
                                         = Math.Round(property.VisiblePercent, 0);
        }

        #endregion

        #region ChangeObserverHeight

        private void ChangeAllProfilesObserverPointHeights(double height)
        {
            for(int i = 0; i < ProfilesProperties.Count; i++)
            {
                ProfilesProperties[i].ObserverHeight = height;
            }

            UpdateProfiles();

            _controller.ChangeSessionsHeights(height);
            _controller.AddInvisibleZones(GetAllColors(true), GetAllColors(false), GetSurfacesFromChart());
            UpdateExtremePoints(profileChart.Series);
            UpdateTableWithNewObserverHeigth(profilePropertiesTable.Rows);
            ShowDetails();
        }

        private void ChangeOnlySelectedProfileObserverHeight(double height)
        {
            ProfilesProperties.First(property => property.LineId == SelectedLineId).ObserverHeight = height;

            ProfileSurface[] profileSurfaces = GetSurfacesFromChart();

            UpdateProfile(SelectedLineId.ToString());
            _controller.AddInvisibleZone(height, profileSurfaces.First(surface => surface.LineId == SelectedLineId),
                                             profileChart.Series[SelectedLineId.ToString()].Color,
                                             profileChart.Series[SelectedLineId.ToString()].BackSecondaryColor);
            UpdateProfileExtremePoints(SelectedLineId);
            UpdateSelectedRowWithNewObserverHeigth(GetSelectedProfileRowIndex());
        }

        private bool ChangeObseverPointHeight(bool changeAll = false)
        {
            if(Regex.IsMatch(observerHeightTextBox.Text, @"^\d+[,|\.]?\d*$"))
            {
                if(SelectedLineId != -1 && !changeAll)
                {
                    ChangeOnlySelectedProfileObserverHeight(Convert.ToDouble(observerHeightTextBox.Text.Replace('.', ',')));
                }
                else
                {
                    ChangeAllProfilesObserverPointHeights(Convert.ToDouble(observerHeightTextBox.Text.Replace('.', ',')));
                }

                return true;
            }

            return false;
        }

        #endregion

        #region Helpers

        internal bool IsPointVisible(int lineId, int pointNumber)
        {
            var serie = profileChart.Series[lineId.ToString()];
            var index = (pointNumber == serie.Points.Count - 1) ? pointNumber : pointNumber + 1;

            if(serie.Points[index].Color == serie.Color)
            {
                return true;
            }

            return false;
        }

        internal string GetSelectedRowData()
        {
            var properies = new StringBuilder();

            var row = profilePropertiesTable.Rows[GetSelectedProfileRowIndex()];

            foreach(DataGridViewCell cell in row.Cells)
            {
                if(cell != row.Cells[0])
                {
                    properies.Append($"{cell.Value};");
                }
            }

            return properies.ToString().Remove(properies.Length - 1);
        }

        private ProfileSurface[] GetSurfacesFromChart()
        {
            var profiles = GetProfiles();
            ProfileSurface[] profileSurfaces = new ProfileSurface[profiles.Count];

            for(int i = 0; i < profiles.Count(); i++)
            {
                profileSurfaces[i] = (ProfileSurface)profiles[i].Tag;
            }

            return profileSurfaces;
        }

        private List<Series> GetProfiles()
        {
            var profiles = new List<Series>();
            var profileSeries = profileChart.Series.Where(serie => Regex.IsMatch(serie.Name, @"^\d+$"));

            foreach(Series serie in profileSeries)
            {
                profiles.Add(serie);
            }

            return profiles;
        }

        private int GetSelectedProfileRowIndex()
        {
            foreach(DataGridViewRow row in profilePropertiesTable.Rows)
            {
                if(row.Cells["ProfileNumberCol"].Value.ToString()
                        == (profileChart.Series[SelectedLineId.ToString()].Name))
                {
                    return row.Index;
                }
            }

            return -1;
        }

        private ListViewItem CreateNewItem(string mainText, string subText)
        {
            var newItem = new ListViewItem(mainText);
            newItem.SubItems.Add(subText);

            return newItem;
        }

        private double GetObserverPointFullHeight(int lineId)
        {
            var segments = _controller.GetLineSegments(lineId);

            if(segments != null)
            {
                return ProfilesProperties.First(property => property.LineId == lineId).ObserverHeight + segments.Max(segment => segment.ProfileSurfacePoints.First().Z);
            }

            return ProfilesProperties.First(property => property.LineId == lineId).ObserverHeight + profileChart.Series[lineId.ToString()].Points[0].YValues[0];
        }


        private List<Color> GetAllColors(bool visible)
        {
            var colors = new List<Color>();

            foreach(var serie in GetProfiles())
            {
                Color color;

                if(visible)
                {
                    color = serie.Color;
                }
                else
                {
                    color = serie.BackSecondaryColor;
                }

                colors.Add(color);
            }

            return colors;
        }

        #endregion

        #region Events

        private void CheckBoxHeader_CheckedChanged(object sender, EventArgs e)
        {
            foreach(DataGridViewRow row in profilePropertiesTable.Rows)
            {
                if(row.Index == profilePropertiesTable.SelectedRows[0].Index)
                {
                    row.Cells["IsVisibleCol"].Value = true;
                }
                else
                {
                    row.Cells["IsVisibleCol"].Value = _checkBoxHeader.Checked;
                }
            }
        }

        private void Profile_MouseDown(object sender, MouseEventArgs e)
        {
            var selectedPoint = profileChart.HitTest(e.X, e.Y);

            if(selectedPoint.ChartElementType == ChartElementType.DataPoint &&
                Regex.IsMatch(selectedPoint.Series.Name, @"^\d+$"))
            {
                //TODO:: Create a list outside the graph
                SelectProfile(selectedPoint.Series.Name);
            }
        }

        private void ProfileChart_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var selectedPoint = profileChart.HitTest(e.X, e.Y);

            if(selectedPoint.ChartElementType == ChartElementType.DataPoint &&
                Regex.IsMatch(selectedPoint.Series.Name, @"^\d+$"))
            {
                if(selectedPoint.Series.Tag is ProfileSurface profileData)
                {
                    var point = profileData.ProfileSurfacePoints[selectedPoint.PointIndex];
                    _controller.InvokeOnProfileGraphClicked(point.X, point.Y);
                }
            }
        }

        private void SurfaceProfileChart_Resize(object sender, EventArgs e)
        {
            SetControlSize();
        }

        private void ObserverHeightTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter && SelectedLineId != -1)
            {
                if(ChangeObseverPointHeight())
                {
                    e.SuppressKeyPress = true;
                }
            }
        }

        private void VisibleLineColorButton_Click(object sender, EventArgs e)
        {
            lineColorDialog.Color = profileChart.Series[SelectedLineId.ToString()].Color;

            if(lineColorDialog.ShowDialog() == DialogResult.OK)
            {
                profileChart.Series[SelectedLineId.ToString()].Color = lineColorDialog.Color;

                UpdateProfileWithNewColor();
                _controller.InvokeGraphRedrawn(SelectedLineId, lineColorDialog.Color);

                visibleLineColorButton.BackColor = lineColorDialog.Color;
            }
        }

        private void InvisibleLineColorButton_Click(object sender, EventArgs e)
        {
            lineColorDialog.Color = profileChart.Series[SelectedLineId.ToString()].BackSecondaryColor;

            if(lineColorDialog.ShowDialog() == DialogResult.OK)
            {
                profileChart.Series[SelectedLineId.ToString()].BackSecondaryColor = lineColorDialog.Color;
                invisibleLineColorButton.BackColor = lineColorDialog.Color;

                ProfileSurface[] profileSurfaces = GetSurfacesFromChart();

                UpdateProfile(SelectedLineId.ToString());
                _controller.AddInvisibleZone(ProfilesProperties.First(property => property.LineId == SelectedLineId).ObserverHeight, profileSurfaces.First(surface => surface.LineId == SelectedLineId),
                                                profileChart.Series[SelectedLineId.ToString()].Color, profileChart.Series[SelectedLineId.ToString()].BackSecondaryColor);

                UpdateProfileExtremePoints(SelectedLineId);
            }
        }

        private void GraphToolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            Axis axis = profileChart.ChartAreas["Default"].AxisX;

            switch(e.Button.Name)
            {
                case "displayProfileSignatureGraphToolBarBtn":

                    if(_isCommentDisplay == true)
                    {
                        foreach(var serie in GetProfiles())
                        {
                            serie.Points[(int)(serie.Points.Count / 2)].Label = String.Empty;
                        }
                    }
                    else
                    {
                        foreach(var serie in GetProfiles())
                        {
                            serie.Points[(int)(serie.Points.Count / 2)].Label = $"{serie.Name}";
                        }
                    }

                    _isCommentDisplay = !_isCommentDisplay;

                    break;

                case "deleteSelectedProfileGraphToolBarBtn":

                    if(SelectedLineId != -1)
                    {
                        DialogResult deleteProfileResult =
                                        MessageBox.Show(LocalizationConstants.RemovingProfileMessage, LocalizationConstants.MessageBoxTitle,
                                                                                        MessageBoxButtons.OKCancel);

                        if(deleteProfileResult == DialogResult.OK)
                        {
                            if(GetProfiles().Count > 1)
                            {
                                DeleteSelectedProfile();
                            }
                            else
                            {
                                _controller.InvokeProfileRemoved(Convert.ToInt32(profileChart.Series.First().Name));
                                profileChart.Series.Clear();
                                _controller.RemoveCurrentTab();
                            }
                        }
                    }

                    break;

                case "addProfileGraphToolBarButton":

                    _controller.AddProfileToExistedGraph();

                    break;

                case "showAllProfilesGraphToolBarBtn":

                    axis.ScaleView.Position = 0;
                    axis.ScaleView.Size = axis.Maximum;

                    SetAxisInterval(axis);

                    break;

                case "observerHeightIgnoreGraphToolBarBtn":

                    _isObserverHeightIgnore = !_isObserverHeightIgnore;
                    SetAxisSizes();
                    SetAxisInterval(profileChart.ChartAreas["Default"].AxisY);
                    UpdateIntersectionsLine();

                    break;

                case "panToSelectedProfileGraphToolBarBtn":

                    _controller.PanToSelectedProfile((int)profileNameLabel.Tag, SelectedLineId);

                    break;

                case "panGraphToolBarBtn":

                    _controller.PanToSelectedProfilesSet((int)profileNameLabel.Tag);

                    break;

                case "zoomInGraphToolBarBtn":

                    if(double.IsNaN(axis.ScaleView.Size))
                    {
                        axis.ScaleView.Size = (axis.Maximum - axis.Minimum) / 2;
                    }
                    else
                    {
                        axis.ScaleView.Size /= 2;
                    }

                    if(axis.Interval > 2)
                    {
                        axis.Interval /= 2;
                        axis.Tag = axis.ScaleView.Size;
                    }

                    break;

                case "zoomOutGraphToolBarBtn":

                    if(axis.ScaleView.Size == axis.Maximum - axis.Minimum)
                    {
                        axis.ScaleView.Size = axis.Maximum;
                        axis.ScaleView.Position = 0;
                        break;
                    }

                    if(double.IsNaN(axis.ScaleView.Size))
                    {
                        axis.ScaleView.Size = axis.Maximum;
                    }
                    else
                    {
                        axis.ScaleView.Size *= 2;
                    }

                    if(axis.ScaleView.Size >= (double)axis.Tag)
                    {
                        axis.Interval *= 2;
                    }

                    break;

                case "addPageGraphToolBarBtn":

                    _controller.AddEmptyGraph();

                    break;

                case "deletePageGraphToolBarBtn":

                    DialogResult deleteTabResult =
                                    MessageBox.Show(LocalizationConstants.RemovingTabMessage, LocalizationConstants.MessageBoxTitle, MessageBoxButtons.OKCancel);

                    if(deleteTabResult == DialogResult.OK)
                    {
                        _controller.RemoveCurrentTab();
                    }

                    break;

                case "saveGraphToolBarBtn":

                    SaveGraph();

                    break;

                case "updateIntersectionsLinesGraphToolBarBtn":
                    if(SelectedLineId != -1)
                    {
                        GetIntersection(SelectedLineId);
                        _controller.DrawIntersectionLines(Convert.ToInt32(profileChart.Series[SelectedLineId.ToString()].Name));
                    }

                    break;
            }
        }

        private void ProfilePropertiesTable_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if(profilePropertiesTable.Columns[e.ColumnIndex].Name == "IsVisibleCol" && profilePropertiesTable.Rows.Count > 0)
            {
                var id = profilePropertiesTable.Rows[e.RowIndex].Cells["ProfileNumberCol"].Value.ToString();
                profileChart.Series[id].Enabled
                        = (Boolean)profilePropertiesTable.Rows[e.RowIndex].Cells["IsVisibleCol"].Value;

                profileChart.Series[$"ExtremePointsLine{id}"].Enabled =
                                     (Boolean)profilePropertiesTable.Rows[e.RowIndex].Cells["IsVisibleCol"].Value;
            }
        }

        private void ProfilePropertiesTable_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if(profilePropertiesTable.IsCurrentCellDirty)
            {
                profilePropertiesTable.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void ObserverHeightTextBox_Leave(object sender, EventArgs e)
        {
            if(GetProfiles().Count() > 0)
            {
                ChangeObseverPointHeight();
            }
        }

        private void ProfilePropertiesTable_SelectionChanged(object sender, EventArgs e)
        {
            if(profilePropertiesTable.SelectedRows.Count > 0)
            {
                SelectProfile(profilePropertiesTable.SelectedRows[0].Cells["ProfileNumberCol"].Value.ToString());
            }
        }

        private void ProfileChart_AxisViewChanged(object sender, ViewEventArgs e)
        {
            SetAxisInterval(e.Axis);
        }

        private void ChangeAllObserversHeightsButton_Click(object sender, EventArgs e)
        {
            ChangeObseverPointHeight(true);
        }

        private void ProfilePropertiesTable_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if(e.ColumnIndex == profilePropertiesTable.Columns["AzimuthCol"].Index)
            {
                List<double> azimuths = ProfilesProperties.Select(profile => profile.Azimuth).ToList();
                azimuths.Sort();

                if(profilePropertiesTable.Columns["AzimuthCol"].Tag == "ASC")
                {
                    azimuths.Reverse();
                    profilePropertiesTable.Columns["AzimuthCol"].Tag = "DESC";
                }
                else
                {
                    profilePropertiesTable.Columns["AzimuthCol"].Tag = "ASC";
                }

                var sortedProperties = new List<ProfileProperties>();

                for(int i = 0; i < azimuths.Count; i++)
                {
                    var properties = ProfilesProperties.Where(profile => profile.Azimuth == azimuths[i]).ToList();

                    if(properties.Count() > 1)
                    {
                        foreach(var property in properties)
                        {
                            sortedProperties.Add(property);
                        }

                        i += (properties.Count() - 1);
                    }
                    else
                    {
                        sortedProperties.Add(properties[0]);
                    }
                }

                FillPropertiesTable(sortedProperties);
            }
        }

        private void ProfilePropertiesTable_Resize(object sender, EventArgs e)
        {
            propertiesPanel.Width = profilePropertiesTable.PreferredSize.Width
                                        + propertiesPanel.Padding.Right
                                        + propertiesPanel.Padding.Left;
        }

        private void PropertiesSplitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            profilePropertiesTable.Height = propertiesSplitContainer.Panel1.Height;

            SetPropertiesContainersSize();
        }

        private void CopyStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                StringBuilder text = new StringBuilder();

                foreach(ListViewItem item in profileDetailsListView.SelectedItems)
                {
                    foreach(ListViewItem.ListViewSubItem sub in item.SubItems)
                    {
                        text.Append(sub.Text + "\t");
                    }

                    text.AppendLine();
                }

                Clipboard.SetDataObject(text.ToString());
            }
            catch(System.Runtime.InteropServices.ExternalException)
            {

            }
        }

        #endregion

        internal void SelectProfile(string serieName)
        {
            if(SelectedLineId.ToString() == serieName)
            {
                return;
            }

            ClearIntersectionLines();

            if(SelectedLineId != -1 && GetProfiles().Count > 1)
            {
                profileChart.Series[SelectedLineId.ToString()].BorderWidth -= 2;
                profileChart.Series[SelectedLineId.ToString()].Font =
                                   new Font(profileChart.Series[SelectedLineId.ToString()].Font, FontStyle.Regular);
            }


            SelectedLineId = Convert.ToInt32(serieName);

            var id = (int)profileNameLabel.Tag;
            var profileName = _controller.GetProfileNameForLabel(ref id, SelectedLineId);

            if(profileName != String.Empty)
            {
                profileNameLabel.Text = $"Профиль: {profileName}";
                profileNameLabel.Tag = id;
            }

            observerHeightTextBox.Text = ProfilesProperties.First(property => property.LineId == Convert.ToInt32(serieName))
                                                           .ObserverHeight.ToString();

            profileChart.Series[SelectedLineId.ToString()].BorderWidth += 2;
            profileChart.Series[SelectedLineId.ToString()].Font =
                                new Font(profileChart.Series[SelectedLineId.ToString()].Font, FontStyle.Bold);

            profilePropertiesTable.Rows[GetSelectedProfileRowIndex()].Selected = true;

            ShowDetails();
            ShowColors();

            invisibleLineColorButton.Visible = true;
            visibleLineColorButton.Visible = true;

            _controller.InvokeSelectedProfile(SelectedLineId);
            _controller.DrawIntersectionLines(SelectedLineId);
        }

        internal SurfaceProfileChartController GetController()
        {
            return _controller;
        }

        internal void SetCurrentChart(MilSpaceProfileGraphsController graphsController)
        {
            _controller.SetCurrentChart(graphsController);
        }

        private void ClearSelection()
        {
            profileNameLabel.Text = String.Empty;
            SelectedLineId = -1;

            observerHeightTextBox.Text = String.Empty;

            profileDetailsListView.Items.Clear();

            profilePropertiesTable.ClearSelection();

            invisibleLineColorButton.Visible = false;
            visibleLineColorButton.Visible = false;
        }

        private void ShowColors()
        {
            visibleLineColorButton.BackColor = profileChart.Series[SelectedLineId.ToString()].Color;
            invisibleLineColorButton.BackColor = profileChart.Series[SelectedLineId.ToString()].BackSecondaryColor;
        }

        private void DeleteSelectedProfile()
        {
            if(SelectedLineId == -1)
            {
                return;
            }

            int index = GetSelectedProfileRowIndex();

            _controller.InvokeProfileRemoved(Convert.ToInt32(profileChart.Series[SelectedLineId.ToString()].Name));

            profileChart.Series.Remove(profileChart
                                       .Series[$"ExtremePointsLine{profileChart.Series[SelectedLineId.ToString()].Name}"]);

            profileChart.Series.Remove(profileChart.Series[SelectedLineId.ToString()]);
            ProfilesProperties.Remove(ProfilesProperties.First(property => property.LineId == SelectedLineId));

            ClearSelection();

            profilePropertiesTable.Rows.RemoveAt(index);

            SetAxisView();
        }

        private void SaveGraph()
        {
            var folderBrowserDialog = new FolderBrowserDialog();

            var id = (int)profileNameLabel.Tag;
            var profileName = _controller.GetProfileName(SelectedLineId);

            var fileName = $"{profileName}_{SelectedLineId}_";

            folderBrowserDialog.Description = "Select the folder to save data";

            if(folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                var path = $"{folderBrowserDialog.SelectedPath}\\{profileName}_{SelectedLineId}";
                fileName = $"{path}\\{fileName}";

                var imageFileName = $"{fileName}graph.emf";
                var propertiesFileName = $"{fileName}profile.csv";
                var pointsFileName = $"{fileName}points.csv";


                if(Directory.Exists(path))
                {
                    DialogResult result = MessageBox.Show(String.Format(LocalizationConstants.FolderAlreadyExistMessage, $"{profileName}_{SelectedLineId}"),
                                                          LocalizationConstants.MessageBoxTitle, MessageBoxButtons.OKCancel);

                    if(result != DialogResult.OK)
                    {
                        return;
                    }
                }
                else
                {
                    Directory.CreateDirectory(path);
                }

                if(GetProfiles().Count > 1)
                {
                    profileChart.SaveImage(imageFileName, ChartImageFormat.Emf);
                }
                else
                {
                    profileChart.SaveImage(imageFileName, ChartImageFormat.Emf);
                }

                File.WriteAllText(propertiesFileName, _controller.GetProfilePropertiesText(), Encoding.Default);
                File.WriteAllText(pointsFileName, _controller.GetProfilePointsPropertiesText(SelectedLineId), Encoding.Default);
            }
        }
    }

}

