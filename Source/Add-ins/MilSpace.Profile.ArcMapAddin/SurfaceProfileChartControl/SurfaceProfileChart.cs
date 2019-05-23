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

namespace MilSpace.Profile.SurfaceProfileChartControl
{
    public partial class SurfaceProfileChart : UserControl
    {
        private SurfaceProfileChartController _controller;
        private string _profileName;
        private bool _isCommentDisplay = false;
        private double _defaultChartHeight;
        private int _maxObserverHeightIndex = 0;
        private CheckBox _checkBoxHeader;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<ProfileProperties> ProfilesProperties { get; set; }
        public bool Current { get; set; }
        public int SelectedProfileIndex { get; set; }

        public SurfaceProfileChart(SurfaceProfileChartController controller)
        {
            Current = false;
            SelectedProfileIndex = -1;

            _controller = controller;
            ProfilesProperties = new List<ProfileProperties>();

            InitializeComponent();

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

            var fullHeights = new Dictionary<int, double>();

            for (int i = 0; i < ProfilesProperties.Count; i++)
            {
                fullHeights.Add(ProfilesProperties[i].LineId, GetObserverPointFullHeight(i));
            }

            _controller.AddInvisibleZones(fullHeights, GetAllColors(true), GetAllColors(false));
            _controller.AddExtremePoints();

            GetIntersections();
            FillPropertiesTable();
            AddCheckBoxHeader();

            if (ProfilesProperties.Count == 1)
            {
                SelectProfile("1");
            }
        }

        internal void SetEmptyGraph()
        {
            foreach(ToolBarButton button in graphToolBar.Buttons)
            {
                button.Enabled = false;
            }

            graphToolBar.Buttons["deletePageGraphToolBarBtn"].Enabled = true;
            propertiesPanel.Enabled = false;
        }

        internal void InitializeProfile(ProfileSession profileSession)
        {
            profileChart.Series.Clear();

            foreach (var line in profileSession.ProfileLines)
            {
                var profileSurface =
                    profileSession.ProfileSurfaces.First(surface => surface.LineId == line.Id);

                AddSerie(profileSurface);
            }

            _profileName = profileSession.SessionName;
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


            foreach (var point in profileSurface.ProfileSurfacePoints)
            {
                profileChart.Series.Last().Points.AddXY(point.Distance, point.Z);
            }

            profileChart.Series.Last().Points.First().MarkerStyle = MarkerStyle.Circle;
        }

        internal void AddInvisibleLine(ProfileSurface surface)
        {
            foreach (var point in surface.ProfileSurfacePoints)
            {
                profileChart.Series[surface.LineId.ToString()]
                        .Points
                        .FirstOrDefault(linePoint => (linePoint.XValue.Equals(point.Distance)))
                        .Color = profileChart.Series[surface.LineId.ToString()].BackSecondaryColor;
            }
        }

        internal void SetExtremePoints(List<ProfileSurfacePoint> extremePoints)
        {
            for (var i = 0; i < extremePoints.Count; i++)
            {
                var observerPoint = new ProfileSurfacePoint
                {
                    Distance = 0,
                    Z = GetObserverPointFullHeight(i)
                };

                AddExtremePoint(observerPoint, extremePoints[i], i + 1);
            }
        }

        internal void AddExtremePoint(ProfileSurfacePoint observerPoint, ProfileSurfacePoint observationPoint, int order)
        {
            profileChart.Series.Add(new Series
            {
                ChartType = SeriesChartType.Line,
                Color = Color.DarkGray,
                Name = $"ExtremePointsLine{order}",
                YValuesPerPoint = 1,
                IsVisibleInLegend = false
            });

            profileChart.Series[$"ExtremePointsLine{order}"].BorderDashStyle = ChartDashStyle.Dash;

            profileChart.Series[$"ExtremePointsLine{order}"].Points.AddXY(observerPoint.Distance, observerPoint.Z);
            profileChart.Series[$"ExtremePointsLine{order}"].Points[0].MarkerStyle = MarkerStyle.Circle;

            profileChart.Series[$"ExtremePointsLine{order}"].Points.AddXY(observationPoint.Distance, observationPoint.Z);

            if (profileChart.Series[$"{order}"].Points.Last().Color == profileChart.Series[$"{order}"].BackSecondaryColor)
            {
                profileChart.Series[$"ExtremePointsLine{order}"].Points[1].MarkerColor = Color.Red;
            }
            profileChart.Series[$"ExtremePointsLine{order}"].Points[1].MarkerStyle = MarkerStyle.Circle;
        }

