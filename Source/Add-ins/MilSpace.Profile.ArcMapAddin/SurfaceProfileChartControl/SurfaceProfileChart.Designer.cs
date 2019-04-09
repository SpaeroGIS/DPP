namespace MilSpace.Profile.SurfaceProfileChartControl
{
    partial class SurfaceProfileChart
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.profileChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.listView1 = new System.Windows.Forms.ListView();
            this.LineName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PathLengt = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.AngelMax = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.AngelMin = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ViewPoint = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.HeightMax = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.HeightMin = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Visibility = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ((System.ComponentModel.ISupportInitialize)(this.profileChart)).BeginInit();
            this.SuspendLayout();
            // 
            // profileChart
            // 
            this.profileChart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            chartArea1.AxisX.IntervalOffsetType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            chartArea1.AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            chartArea1.AxisX.Minimum = 0D;
            chartArea1.Name = "Default";
            this.profileChart.ChartAreas.Add(chartArea1);
            this.profileChart.Location = new System.Drawing.Point(0, 0);
            this.profileChart.Name = "profileChart";
            series1.ChartArea = "Default";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.Name = "Series1";
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int64;
            series1.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int64;
            this.profileChart.Series.Add(series1);
            this.profileChart.Size = new System.Drawing.Size(681, 240);
            this.profileChart.TabIndex = 0;
            this.profileChart.Text = "chart1";
            this.profileChart.Click += new System.EventHandler(this.profileChart_Click);
            this.profileChart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Profile_MouseDown);
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.LineName,
            this.ViewPoint,
            this.PathLengt,
            this.AngelMax,
            this.AngelMin,
            this.HeightMax,
            this.HeightMin,
            this.Visibility});
            this.listView1.Location = new System.Drawing.Point(700, 0);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(343, 152);
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // LineName
            // 
            this.LineName.Text = "Name";
            this.LineName.Width = 63;
            // 
            // PathLengt
            // 
            this.PathLengt.DisplayIndex = 1;
            this.PathLengt.Text = "Path length";
            this.PathLengt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.PathLengt.Width = 55;
            // 
            // AngelMax
            // 
            this.AngelMax.DisplayIndex = 2;
            this.AngelMax.Text = "Angel (max)";
            // 
            // AngelMin
            // 
            this.AngelMin.DisplayIndex = 3;
            this.AngelMin.Text = "Angel (min)";
            // 
            // ViewPoint
            // 
            this.ViewPoint.DisplayIndex = 4;
            this.ViewPoint.Text = "Point of View";
            // 
            // HeightMax
            // 
            this.HeightMax.Text = "Height (max)";
            // 
            // HeightMin
            // 
            this.HeightMin.Text = "Height (min)";
            // 
            // Visibility
            // 
            this.Visibility.Text = "Visibility (%)";
            // 
            // SurfaceProfileChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.profileChart);
            this.Name = "SurfaceProfileChart";
            this.Size = new System.Drawing.Size(1043, 243);
            this.Load += new System.EventHandler(this.SurfaceProfileChart_Load);
            ((System.ComponentModel.ISupportInitialize)(this.profileChart)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart profileChart;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader LineName;
        private System.Windows.Forms.ColumnHeader PathLengt;
        private System.Windows.Forms.ColumnHeader AngelMax;
        private System.Windows.Forms.ColumnHeader AngelMin;
        private System.Windows.Forms.ColumnHeader ViewPoint;
        private System.Windows.Forms.ColumnHeader HeightMax;
        private System.Windows.Forms.ColumnHeader HeightMin;
        private System.Windows.Forms.ColumnHeader Visibility;
    }
}
