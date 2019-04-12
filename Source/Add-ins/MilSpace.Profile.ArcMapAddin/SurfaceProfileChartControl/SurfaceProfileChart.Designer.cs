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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.profileChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.profileProperties = new System.Windows.Forms.ListView();
            this.lineName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pathLength = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.angleMax = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.angleMin = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.heighMax = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.heighMin = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.visibilityPersentage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ((System.ComponentModel.ISupportInitialize)(this.profileChart)).BeginInit();
            this.SuspendLayout();
            // 
            // profileChart
            // 
            this.profileChart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            chartArea3.AxisX.IntervalOffsetType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            chartArea3.AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            chartArea3.AxisX.Minimum = 0D;
            chartArea3.Name = "Default";
            this.profileChart.ChartAreas.Add(chartArea3);
            this.profileChart.Location = new System.Drawing.Point(0, 0);
            this.profileChart.Name = "profileChart";
            series3.ChartArea = "Default";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series3.Name = "Series1";
            series3.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int64;
            series3.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int64;
            this.profileChart.Series.Add(series3);
            this.profileChart.Size = new System.Drawing.Size(430, 281);
            this.profileChart.TabIndex = 0;
            this.profileChart.Text = "chart1";
            this.profileChart.Click += new System.EventHandler(this.profileChart_Click);
            this.profileChart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Profile_MouseDown);
            // 
            // profileProperties
            // 
            this.profileProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.profileProperties.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.lineName,
            this.pathLength,
            this.angleMax,
            this.angleMin,
            this.heighMax,
            this.heighMin,
            this.visibilityPersentage,
            this.columnHeader1});
            this.profileProperties.GridLines = true;
            this.profileProperties.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.profileProperties.HoverSelection = true;
            this.profileProperties.Location = new System.Drawing.Point(436, 3);
            this.profileProperties.Name = "profileProperties";
            this.profileProperties.Size = new System.Drawing.Size(242, 208);
            this.profileProperties.TabIndex = 37;
            this.profileProperties.UseCompatibleStateImageBehavior = false;
            this.profileProperties.View = System.Windows.Forms.View.Details;
            // 
            // lineName
            // 
            this.lineName.Text = "Name";
            // 
            // pathLength
            // 
            this.pathLength.Text = "Длинна";
            // 
            // angleMax
            // 
            this.angleMax.Text = "Угол (max)";
            // 
            // angleMin
            // 
            this.angleMin.Text = "Угол (min)";
            // 
            // heighMax
            // 
            this.heighMax.Text = "Высота (max)";
            // 
            // heighMin
            // 
            this.heighMin.Text = "Высота (min)";
            // 
            // visibilityPersentage
            // 
            this.visibilityPersentage.Text = "Видимость % ";
            // 
            // SurfaceProfileChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.profileProperties);
            this.Controls.Add(this.profileChart);
            this.Name = "SurfaceProfileChart";
            this.Size = new System.Drawing.Size(681, 284);
            this.Load += new System.EventHandler(this.SurfaceProfileChart_Load);
            this.Resize += new System.EventHandler(this.SurfaceProfileChart_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.profileChart)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart profileChart;
        private System.Windows.Forms.ListView profileProperties;
        private System.Windows.Forms.ColumnHeader lineName;
        private System.Windows.Forms.ColumnHeader pathLength;
        private System.Windows.Forms.ColumnHeader angleMax;
        private System.Windows.Forms.ColumnHeader angleMin;
        private System.Windows.Forms.ColumnHeader heighMax;
        private System.Windows.Forms.ColumnHeader heighMin;
        private System.Windows.Forms.ColumnHeader visibilityPersentage;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}
