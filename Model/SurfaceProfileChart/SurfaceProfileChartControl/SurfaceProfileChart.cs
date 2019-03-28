using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MilSpace.DataAccess.DataTransfer;

namespace SurfaceProfileChart.SurfaceProfileChartControl
{
    public partial class SurfaceProfileChart : UserControl
    {
        private SurfaceProfileChartController _controller;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<ProfileProperty> ProfilesProperties { get; set; }
        public bool Current { get; set; }
        public int SelectedProfileIndex { get; set; }

        public SurfaceProfileChart()
        {
            Current = false;
            SelectedProfileIndex = -1;

            _controller = new SurfaceProfileChartController(this);
            ProfilesProperties = new List<ProfileProperty>();

            InitializeComponent();

            _controller.LoadSeries();
            _controller.AddInvisibleZones(profileChart.Series[0].Points[0].YValues[0]);
            _controller.SetProfilesProperties();
            AddLegends();
        }

        public void InitializeProfile(ProfileSession profileSession, List<ProfileSurfacePoint> extremePoints)
        {
            profileChart.Series.Clear();

            foreach (var line in profileSession.ProfileLines)
            {
                profileChart.Series.Add(new Series
                {
                    ChartType = SeriesChartType.Line,
                    Color = Color.ForestGreen,
                    Name = line.Id.ToString(),
                    YValuesPerPoint = 1,
                    IsVisibleInLegend = false
                });

                var profileSurface =
                    profileSession.ProfileSurfaces.First(surface => surface.LineId == line.Id);

                foreach (var point in profileSurface.ProfileSurfacePoints)
                {
                    profileChart.Series.Last().Points.AddXY(point.Distance, point.Z);
                }
            }

            SetExtremePoints(extremePoints);
        }

        public void AddInvisibleLine(ProfileSurface surface)
        {
            foreach (var point in surface.ProfileSurfacePoints)
            {
                profileChart.Series[surface.LineId.ToString()].Points
                    .FirstOrDefault(linePoint => (linePoint.XValue == point.Distance)).Color = Color.Red;
            }
        }

        private void SetExtremePoints(List<ProfileSurfacePoint> extremePoints)
        {
           
            for(var i = 1; i < extremePoints.Count; i++)
            {
                profileChart.Series.Add(new Series
                {
                    ChartType = SeriesChartType.Line,
                    Color = Color.Blue,
                    Name = $"ExtremePointsLine{i}",
                    YValuesPerPoint = 1,
                    IsVisibleInLegend = false
                });


                profileChart.Series[$"ExtremePointsLine{i}"].Points.AddXY(extremePoints[0].Distance, extremePoints[0].Z);
                profileChart.Series[$"ExtremePointsLine{i}"].Points[0].MarkerStyle = MarkerStyle.Circle;
                profileChart.Series[$"ExtremePointsLine{i}"].Points.AddXY(extremePoints[i].Distance, extremePoints[i].Z);
                profileChart.Series[$"ExtremePointsLine{i}"].Points[1].MarkerStyle = MarkerStyle.Circle;
            }
        }

        private void ChangeObserverPointHeight(double height)
        {
            var observerPoint = profileChart.Series["ExtremePoints"].Points.First(point => point.XValue == 0);
            observerPoint.SetValueY(height);
        }

        private void SaveChartAsImage()
        {
            profileChart.SaveImage("Chart.png", ChartImageFormat.Png);
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
            var maxPoints = new List<DataPoint>();
            var minPoints = new List<DataPoint>();

            foreach (var profileChartSeries in profileChart.Series)
            {
                maxPoints.Add(profileChartSeries.Points.FindMaxByValue("Y1", 0));
                minPoints.Add(profileChartSeries.Points.FindMinByValue("Y1", 0));
            }

            profileChart.ChartAreas["Default"].AxisY.Maximum =
                maxPoints.Max(point => point.YValues.Max(yValue => yValue));
            profileChart.ChartAreas["Default"].AxisY.Minimum =
                minPoints.Min(point => point.YValues.Min(yValue => yValue));
        }

        private void SelectProfile(HitTestResult selectedPoint)
        {
            if (SelectedProfileIndex != -1)
            {
                profileChart.Series[SelectedProfileIndex].BorderWidth -= 1;
                foreach (var cell in profileChart.Legends["Properties"].CustomItems[SelectedProfileIndex + 1].Cells)
                {
                    cell.BackColor = Color.White;
                }
            }

            SelectedProfileIndex = profileChart.Series.IndexOf(selectedPoint.Series.Name);
            profileChart.Series[SelectedProfileIndex].BorderWidth += 1;
            foreach (var cell in profileChart.Legends["Properties"].CustomItems[SelectedProfileIndex+1].Cells)
            {
                cell.BackColor = Color.ForestGreen;
            }
        }

        private void AddLegends()
        {
            profileChart.Legends.Add("Properties");

            profileChart.Legends["Properties"].HeaderSeparator = LegendSeparatorStyle.Line;
            profileChart.Legends["Properties"].Docking = Docking.Bottom;
            profileChart.Legends["Properties"].LegendStyle = LegendStyle.Table;
            profileChart.Legends["Properties"].TableStyle = LegendTableStyle.Tall;


            LegendItem headerItem = new LegendItem();

            headerItem.Cells.Add(LegendCellType.Text, "Line name", ContentAlignment.MiddleCenter);
            headerItem.Cells.Add(LegendCellType.Text, "Path length", ContentAlignment.MiddleCenter);
            headerItem.Cells.Add(LegendCellType.Text, "Max angle", ContentAlignment.MiddleCenter);
            headerItem.Cells.Add(LegendCellType.Text, "Min angle", ContentAlignment.MiddleCenter);
            headerItem.Cells.Add(LegendCellType.Text, "Max height", ContentAlignment.MiddleCenter);
            headerItem.Cells.Add(LegendCellType.Text, "Min height", ContentAlignment.MiddleCenter);

            profileChart.Legends["Properties"].CustomItems.Add(headerItem);

            foreach (var profilesProperties in ProfilesProperties)
            {
                LegendItem newItem = new LegendItem();

                newItem.Cells.Add(LegendCellType.Text, profilesProperties.LineId.ToString(), ContentAlignment.MiddleCenter);
                newItem.Cells.Add(LegendCellType.Text, Math.Round(profilesProperties.PathLength, 0).ToString(), ContentAlignment.MiddleCenter);
                newItem.Cells.Add(LegendCellType.Text, Math.Round(profilesProperties.MaxAngle, 1).ToString(), ContentAlignment.MiddleCenter);
                newItem.Cells.Add(LegendCellType.Text, Math.Round(profilesProperties.MinAngle, 1).ToString(), ContentAlignment.MiddleCenter);
                newItem.Cells.Add(LegendCellType.Text, Math.Round(profilesProperties.MaxHeight, 0).ToString(), ContentAlignment.MiddleCenter);
                newItem.Cells.Add(LegendCellType.Text, Math.Round(profilesProperties.MinHeight, 0).ToString(), ContentAlignment.MiddleCenter);

                    profileChart.Legends["Properties"].CustomItems.Add(newItem);
            }
        }

        private void Profile_MouseDown(object sender, MouseEventArgs e)
        {
            var selectedPoint = profileChart.HitTest(e.X, e.Y);

            if (selectedPoint.ChartElementType == ChartElementType.DataPoint)
            {
                SelectProfile(selectedPoint);
                //todo fire event 
            }

        }

        private void profileChart_Click(object sender, EventArgs e)
        {

        }
    }
}
