namespace SurfaceProfileChart.SurfaceProfileChartControl
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
            ((System.ComponentModel.ISupportInitialize)(this.profileChart)).BeginInit();
            this.SuspendLayout();
            // 
            // profileChart
            // 
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
            // SurfaceProfileChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.profileChart);
            this.Name = "SurfaceProfileChart";
            this.Size = new System.Drawing.Size(684, 243);
            this.Load += new System.EventHandler(this.SurfaceProfileChart_Load);
            ((System.ComponentModel.ISupportInitialize)(this.profileChart)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart profileChart;
    }
}
