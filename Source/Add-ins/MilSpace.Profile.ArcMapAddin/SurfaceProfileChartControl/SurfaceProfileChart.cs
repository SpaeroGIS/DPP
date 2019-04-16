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

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<ProfileProperties> ProfilesProperties { get; set; }
        public bool Current { get; set; }
        public int SelectedProfileIndex { get; set; }
        public double ObserverHeight { get; set; }

        public SurfaceProfileChart(SurfaceProfileChartController controller)
        {
            Current = false;
            SelectedProfileIndex = -1;

            _controller = controller;
            ProfilesProperties = new List<ProfileProperties>();

            InitializeComponent();
            profileChart.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        }

        internal void InitializeGraph()
        {
            _controller.LoadSeries();
            _controller.AddInvisibleZones(profileChart.Series[0].Points[0].YValues[0]);
            _controller.AddExtremePoints();
            _controller.SetProfilesProperties();
            FillPropertiesTable();
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

        }

        internal void AddSerie(ProfileSurface profileSurface)
        {
            profileChart.Series.Add(new Series
            {
                ChartType = SeriesChartType.Line,
                Color = Color.ForestGreen,
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
                    .FirstOrDefault(linePoint => (linePoint.XValue == point.Distance)).Color = Color.Red;
            }
        }

        internal void SetExtremePoints(List<ProfileSurfacePoint> extremePoints)
        {
            ObserverHeight = extremePoints[0].Z;

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

            if (profileChart.Series[$"{order}"].Points.Last().Color == Color.Red)
            {
                profileChart.Series[$"ExtremePointsLine{order}"].Points[1].MarkerColor = Color.Red;
            }
            profileChart.Series[$"ExtremePointsLine{order}"].Points[1].MarkerStyle = MarkerStyle.Circle;
        }

        private void UpdateExtremePoins()
        {
            for (int i = 1; i < profileChart.Series.Count / 2 + 1; i++)
            {
                profileChart.Series[$"ExtremePointsLine{i}"].Points[0].SetValueY(ObserverHeight);

                if (profileChart.Series[$"{i}"].Points.Last().Color == Color.Red)
                {
                    profileChart.Series[$"ExtremePointsLine{i}"].Points[1].MarkerColor = Color.Red;
                }
                else
                {
                    profileChart.Series[$"ExtremePointsLine{i}"].Points[1].MarkerColor = Color.DarkGray;
                }
            }
        }

        private void UpdateProfiles()
        {
            for (int i = 1; i < profileChart.Series.Count / 2 + 1; i++)
            {
                foreach (var point in profileChart.Series[$"{i}"].Points)
                {
                    point.Color = Color.ForestGreen;
                }
            }
        }

        private void ChangeObserverPointHeight(double height)
        {
            ObserverHeight = height;

            foreach (DataGridViewRow row in profilePropertiesTable.Rows)
            {
                row.Cells["ObserverPointHeightCol"].Value = height;
            }

            UpdateProfiles();
            _controller.AddInvisibleZones(height, GetSurfacesFromChart());
            UpdateExtremePoins();
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
                profileChart.Series[SelectedProfileIndex].BorderWidth -= 1;

                profilePropertiesTable.Rows[SelectedProfileIndex].DefaultCellStyle.BackColor = Color.White;
            }

            SelectedProfileIndex = profileChart.Series.IndexOf(serieName);
            profileChart.Series[SelectedProfileIndex].BorderWidth += 1;

            profilePropertiesTable.Rows[SelectedProfileIndex].DefaultCellStyle.BackColor =
                    profileChart.Series[SelectedProfileIndex].Color;
        }

        private void FillPropertiesTable()
        {
            profilePropertiesTable.Rows.Clear();

            foreach (var profilesProperties in ProfilesProperties)
            {
                profilePropertiesTable.Rows.Add(profilesProperties.LineId.ToString(),
                            Math.Round(ObserverHeight, 1),
                            "",
                            Math.Round(profilesProperties.PathLength, 0).ToString(),
                            Math.Round(profilesProperties.MinHeight, 0).ToString(),
                            Math.Round(profilesProperties.MaxHeight, 0).ToString(),
                            Math.Round(profilesProperties.MinAngle, 1).ToString(),
                            Math.Round(profilesProperties.MaxAngle, 1).ToString(),
                            "");
            }
        }

        private void Profile_MouseDown(object sender, MouseEventArgs e)
        {
            var selectedPoint = profileChart.HitTest(e.X, e.Y);

            if (selectedPoint.ChartElementType == ChartElementType.DataPoint)
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
            profileChart.Width = Width - profilePropertiesTable.Width - 10;
            profileChart.Height = Height - 10;

        }

        private void profileChart_Click(object sender, EventArgs e)
        {

        }

        private void SurfaceProfileChart_Resize(object sender, EventArgs e)
        {
            SetControlSize();
        }

        private void observerHeightTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if(Regex.IsMatch(observerHeightTextBox.Text, @"^\d+[,|\.]?\d*$"))
                {
                    ChangeObserverPointHeight(Convert.ToDouble(observerHeightTextBox.Text.Replace('.', ',')));
                }
            }
        }

        private void profilePropertiesTable_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectProfile(profileChart.Series[profilePropertiesTable.SelectedCells[0].RowIndex].Name);
        }
    }
}
