using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MilSpace.DataAccess.DataTransfer;
using System.Text.RegularExpressions;

namespace MilSpace.Profile.SurfaceProfileChartControl
{
    public partial class SurfaceProfileChart : UserControl
    {
        private SurfaceProfileChartController _controller;
        private int _profileId;
        private bool _isAllProfilesChangeObserverHeight = false;
        private double _defaultChartHeight;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<ProfileProperties> ProfilesProperties { get; set; }
        public bool Current { get; set; }
        public int SelectedProfileIndex { get; set; }
        public List<double> ObserversHeights { get; set; }

        public SurfaceProfileChart(SurfaceProfileChartController controller)
        {
            Current = false;
            SelectedProfileIndex = -1;

            _controller = controller;
            ProfilesProperties = new List<ProfileProperties>();
            ObserversHeights = new List<double>();

            InitializeComponent();

            SetDetailsView();

            profilePropertiesTable.RowTemplate.Height = 18;
            profileChart.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        }

        internal void InitializeGraph()
        {
            _controller.LoadSeries();
            _controller.SetProfilesProperties();

            foreach (var serie in profileChart.Series)
            {
                ObserversHeights.Add(0);
            }

            var fullHeights = new List<double>();

            for(int i = 0; i < ObserversHeights.Count; i++)
            {
                fullHeights.Add(GetObserverPointFullHeight(i));
            }

            _controller.AddInvisibleZones(fullHeights);
            _controller.AddExtremePoints();

            FillPropertiesTable();

            if (ProfilesProperties.Count == 1)
            {
                SelectProfile("1");
            }
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

            _profileId = profileSession.SessionId;
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
        }

        internal void AddInvisibleLine(ProfileSurface surface)
        {
            foreach (var point in surface.ProfileSurfacePoints)
            {
                profileChart.Series[surface.LineId.ToString()]
                        .Points
                        .FirstOrDefault(linePoint => (linePoint.XValue == point.Distance))
                        .Color = profileChart.Series[surface.LineId.ToString()].BackSecondaryColor;
            }
        }

