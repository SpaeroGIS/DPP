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
        private bool _isOnlySelectedProfileChangeObserverHeight = false;

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
            profileChart.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        }

        internal void InitializeGraph()
        {
            _controller.LoadSeries();
            _controller.SetProfilesProperties();

            foreach (var serie in profileChart.Series)
            {
                ObserversHeights.Add(serie.Points[0].YValues[0]);
            }

            _controller.AddInvisibleZones(ObserversHeights);
            _controller.AddExtremePoints();

            FillPropertiesTable();

            profileChart.ChartAreas["Default"].Position = new ElementPosition(0, 5, 100, 95);
            profileChart.ChartAreas["Default"].InnerPlotPosition = new ElementPosition(6, 0, 94, 90);
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
                profileChart.Series[surface.LineId.ToString()].Points
                    .FirstOrDefault(linePoint => (linePoint.XValue == point.Distance)).Color = profileChart.Series[surface.LineId.ToString()].BackSecondaryColor;
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
            profileChart.Series[$"ExtremePointsLine{index}"].Points[0].SetValueY(ObserversHeights[index - 1]);

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
                profileDetailsListBox.Items[3] = $"Высота пункта наблюдения: {Math.Round(ObserversHeights[index], 0)}м;";
            }
        }

        private void ChangeObserverPointHeight(double height)
        {
            for (int i = 0; i < ObserversHeights.Count; i++)
            {
                ObserversHeights[i] = height;
            }

            UpdateProfiles(profileChart.Series);
            _controller.AddInvisibleZones(ObserversHeights, GetSurfacesFromChart());
            UpdateExtremePoins(profileChart.Series);
            UpdateTableWithNewObserverHeigth(profilePropertiesTable.Rows);
        }

        private void ChangeOnlySelectedProfileObserverHeight(double height)
        {
            ObserversHeights[SelectedProfileIndex] = height;

            ProfileSurface[] profileSurfaces = GetSurfacesFromChart();

            UpdateProfile(SelectedProfileIndex + 1);
            _controller.AddInvisibleZone(height, profileSurfaces[SelectedProfileIndex]);
            UpdateProfileExtremePoints(SelectedProfileIndex + 1);
            UpdateSelectedRowWithNewObserverHeigth(SelectedProfileIndex);
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
            profileChart.ChartAreas["Default"].CursorX.IsUserEnabled = true;
            profileChart.ChartAreas["Default"].CursorX.IsUserSelectionEnabled = true;
            profileChart.ChartAreas["Default"].AxisX.ScaleView.Zoomable = true;
            profileChart.ChartAreas["Default"].CursorY.IsUserEnabled = true;
            profileChart.ChartAreas["Default"].CursorY.IsUserSelectionEnabled = true;
            profileChart.ChartAreas["Default"].AxisY.ScaleView.Zoomable = true;
            profileChart.ChartAreas["Default"].AxisX.LabelStyle.Format = "#";
            profileChart.ChartAreas["Default"].AxisY.LabelStyle.Format = "#";

            profileChart.Size = this.Size;

            SetYHeight();
        }

        private void SetYHeight()
        {
            if (ProfilesProperties.Count == 1)
            {
                profileChart.ChartAreas["Default"].AxisY.Maximum = ProfilesProperties[0].MaxHeight
                    + ProfilesProperties[0].MaxHeight / 10;
                profileChart.ChartAreas["Default"].AxisY.Minimum = ProfilesProperties[0].MinHeight
                    - ProfilesProperties[0].MaxHeight / 10;
            }
            else
            {
                double maxHeight = ProfilesProperties.Max(profileProperties => profileProperties.MaxHeight);
                double minHeight = ProfilesProperties.Min(profileProperties => profileProperties.MinHeight);
                double absHeight = maxHeight - minHeight;

                profileChart.ChartAreas["Default"].AxisY.Maximum = maxHeight + absHeight / 10;
                profileChart.ChartAreas["Default"].AxisY.Minimum = minHeight - absHeight / 10;
            }
        }

        private void SelectProfile(string serieName)
        {
            if (SelectedProfileIndex != -1)
            {
                profileChart.Series[SelectedProfileIndex].BorderWidth -= 2;

                profilePropertiesTable.Rows[SelectedProfileIndex].DefaultCellStyle.BackColor = Color.White;
            }

            profileNameLabel.Text = $"Профиль: {_profileId}";

            SelectedProfileIndex = profileChart.Series.IndexOf(serieName);
            profileChart.Series[SelectedProfileIndex].BorderWidth += 2;

            profilePropertiesTable.Rows[SelectedProfileIndex].DefaultCellStyle.BackColor =
                    profileChart.Series[SelectedProfileIndex].Color;

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
                profilePropertiesTable.Rows.Add(profilesProperties.LineId.ToString(),
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

        private void ShowDetails()
        {
            profileDetailsListBox.Items.Clear();

            profileDetailsListBox.Items.Add($"Состояние: ;");
            profileDetailsListBox.Items.Add($"Номер в списке: {SelectedProfileIndex + 1};");
            profileDetailsListBox.Items.Add($"Высота пункта наблюдения: {Math.Round(ObserversHeights[SelectedProfileIndex], 0)}м;");
            profileDetailsListBox.Items.Add($"Первая точка: X={Math.Round(profileChart.Series[SelectedProfileIndex].Points[0].XValue, 0)};" +
                $" Y={Math.Round(profileChart.Series[SelectedProfileIndex].Points[0].YValues[0], 0)};");
            profileDetailsListBox.Items.Add($"Последняя точка: X={Math.Round(profileChart.Series[SelectedProfileIndex].Points.Last().XValue, 0)};" +
                $" Y={Math.Round(profileChart.Series[SelectedProfileIndex].Points.Last().YValues[0], 0)};");
            profileDetailsListBox.Items.Add($"Азимут: {Math.Round(ProfilesProperties[SelectedProfileIndex].Azimuth, 1)};");
            profileDetailsListBox.Items.Add($"Длина: {Math.Round(ProfilesProperties[SelectedProfileIndex].PathLength, 0)}м;");
            profileDetailsListBox.Items.Add($"Высота: {Math.Round(ProfilesProperties[SelectedProfileIndex].MinHeight, 0)}м" +
                $"-{Math.Round(ProfilesProperties[SelectedProfileIndex].MaxHeight, 0)}м;");
            profileDetailsListBox.Items.Add($"Максимальный угол подъема(градусы): {Math.Round(ProfilesProperties[SelectedProfileIndex].MaxAngle, 1)};");
            profileDetailsListBox.Items.Add($"Максимальный угол спуска(градусы): {Math.Round(ProfilesProperties[SelectedProfileIndex].MinAngle, 1)};");
            profileDetailsListBox.Items.Add($"Процент видимых участков: {Math.Round(ProfilesProperties[SelectedProfileIndex].VisiblePercent, 2)}%;");
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

        private void SurfaceProfileChart_Resize(object sender, EventArgs e)
        {
            SetControlSize();
        }

        private void ObserverHeightTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (Regex.IsMatch(observerHeightTextBox.Text, @"^\d+[,|\.]?\d*$"))
                {
                    if (_isOnlySelectedProfileChangeObserverHeight)
                    {
                        ChangeOnlySelectedProfileObserverHeight(Convert.ToDouble(observerHeightTextBox.Text.Replace('.', ',')));
                        _isOnlySelectedProfileChangeObserverHeight = false;
                    }
                    else
                    {
                        ChangeObserverPointHeight(Convert.ToDouble(observerHeightTextBox.Text.Replace('.', ',')));
                    }
                }
            }
        }

        private void ProfilePropertiesTable_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectProfile(profileChart.Series[profilePropertiesTable.SelectedCells[0].RowIndex].Name);
        }

        private void ChangeProfileObserverHeight()
        {
            _isOnlySelectedProfileChangeObserverHeight = true;
            observerHeightTextBox.Focus();
        }

        private void PropertiesToolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            switch (e.Button.Name)
            {
                case "changeOnlySelectedProfileObserverHeightToolBarBtn":
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
                profilePropertiesTable.Rows[SelectedProfileIndex].DefaultCellStyle.BackColor =
                    profileChart.Series[SelectedProfileIndex].Color;
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
                _controller.AddInvisibleZone(ObserversHeights[SelectedProfileIndex], profileSurfaces[SelectedProfileIndex]);
                UpdateProfileExtremePoints(SelectedProfileIndex + 1);
            }
        }

        private void GraphToolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {

        }
    }
}