        private void FillPropertiesTable(List<ProfileProperties> properties = null)
        {
            if (properties == null)
            {
                properties = ProfilesProperties;
            }

            profilePropertiesTable.Rows.Clear();

            foreach (var profileProperties in properties)
            {
                var serie = profileChart.Series[profileProperties.LineId.ToString()];
                profilePropertiesTable.Rows.Add(
                            serie.Enabled,
                            profileProperties.LineId,
                            Math.Round(profileProperties.Azimuth, 0),
                            Math.Round(profileProperties.ObserverHeight, 0),
                            Math.Round(profileProperties.PathLength, 0),
                            Math.Round(profileProperties.MinHeight, 0),
                            Math.Round(profileProperties.MaxHeight, 0),
                            Math.Round(profileProperties.MaxHeight - profileProperties.MinHeight, 0),
                            Math.Round(profileProperties.MinAngle, 0),
                            Math.Round(profileProperties.MaxAngle, 0),
                            Math.Round(profileProperties.VisiblePercent, 0));
            }
        }

        private void ShowDetails()
        {
            var surface = (ProfileSurface)profileChart.Series[SelectedProfileIndex].Tag;

            profileDetailsListView.Items.Clear();

            profileDetailsListView.Items.Add(CreateNewItem($"Состояние: ", ""));
            profileDetailsListView.Items.Add(CreateNewItem($"Номер: ", $"{ProfilesProperties[SelectedProfileIndex].LineId}"));
            profileDetailsListView.Items
                                    .Add(CreateNewItem($"Начало/конец: ",
                                        $"{Math.Round(surface.ProfileSurfacePoints[0].X, 5)};"
                                        + $"{Math.Round(surface.ProfileSurfacePoints[0].Y, 5)}"
                                        + $" - {Math.Round(surface.ProfileSurfacePoints.Last().X, 5)};"
                                        + $"{Math.Round(surface.ProfileSurfacePoints.Last().Y, 5)}"));
            profileDetailsListView.Items
                                    .Add(CreateNewItem($"Азимут:",
                                         $"{Math.Round(ProfilesProperties[SelectedProfileIndex].Azimuth, 1)}"));
            profileDetailsListView.Items
                                    .Add(CreateNewItem($"Длина (м):",
                                            $"{Math.Round(ProfilesProperties[SelectedProfileIndex].PathLength, 0)}"));
            profileDetailsListView.Items
                                    .Add(CreateNewItem($"Высота (м):",
                                            $"{Math.Round(ProfilesProperties[SelectedProfileIndex].MinHeight, 0)}"
                                            + $"-{Math.Round(ProfilesProperties[SelectedProfileIndex].MaxHeight, 0)}"));
            profileDetailsListView.Items
                                    .Add(CreateNewItem($"Max угол подъема:",
                                        $"{Math.Round(ProfilesProperties[SelectedProfileIndex].MaxAngle, 1)}"));
            profileDetailsListView.Items
                                    .Add(CreateNewItem($"Max угол спуска: ",
                                        $"{Math.Round(ProfilesProperties[SelectedProfileIndex].MinAngle, 1)}"));
            profileDetailsListView.Items
                                    .Add(CreateNewItem($"Видимые зоны (%): ",
                                            $"{Math.Round(ProfilesProperties[SelectedProfileIndex].VisiblePercent, 2)}"));
        }