        internal void SetExtremePoints(List<ProfileSurfacePoint> extremePoints)
        {
            for (var i = 1; i < extremePoints.Count; i++)
            {
                AddExtremePoint(extremePoints[0], extremePoints[i], i);
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

        private void UpdateExtremePoins(SeriesCollection series)
        {
            for (int i = 1; i < series.Count / 2 + 1; i++)
            {
                UpdateProfileExtremePoints(i);
            }
        }

        private void UpdateProfileExtremePoints(int index)
        {
            profileChart.Series[$"ExtremePointsLine{index}"].Points[0].SetValueY(GetObserverPointFullHeight(index-1));

            double diff = GetObserverPointFullHeight(index - 1) - profileChart.ChartAreas["Default"].AxisY.Maximum;

            if (diff > 0)
            {
                double newHeight = profileChart.ChartAreas["Default"].AxisY.Maximum + diff;
                profileChart.ChartAreas["Default"].AxisY.Maximum = 
                                        newHeight + (newHeight - profileChart.ChartAreas["Default"].AxisY.Minimum) /10;
            }
            else 
            {
                var fullHeights = new List<double>();

                for (int i = 0; i < ObserversHeights.Count; i++)
                {
                    fullHeights.Add(GetObserverPointFullHeight(i));
                }

                if (fullHeights.Max() < _defaultChartHeight)
                {
                    profileChart.ChartAreas["Default"].AxisY.Maximum = _defaultChartHeight;
                }
            }

            if (profileChart.Series[$"{index}"].Points.Last().Color == profileChart.Series[$"{index}"].BackSecondaryColor)
            {
                profileChart.Series[$"ExtremePointsLine{index}"].Points[1].MarkerColor = Color.Red;
            }
            else
            {
                profileChart.Series[$"ExtremePointsLine{index}"].Points[1].MarkerColor = Color.DarkGray;
            }
        }

        private void UpdateProfiles(SeriesCollection series)
        {
            for (int i = 1; i < series.Count / 2 + 1; i++)
            {
                UpdateProfile(i);
            }
        }

        private void UpdateProfile(int index)
        {
            foreach (var point in profileChart.Series[$"{index}"].Points)
            {
                point.Color = profileChart.Series[$"{index}"].Color;
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
            profilePropertiesTable.Rows[index].Cells["VisiblePercentCol"].Value = Math.Round(ProfilesProperties[index].VisiblePercent, 2);

            if (SelectedProfileIndex != -1)
            {
                profileDetailsListView.Items[3] = CreateNewItem($"Высота пункта наблюдения: ", 
                                                                    $"{Math.Round(ObserversHeights[index], 0)}м;");
            }
        }

        private void ChangeAllProfilesObserverPointHeights(double height)
        {
            for (int i = 0; i < ObserversHeights.Count; i++)
            {
                ObserversHeights[i] = height;
            }

            UpdateProfiles(profileChart.Series);

            var fullHeights = new List<double>();

            for (int i = 0; i < ObserversHeights.Count; i++)
            {
                fullHeights.Add(GetObserverPointFullHeight(i));
            }

            _controller.AddInvisibleZones(fullHeights, GetSurfacesFromChart());
            UpdateExtremePoins(profileChart.Series);
            UpdateTableWithNewObserverHeigth(profilePropertiesTable.Rows);
        }

        private void ChangeOnlySelectedProfileObserverHeight(double height)
        {
            ObserversHeights[SelectedProfileIndex] = height;

            ProfileSurface[] profileSurfaces = GetSurfacesFromChart();

            UpdateProfile(SelectedProfileIndex + 1);
            _controller.AddInvisibleZone(GetObserverPointFullHeight(SelectedProfileIndex)
                                            , profileSurfaces[SelectedProfileIndex]);
            UpdateProfileExtremePoints(SelectedProfileIndex + 1);
            UpdateSelectedRowWithNewObserverHeigth(SelectedProfileIndex);
        }

        private bool ChangeObseverPointHeight()
        {
            if (Regex.IsMatch(observerHeightTextBox.Text, @"^\d+[,|\.]?\d*$"))
            {
                if (_isAllProfilesChangeObserverHeight || SelectedProfileIndex == -1)
                {
                    ChangeAllProfilesObserverPointHeights(Convert.ToDouble(observerHeightTextBox.Text.Replace('.', ',')));
                    _isAllProfilesChangeObserverHeight = false;
                }
                else
                {
                    ChangeOnlySelectedProfileObserverHeight(Convert.ToDouble(observerHeightTextBox.Text.Replace('.', ',')));
                }

                return true;
            }

            return false;
        }

        private void SaveChartAsImage()
        {
            profileChart.SaveImage("Chart.png", ChartImageFormat.Png);
        }

        private ProfileSurface[] GetSurfacesFromChart()
        {
            ProfileSurface[] profileSurfaces = new ProfileSurface[profileChart.Series.Count / 2];

            for (int i = 0; i < profileChart.Series.Count / 2; i++)
            {
                profileSurfaces[i] = (ProfileSurface)profileChart.Series[i].Tag;
            }

            return profileSurfaces;
        }

        private void SurfaceProfileChart_Load(object sender, EventArgs e)
        {
            SetProfileView();
        }

        private void SetProfileView()
        {
            profileChart.ChartAreas["Default"].AxisX.LabelStyle.Format = "#";
            profileChart.ChartAreas["Default"].AxisY.LabelStyle.Format = "#";

            profileChart.ChartAreas["Default"].Position = new ElementPosition(0, 4, 95, 92);
            profileChart.ChartAreas["Default"].InnerPlotPosition = new ElementPosition(6, 0, 94, 95);

            profileChart.Size = this.Size;

            SetYHeight();
        }

        private void SetDetailsView()
        {
            profileDetailsListView.View = View.Details;

            profileDetailsListView.Columns.Add("Attribute", 150);
            profileDetailsListView.Columns.Add("Value", 150);

            profileDetailsListView.HeaderStyle = ColumnHeaderStyle.None;
        }

        private void SetYHeight()
        {
            if (ProfilesProperties.Count == 1)
            {
                profileChart.ChartAreas["Default"].AxisY.Maximum = ProfilesProperties[0].MaxHeight
                    + ProfilesProperties[0].MaxHeight / 10;
                profileChart.ChartAreas["Default"].AxisY.Minimum = ProfilesProperties[0].MinHeight
                    - ProfilesProperties[0].MaxHeight / 10;
                profileChart.ChartAreas["Default"].AxisX.Maximum = profileChart.Series[0].Points.Last().XValue
                  + profileChart.Series[0].Points.Last().XValue / 10;
            }
            else
            {
                double maxHeight = ProfilesProperties.Max(profileProperties => profileProperties.MaxHeight);
                double minHeight = ProfilesProperties.Min(profileProperties => profileProperties.MinHeight);
                double absHeight = maxHeight - minHeight;

                profileChart.ChartAreas["Default"].AxisY.Maximum = maxHeight + absHeight / 10;
                profileChart.ChartAreas["Default"].AxisY.Minimum = minHeight - absHeight / 10;

                double maxWidth = profileChart.Series.Max(serie => serie.Points.Last().XValue);

                profileChart.ChartAreas["Default"].AxisX.Maximum = maxWidth + maxWidth / 30;
            }

            _defaultChartHeight = profileChart.ChartAreas["Default"].AxisY.Maximum;
        }

        private void SelectProfile(string serieName)
        {
            if (SelectedProfileIndex == profileChart.Series.IndexOf(serieName))
            {
                return;
            }

            if (SelectedProfileIndex != -1 && profileChart.Series.Count > 2)
            {
                profileChart.Series[SelectedProfileIndex].BorderWidth -= 2;

                profilePropertiesTable.Rows[SelectedProfileIndex].DefaultCellStyle.BackColor = Color.White;
            }

            profileNameLabel.Text = $"Профиль: {_profileId}";
            SelectedProfileIndex = profileChart.Series.IndexOf(serieName);

            if (profileChart.Series.Count > 2)
            {
                profileChart.Series[SelectedProfileIndex].BorderWidth += 2;

                profilePropertiesTable.Rows[SelectedProfileIndex].DefaultCellStyle.BackColor =
                        profileChart.Series[SelectedProfileIndex].Color;
            }
           
            ShowDetails();
            ShowColors();

            propertiesToolBar.Buttons[1].Enabled = true;
            propertiesToolBar.Buttons[2].Enabled = true;
            propertiesToolBar.Buttons[3].Enabled = true;
            propertiesToolBar.Buttons[4].Enabled = true;

            invisibleLineColorButton.Enabled = true;
            visibleLineColorButton.Enabled = true;
        }

        private void FillPropertiesTable()
        {
            profilePropertiesTable.Rows.Clear();

            foreach (var profilesProperties in ProfilesProperties)
            {
                profilePropertiesTable.Rows.Add( 
                            true,
                            profilesProperties.LineId.ToString(),
                            Math.Round(profilesProperties.Azimuth, 1).ToString(),
                            Math.Round(profilesProperties.PathLength, 0).ToString(),
                            Math.Round(profilesProperties.MinHeight, 0).ToString(),
                            Math.Round(profilesProperties.MaxHeight, 0).ToString(),
                            Math.Round(profilesProperties.MinAngle, 1).ToString(),
                            Math.Round(profilesProperties.MaxAngle, 1).ToString(),
                            Math.Round(profilesProperties.VisiblePercent, 2).ToString());
            }
        }

        private void ShowColors()
        {
            visibleLineColorButton.BackColor = profileChart.Series[SelectedProfileIndex].Color;
            invisibleLineColorButton.BackColor = profileChart.Series[SelectedProfileIndex].BackSecondaryColor;
        }

        private ListViewItem CreateNewItem(string mainText, string subText)
        {
            var newItem = new ListViewItem(mainText);
            newItem.SubItems.Add(subText);

            return newItem;
        }

        private void ShowDetails()
        {
            var surface = (ProfileSurface)profileChart.Series[SelectedProfileIndex].Tag;

            profileDetailsListView.Items.Clear();

            profileDetailsListView.Items.Add(CreateNewItem($"Состояние: ;", ""));
            profileDetailsListView.Items.Add(CreateNewItem($"Номер: ", $"{SelectedProfileIndex + 1};"));
            profileDetailsListView.Items
                                    .Add(CreateNewItem($"Высота наблюдения: ", 
                                                        $"{Math.Round(ObserversHeights[SelectedProfileIndex], 0)}м;"));
            profileDetailsListView.Items
                                    .Add(CreateNewItem($"Oт: ",
                                        $"X={Math.Round(surface.ProfileSurfacePoints[0].X, 0)};"
                                        + $" Y={Math.Round(surface.ProfileSurfacePoints[0].Y, 0)};"));
            profileDetailsListView.Items
                                    .Add(CreateNewItem($"До:",
                                        $"X={Math.Round(surface.ProfileSurfacePoints.Last().X, 0)};"
                                        + $" Y={Math.Round(surface.ProfileSurfacePoints.Last().X, 0)};"));
            profileDetailsListView.Items
                                    .Add(CreateNewItem($"Азимут:",
                                         $"{Math.Round(ProfilesProperties[SelectedProfileIndex].Azimuth, 1)};"));
            profileDetailsListView.Items
                                    .Add(CreateNewItem($"Длина:",
                                            $"{Math.Round(ProfilesProperties[SelectedProfileIndex].PathLength, 0)}м;"));
            profileDetailsListView.Items
                                    .Add(CreateNewItem($"Высота:",
                                            $"{Math.Round(ProfilesProperties[SelectedProfileIndex].MinHeight, 0)}м"
                                            + $"-{Math.Round(ProfilesProperties[SelectedProfileIndex].MaxHeight, 0)}м;"));
            profileDetailsListView.Items
                                    .Add(CreateNewItem($"Max угол подъема:",
                                        $"{Math.Round(ProfilesProperties[SelectedProfileIndex].MaxAngle, 1)};"));
            profileDetailsListView.Items
                                    .Add(CreateNewItem($"Max угол спуска: ",
                                        $"{Math.Round(ProfilesProperties[SelectedProfileIndex].MinAngle, 1)};"));
            profileDetailsListView.Items
                                    .Add(CreateNewItem($"Видимые зоны: ",
                                            $"{Math.Round(ProfilesProperties[SelectedProfileIndex].VisiblePercent, 2)}%;"));
        }

        private void Profile_MouseDown(object sender, MouseEventArgs e)
        {
            var selectedPoint = profileChart.HitTest(e.X, e.Y);

            if (selectedPoint.ChartElementType == ChartElementType.DataPoint &&
                Regex.IsMatch(selectedPoint.Series.Name, @"^\d+$"))
            {
                //TODO:: Create a list outside the graph
                SelectProfile(selectedPoint.Series.Name);
                if (selectedPoint.Series.Tag is ProfileSurface profileData)
                {
                    var point = profileData.ProfileSurfacePoints[selectedPoint.PointIndex];
                    _controller.InvokeOnProfileGraphClicked(point.X, point.Y);
                }
            }
        }

        internal void SetControlSize()
        {
            profileChart.Width = Width - profilePropertiesTable.Width - 20;
            profileChart.Height = Height - graphToolBar.Height - 20;
        }

        private double GetObserverPointFullHeight(int index)
        {
            return ObserversHeights[index] + profileChart.Series[index].Points[0].YValues[0];
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

        private void ProfilePropertiesTable_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectProfile(profileChart.Series[e.RowIndex].Name);
        }

        private void ChangeProfileObserverHeight()
        {
            _isAllProfilesChangeObserverHeight = true;
            observerHeightTextBox.Focus();
        }

        private void PropertiesToolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            switch (e.Button.Name)
            {
                case "changeAllProfilesObserverHeightToolBarBtn":
                    ChangeProfileObserverHeight();
                    break;
            }
        }

        private void VisibleLineColorButton_Click(object sender, EventArgs e)
        {
            lineColorDialog.Color = profileChart.Series[SelectedProfileIndex].Color;

            if (lineColorDialog.ShowDialog() == DialogResult.OK)
            {
                profileChart.Series[SelectedProfileIndex].Color = lineColorDialog.Color;
                UpdateProfileWithNewColor();
                visibleLineColorButton.BackColor = lineColorDialog.Color;

                if(profileChart.Series.Count > 2)
                {
                    profilePropertiesTable.Rows[SelectedProfileIndex].DefaultCellStyle.BackColor =
                    profileChart.Series[SelectedProfileIndex].Color;
                }
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

                UpdateProfile(SelectedProfileIndex + 1);
                _controller.AddInvisibleZone(GetObserverPointFullHeight(SelectedProfileIndex), profileSurfaces[SelectedProfileIndex]);
                UpdateProfileExtremePoints(SelectedProfileIndex + 1);
            }
        }

        private void GraphToolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            Axis axis = profileChart.ChartAreas["Default"].AxisX;

            switch (e.Button.Name)
            {
                case "zoomInGraphToolBarBtn":

                    if (double.IsNaN(axis.ScaleView.Size))
                    {
                        axis.ScaleView.Size = (axis.Maximum - axis.Minimum)/2;
                    }
                    else
                    {
                        axis.ScaleView.Size /= 2;
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

                    break;
            }
        }

        private void ProfilePropertiesTable_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (profilePropertiesTable.Columns[e.ColumnIndex].Name == "IsVisibleCol")
            {
                profileChart.Series[$"{e.RowIndex + 1}"].Enabled =
                                     (Boolean)profilePropertiesTable.Rows[e.RowIndex].Cells["IsVisibleCol"].Value;

                profileChart.Series[$"ExtremePointsLine{e.RowIndex + 1}"].Enabled =
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
            ChangeObseverPointHeight();
        }
    }
}