        #endregion

        #region Intersections

        internal void AddIntersectionsLine(double startPoint, double lastPoint, Color color, string layerName)
        {
            string name = $"Intersections {layerName}";

            var layerIntersectionLine = profileChart.Series.FindByName(name);
            int i = 1;

            while (profileChart.Series.FindByName(name) != null)
            {
                name = $"Intersections {layerName}{i}";
                i++;
            }
            Series intersectionLine = new Series()
            {
                ChartType = SeriesChartType.StepLine,
                Color = color,
                Name = name,
                BorderWidth = 8,
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
            foreach (var layer in intersectionLines)
            {
                foreach (var line in layer.Lines)
                {
                    AddIntersectionsLine(line.PointFromDistance, line.PointToDistance, layer.LineColor, line.LayerType.ToString());
                }
            }
        }

        internal void ClearIntersectionLines()
        {
            var intersectionLines = new List<Series>();
            intersectionLines = profileChart.Series.Where(serie => serie.Name.Contains("Intersections")).ToList();

            if (intersectionLines == null) { return; }

            foreach (var serie in intersectionLines)
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

            if (intersectionLines == null) { return; }

            foreach (var serie in intersectionLines)
            {
                serie.Enabled = true;
            }
        }

        private void HideIntersectionLines()
        {
            var intersectionLines = new List<Series>();
            intersectionLines = profileChart.Series.Where(serie => serie.Name.Contains("Intersections")).ToList();

            if (intersectionLines == null) { return; }

            foreach (var serie in intersectionLines)
            {
                serie.Enabled = false;
            }
        }

        private void GetIntersections()
        {
           _controller.InvokeGetIntersectionLines();
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
            profilePropertiesTable.Columns["DescentAngleCol"].Width = twoPositionWidth;
            profilePropertiesTable.Columns["RiseAngleCol"].Width = twoPositionWidth;
            profilePropertiesTable.Columns["VisiblePercentCol"].Width = threePositionWidth;

            var tableWidth = 0;

            foreach (DataGridViewColumn column in profilePropertiesTable.Columns)
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
            if (ProfilesProperties.Count == 1)
            {
                var height = ProfilesProperties[0].MaxHeight + ProfilesProperties.Max(property => property.ObserverHeight);

                profileChart.ChartAreas["Default"].AxisY.Maximum = height + height / 10;
                profileChart.ChartAreas["Default"].AxisY.Minimum = ProfilesProperties[0].MinHeight
                    - height / 10;

                profileChart.ChartAreas["Default"].AxisX.Maximum = profileChart.Series[0].Points.Last().XValue
                  + profileChart.Series[0].Points.Last().XValue / 10;
            }
            else
            {
                double maxHeight = ProfilesProperties.Max(profileProperties => profileProperties.MaxHeight)
                                              + ProfilesProperties.Max(property => property.ObserverHeight);

                double minHeight = ProfilesProperties.Min(profileProperties => profileProperties.MinHeight);
                double absHeight = maxHeight - minHeight;

                profileChart.ChartAreas["Default"].AxisY.Maximum = maxHeight + absHeight / 10;
                profileChart.ChartAreas["Default"].AxisY.Minimum = minHeight - absHeight / 10;


                double maxWidth = profileChart.Series.Max(serie => serie.Points.Last().XValue);

                profileChart.ChartAreas["Default"].AxisX.Maximum = maxWidth + maxWidth / 30;
            }

            SetAxisYMinValue();

            _defaultChartHeight = profileChart.ChartAreas["Default"].AxisY.Maximum;
        }

        private void SetAxisYMinValue()
        {
            var size = profileChart.ChartAreas["Default"].AxisY.Maximum
                             - profileChart.ChartAreas["Default"].AxisY.Minimum;

            if (size < 10)
            {
                profileChart.ChartAreas["Default"].AxisY.Minimum -=
               (profileChart.ChartAreas["Default"].AxisY.Minimum % 2);

                return;
            }

            if (profileChart.ChartAreas["Default"].AxisY.Minimum % 10 > 5)
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

            if (axisSize <= 5) { axis.Interval = 1; return; }
            if (axisSize <= 10) { axis.Interval = 2; return; }
            if (axisSize <= 20) { axis.Interval = 5; return; }

            if (axisSize <= 50)
            {
                intervalSize = (int)Math.Truncate(axisSize / (axisSize / 10));
            }
            else
            {
                intervalSize = (int)Math.Truncate(axisSize / 5);
            }

            int intervalKoef;

            if (intervalSize <= 50) { intervalKoef = 10; }
            else if (intervalSize <= 250) { intervalKoef = 50; }
            else if (intervalSize <= 500) { intervalKoef = 100; }
            else if (intervalSize <= 1000) { intervalKoef = 200; }
            else if (intervalSize <= 2500) { intervalKoef = 500; }
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

        private void UpdateExtremePoins(SeriesCollection series)
        {
            for (int i = 0; i < GetProfiles().Count(); i++)
            {
                UpdateProfileExtremePoints(i);
            }
        }

        private void UpdateProfileExtremePoints(int index)
        {
            var serieName = profileChart.Series[index].Name;

            profileChart.Series[$"ExtremePointsLine{serieName}"].Points[0].SetValueY(GetObserverPointFullHeight(index));

            double diff = GetObserverPointFullHeight(index) - profileChart.ChartAreas["Default"].AxisY.Maximum;

            if (diff > 0)
            {
                double newHeight = profileChart.ChartAreas["Default"].AxisY.Maximum + diff;
                profileChart.ChartAreas["Default"].AxisY.Maximum =
                                        newHeight + (newHeight - profileChart.ChartAreas["Default"].AxisY.Minimum) / 10;

                SetAxisInterval(profileChart.ChartAreas["Default"].AxisY);

                _maxObserverHeightIndex = index;
            }
            else
            {
                var fullHeights = new List<double>();
                int newMaxIndex = _maxObserverHeightIndex;

                for (int i = 0; i < ProfilesProperties.Count; i++)
                {
                    fullHeights.Add(GetObserverPointFullHeight(i));

                    if (ProfilesProperties[i].ObserverHeight == ProfilesProperties.Max(property => property.ObserverHeight))
                    {
                        newMaxIndex = i;
                    }
                }

                if (index == _maxObserverHeightIndex)
                {
                    profileChart.ChartAreas["Default"].AxisY.Maximum =
                                       fullHeights.Max() + (fullHeights.Max() - profileChart.ChartAreas["Default"].AxisY.Minimum) / 10;

                    _maxObserverHeightIndex = newMaxIndex;
                }

                if (fullHeights.Max() < _defaultChartHeight)
                {
                    profileChart.ChartAreas["Default"].AxisY.Maximum = _defaultChartHeight;
                }
            }

            SetAxisInterval(profileChart.ChartAreas["Default"].AxisY);

            if (profileChart.Series[$"{serieName}"].Points.Last().Color == profileChart.Series[$"{serieName}"].BackSecondaryColor)
            {
                profileChart.Series[$"ExtremePointsLine{serieName}"].Points[1].MarkerColor = Color.Red;
            }
            else
            {
                profileChart.Series[$"ExtremePointsLine{serieName}"].Points[1].MarkerColor = Color.DarkGray;
            }
        }

        private void UpdateProfiles(SeriesCollection series)
        {
            for (int i = 0; i < GetProfiles().Count(); i++)
            {
                UpdateProfile(i);
            }
        }

        private void UpdateProfile(int index)
        {
            foreach (var point in profileChart.Series[index].Points)
            {
                point.Color = profileChart.Series[index].Color;
            }
        }

        private void UpdateProfileWithNewColor()
        {
            foreach (var point in profileChart.Series[SelectedProfileIndex].Points)
            {
                if (point.Color != profileChart.Series[SelectedProfileIndex].BackSecondaryColor)
                {
                    point.Color = profileChart.Series[SelectedProfileIndex].Color;
                }
            }
        }

        private void UpdateTableWithNewObserverHeigth(DataGridViewRowCollection rows)
        {
            for (int i = 0; i < profilePropertiesTable.Rows.Count; i++)
            {
                UpdateSelectedRowWithNewObserverHeigth(i);
            }
        }

        private void UpdateSelectedRowWithNewObserverHeigth(int index)
        {
            var serie = profileChart.Series[profilePropertiesTable.Rows[index].Cells["ProfileNumberCol"].Value.ToString()];
            var propertyIndex = profileChart.Series.IndexOf(serie);

            profilePropertiesTable.Rows[index].Cells["ObserverHeightCol"].Value
                                         = Math.Round(ProfilesProperties[propertyIndex].ObserverHeight, 0);

            profilePropertiesTable.Rows[index].Cells["VisiblePercentCol"].Value
                                         = Math.Round(ProfilesProperties[propertyIndex].VisiblePercent, 0);
        }

        #endregion

        #region ChangeObserverHeight

        private void ChangeAllProfilesObserverPointHeights(double height)
        {
            for (int i = 0; i < ProfilesProperties.Count; i++)
            {
                ProfilesProperties[i].ObserverHeight = height;
            }

            UpdateProfiles(profileChart.Series);

            var fullHeights = new Dictionary<int, double>();

            for (int i = 0; i < ProfilesProperties.Count; i++)
            {
                fullHeights.Add(ProfilesProperties[i].LineId, GetObserverPointFullHeight(i));
            }

            _controller.AddInvisibleZones(fullHeights, GetAllColors(true), GetAllColors(false), GetSurfacesFromChart());
            UpdateExtremePoins(profileChart.Series);
            UpdateTableWithNewObserverHeigth(profilePropertiesTable.Rows);
        }

        private void ChangeOnlySelectedProfileObserverHeight(double height)
        {
            ProfilesProperties[SelectedProfileIndex].ObserverHeight = height;

            ProfileSurface[] profileSurfaces = GetSurfacesFromChart();

            UpdateProfile(SelectedProfileIndex);
            _controller.AddInvisibleZone(GetObserverPointFullHeight(SelectedProfileIndex),
                                             profileSurfaces[SelectedProfileIndex], profileChart.Series[SelectedProfileIndex].Color,
                                             profileChart.Series[SelectedProfileIndex].BackSecondaryColor);
            UpdateProfileExtremePoints(SelectedProfileIndex);
            UpdateSelectedRowWithNewObserverHeigth(GetSelectedProfileRowIndex());
        }

        private bool ChangeObseverPointHeight(bool changeAll = false)
        {
            if (Regex.IsMatch(observerHeightTextBox.Text, @"^\d+[,|\.]?\d*$"))
            {
                if (SelectedProfileIndex != -1 && !changeAll)
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

        private ProfileSurface[] GetSurfacesFromChart()
        {
            ProfileSurface[] profileSurfaces = new ProfileSurface[profileChart.Series.Count / 2];

            for (int i = 0; i < GetProfiles().Count(); i++)
            {
                profileSurfaces[i] = (ProfileSurface)profileChart.Series[i].Tag;
            }

            return profileSurfaces;
        }

        private List<Series> GetProfiles()
        {
            var profiles = new List<Series>();
            var profileSeries = profileChart.Series.Where(serie => Regex.IsMatch(serie.Name, @"^\d+$"));

            for (int i = 0; i < profileSeries.Count(); i++)
            {
                profiles.Add(profileChart.Series[i]);
            }

            return profiles;
        }

        private int GetSelectedProfileRowIndex()
        {
            foreach (DataGridViewRow row in profilePropertiesTable.Rows)
            {
                if (row.Cells["ProfileNumberCol"].Value.ToString()
                        == (profileChart.Series[SelectedProfileIndex].Name))
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

        private double GetObserverPointFullHeight(int index)
        {
            return ProfilesProperties[index].ObserverHeight + profileChart.Series[index].Points[0].YValues[0];
        }


        private List<Color> GetAllColors(bool visible)
        {
            var colors = new List<Color>();

            foreach (var serie in profileChart.Series)
            {
                Color color;

                if (visible)
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

        private void SurfaceProfileChart_Load(object sender, EventArgs e)
        {
           // SetProfileView();
        }

        private void CheckBoxHeader_CheckedChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in profilePropertiesTable.Rows)
            {
                if (row.Index == profilePropertiesTable.SelectedRows[0].Index)
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

            if (selectedPoint.ChartElementType == ChartElementType.DataPoint &&
                Regex.IsMatch(selectedPoint.Series.Name, @"^\d+$"))
            {
                //TODO:: Create a list outside the graph
                SelectProfile(selectedPoint.Series.Name);
            }
        }

        private void ProfileChart_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var selectedPoint = profileChart.HitTest(e.X, e.Y);

            if (selectedPoint.ChartElementType == ChartElementType.DataPoint &&
                Regex.IsMatch(selectedPoint.Series.Name, @"^\d+$"))
            {
                if (selectedPoint.Series.Tag is ProfileSurface profileData)
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
            if (e.KeyCode == Keys.Enter)
            {
                if (ChangeObseverPointHeight())
                {
                    e.SuppressKeyPress = true;
                }
            }
        }

        private void VisibleLineColorButton_Click(object sender, EventArgs e)
        {
            lineColorDialog.Color = profileChart.Series[SelectedProfileIndex].Color;

            if (lineColorDialog.ShowDialog() == DialogResult.OK)
            {
                profileChart.Series[SelectedProfileIndex].Color = lineColorDialog.Color;

                UpdateProfileWithNewColor();
                _controller.InvokeGraphRedrawn(Convert.ToInt32(profileChart.Series[SelectedProfileIndex].Name), lineColorDialog.Color);

                visibleLineColorButton.BackColor = lineColorDialog.Color;
            }
        }

        private void InvisibleLineColorButton_Click(object sender, EventArgs e)
        {
            lineColorDialog.Color = profileChart.Series[SelectedProfileIndex].BackSecondaryColor;

            if (lineColorDialog.ShowDialog() == DialogResult.OK)
            {
                profileChart.Series[SelectedProfileIndex].BackSecondaryColor = lineColorDialog.Color;
                invisibleLineColorButton.BackColor = lineColorDialog.Color;

                ProfileSurface[] profileSurfaces = GetSurfacesFromChart();

                UpdateProfile(SelectedProfileIndex);
                _controller.AddInvisibleZone(GetObserverPointFullHeight(SelectedProfileIndex), profileSurfaces[SelectedProfileIndex],
                                                profileChart.Series[SelectedProfileIndex].Color,
                                                profileChart.Series[SelectedProfileIndex].BackSecondaryColor);
                UpdateProfileExtremePoints(SelectedProfileIndex);
            }
        }

        private void GraphToolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            Axis axis = profileChart.ChartAreas["Default"].AxisX;

            switch (e.Button.Name)
            {
                case "displayProfileSignatureGraphToolBarBtn":

                    if (_isCommentDisplay == true)
                    {
                        foreach (var serie in GetProfiles())
                        {
                            serie.Points[(int)(serie.Points.Count / 2)].Label = String.Empty;
                        }
                    }
                    else
                    {
                        foreach (var serie in GetProfiles())
                        {
                            serie.Points[(int)(serie.Points.Count / 2)].Label = $"{serie.Name}";
                        }
                    }

                    _isCommentDisplay = !_isCommentDisplay;

                    break;

                case "deleteSelectedProfileGraphToolBarBtn":

                    if (GetProfiles().Count > 1)
                    {
                        DeleteSelectedProfile();
                    }
                    else
                    {
                        _controller.InvokeProfileRemoved(Convert.ToInt32(profileChart.Series.First().Name));
                        profileChart.Series.Clear();
                        _controller.RemoveCurrentTab();
                    }

                    break;

                case "showAllProfilesGraphToolBarBtn":

                    axis.ScaleView.Position = 0;
                    axis.ScaleView.Size = axis.Maximum;

                    SetAxisInterval(axis);

                    break;

                case "zoomInGraphToolBarBtn":

                    if (double.IsNaN(axis.ScaleView.Size))
                    {
                        axis.ScaleView.Size = (axis.Maximum - axis.Minimum) / 2;
                    }
                    else
                    {
                        axis.ScaleView.Size /= 2;
                    }

                    if (axis.Interval > 2)
                    {
                        axis.Interval /= 2;
                        axis.Tag = axis.ScaleView.Size;
                    }

                    break;

                case "zoomOutGraphToolBarBtn":

                    if (axis.ScaleView.Size == axis.Maximum - axis.Minimum)
                    {
                        axis.ScaleView.Size = axis.Maximum;
                        axis.ScaleView.Position = 0;
                        break;
                    }

                    if (double.IsNaN(axis.ScaleView.Size))
                    {
                        axis.ScaleView.Size = axis.Maximum;
                    }
                    else
                    {
                        axis.ScaleView.Size *= 2;
                    }

                    if (axis.ScaleView.Size >= (double)axis.Tag)
                    {
                        axis.Interval *= 2;
                    }

                    break;

                case "addPageGraphToolBarBtn":

                    _controller.AddEmptyGraph();

                    break;

                case "deletePageGraphToolBarBtn":

                    _controller.RemoveCurrentTab();

                    break;

                case "saveGraphToolBarBtn":

                    SaveGraph();

                    break;

                case "updateIntersectionsLinesGraphToolBarBtn":

                    GetIntersections();
                    _controller.DrawIntersectionLines(Convert.ToInt32(profileChart.Series[SelectedProfileIndex].Name));

                    break;
            }
        }

        private void ProfilePropertiesTable_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (profilePropertiesTable.Columns[e.ColumnIndex].Name == "IsVisibleCol")
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
            if (profilePropertiesTable.IsCurrentCellDirty)
            {
                profilePropertiesTable.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void ObserverHeightTextBox_Leave(object sender, EventArgs e)
        {
            if (GetProfiles().Count() > 0)
            {
                ChangeObseverPointHeight();
            }
        }

        private void ProfilePropertiesTable_SelectionChanged(object sender, EventArgs e)
        {
            if (profilePropertiesTable.SelectedRows.Count > 0)
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
            if (e.ColumnIndex == profilePropertiesTable.Columns["AzimuthCol"].Index)
            {
                List<double> azimuths = ProfilesProperties.Select(profile => profile.Azimuth).ToList();
                azimuths.Sort();

                if (profilePropertiesTable.Columns["AzimuthCol"].Tag == "ASC")
                {
                    azimuths.Reverse();
                    profilePropertiesTable.Columns["AzimuthCol"].Tag = "DESC";
                }
                else
                {
                    profilePropertiesTable.Columns["AzimuthCol"].Tag = "ASC";
                }

                var sortedProperties = new List<ProfileProperties>();

                for (int i = 0; i < azimuths.Count; i++)
                {
                    var properties = ProfilesProperties.Where(profile => profile.Azimuth == azimuths[i]).ToList();

                    if (properties.Count() > 1)
                    {
                        foreach (var property in properties)
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

                foreach (ListViewItem item in profileDetailsListView.SelectedItems)
                {
                    foreach (ListViewItem.ListViewSubItem sub in item.SubItems)
                    {
                        text.Append(sub.Text + "\t");
                    }

                    text.AppendLine();
                }

                Clipboard.SetDataObject(text.ToString());
            }
            catch (System.Runtime.InteropServices.ExternalException)
            {

            }
        }

        #endregion

        internal void SelectProfile(string serieName)
        {
            if (SelectedProfileIndex == profileChart.Series.IndexOf(serieName))
            {
                return;
            }

            ClearIntersectionLines();

            if (SelectedProfileIndex != -1 && profileChart.Series.Count > 2)
            {
                profileChart.Series[SelectedProfileIndex].BorderWidth -= 2;
                profileChart.Series[SelectedProfileIndex].Font =
                                   new Font(profileChart.Series[SelectedProfileIndex].Font, FontStyle.Regular);
            }

            profileNameLabel.Text = $"Профиль: {_profileName}";
            SelectedProfileIndex = profileChart.Series.IndexOf(serieName);

            observerHeightTextBox.Text = ProfilesProperties[SelectedProfileIndex].ObserverHeight.ToString();

            profileChart.Series[SelectedProfileIndex].BorderWidth += 2;
            profileChart.Series[SelectedProfileIndex].Font =
                                new Font(profileChart.Series[SelectedProfileIndex].Font, FontStyle.Bold);

            profilePropertiesTable.Rows[GetSelectedProfileRowIndex()].Selected = true;

            ShowDetails();
            ShowColors();

            invisibleLineColorButton.Visible = true;
            visibleLineColorButton.Visible = true;

            _controller.InvokeSelectedProfile(Convert.ToInt32(serieName));
            _controller.DrawIntersectionLines(Convert.ToInt32(serieName));
        }

        private void ClearSelection()
        {
            profileNameLabel.Text = String.Empty;
            SelectedProfileIndex = -1;

            observerHeightTextBox.Text = String.Empty;

            profileDetailsListView.Clear();

            profilePropertiesTable.ClearSelection();

            invisibleLineColorButton.Visible = false;
            visibleLineColorButton.Visible = false;
        }

        private void ShowColors()
        {
            visibleLineColorButton.BackColor = profileChart.Series[SelectedProfileIndex].Color;
            invisibleLineColorButton.BackColor = profileChart.Series[SelectedProfileIndex].BackSecondaryColor;
        }

        private void DeleteSelectedProfile()
        {
            if (SelectedProfileIndex == -1)
            {
                return;
            }

            int index = GetSelectedProfileRowIndex();

            _controller.InvokeProfileRemoved(Convert.ToInt32(profileChart.Series[SelectedProfileIndex].Name));

            profileChart.Series.Remove(profileChart
                                       .Series[$"ExtremePointsLine{profileChart.Series[SelectedProfileIndex].Name}"]);

            profileChart.Series.RemoveAt(SelectedProfileIndex);
            ProfilesProperties.RemoveAt(SelectedProfileIndex);

            SelectedProfileIndex = -1;

            profilePropertiesTable.Rows.RemoveAt(index);

            var fullHeights = new List<double>();

            for (int i = 0; i < ProfilesProperties.Count; i++)
            {
                fullHeights.Add(GetObserverPointFullHeight(i));
            }

            if (fullHeights.Max() > _defaultChartHeight)
            {
                profileChart.ChartAreas["Default"].AxisY.Maximum =
                                        fullHeights.Max()
                                        + (fullHeights.Max() - profileChart.ChartAreas["Default"].AxisY.Minimum) / 10;
                SetAxisYMinValue();
            }
            else
            {
                profileChart.ChartAreas["Default"].AxisY.Maximum = _defaultChartHeight;
            }

            SetAxisInterval(profileChart.ChartAreas["Default"].AxisY);
        }

        private void SaveGraph()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "|*.png";
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                profileChart.SaveImage(saveFileDialog.FileName, ChartImageFormat.Png);
            }
        }
    }

}
       
    