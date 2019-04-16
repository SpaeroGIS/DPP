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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.profileChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.observerHeightLabel = new System.Windows.Forms.Label();
            this.observerHeightTextBox = new System.Windows.Forms.TextBox();
            this.profilePropertiesTable = new System.Windows.Forms.DataGridView();
            this.ProfileNumberCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ObserverPointHeightCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BearingAzimuthCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProfileLengthCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MinHeightCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MaxHeightCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MinAngleCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MaxAngleCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VisiblePercentCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.profileChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.profilePropertiesTable)).BeginInit();
            this.SuspendLayout();
            // 
            // profileChart
            // 
            this.profileChart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            chartArea2.AxisX.IntervalOffsetType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            chartArea2.AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            chartArea2.AxisX.Minimum = 0D;
            chartArea2.Name = "Default";
            this.profileChart.ChartAreas.Add(chartArea2);
            this.profileChart.Location = new System.Drawing.Point(0, 0);
            this.profileChart.Name = "profileChart";
            series2.ChartArea = "Default";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series2.Name = "Series1";
            series2.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int64;
            series2.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int64;
            this.profileChart.Series.Add(series2);
            this.profileChart.Size = new System.Drawing.Size(430, 260);
            this.profileChart.TabIndex = 0;
            this.profileChart.Text = "chart1";
            this.profileChart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Profile_MouseDown);
            // 
            // observerHeightLabel
            // 
            this.observerHeightLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.observerHeightLabel.AutoSize = true;
            this.observerHeightLabel.Location = new System.Drawing.Point(494, 11);
            this.observerHeightLabel.Name = "observerHeightLabel";
            this.observerHeightLabel.Size = new System.Drawing.Size(152, 13);
            this.observerHeightLabel.TabIndex = 38;
            this.observerHeightLabel.Text = "Высота точки наблюденя (м)";
            // 
            // observerHeightTextBox
            // 
            this.observerHeightTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.observerHeightTextBox.Location = new System.Drawing.Point(652, 8);
            this.observerHeightTextBox.Name = "observerHeightTextBox";
            this.observerHeightTextBox.Size = new System.Drawing.Size(59, 20);
            this.observerHeightTextBox.TabIndex = 39;
            this.observerHeightTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ObserverHeightTextBox_KeyDown);
            // 
            // profilePropertiesTable
            // 
            this.profilePropertiesTable.AllowUserToAddRows = false;
            this.profilePropertiesTable.AllowUserToDeleteRows = false;
            this.profilePropertiesTable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.profilePropertiesTable.BackgroundColor = System.Drawing.SystemColors.Control;
            this.profilePropertiesTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.profilePropertiesTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.profilePropertiesTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ProfileNumberCol,
            this.ObserverPointHeightCol,
            this.BearingAzimuthCol,
            this.ProfileLengthCol,
            this.MinHeightCol,
            this.MaxHeightCol,
            this.MinAngleCol,
            this.MaxAngleCol,
            this.VisiblePercentCol});
            this.profilePropertiesTable.Location = new System.Drawing.Point(493, 34);
            this.profilePropertiesTable.MultiSelect = false;
            this.profilePropertiesTable.Name = "profilePropertiesTable";
            this.profilePropertiesTable.ReadOnly = true;
            this.profilePropertiesTable.RowHeadersVisible = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            this.profilePropertiesTable.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.profilePropertiesTable.Size = new System.Drawing.Size(352, 206);
            this.profilePropertiesTable.TabIndex = 40;
            this.profilePropertiesTable.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.ProfilePropertiesTable_CellClick);
            // 
            // ProfileNumberCol
            // 
            this.ProfileNumberCol.HeaderText = "N";
            this.ProfileNumberCol.Name = "ProfileNumberCol";
            this.ProfileNumberCol.ReadOnly = true;
            this.ProfileNumberCol.ToolTipText = "Номер профиля";
            this.ProfileNumberCol.Width = 30;
            // 
            // ObserverPointHeightCol
            // 
            this.ObserverPointHeightCol.HeaderText = "OH";
            this.ObserverPointHeightCol.Name = "ObserverPointHeightCol";
            this.ObserverPointHeightCol.ReadOnly = true;
            this.ObserverPointHeightCol.ToolTipText = "Высота пункта наблюдения (м)";
            this.ObserverPointHeightCol.Width = 40;
            // 
            // BearingAzimuthCol
            // 
            this.BearingAzimuthCol.HeaderText = "AZ";
            this.BearingAzimuthCol.Name = "BearingAzimuthCol";
            this.BearingAzimuthCol.ReadOnly = true;
            this.BearingAzimuthCol.ToolTipText = "Азимут направления от пункта наблюдения к последней точке профиля";
            this.BearingAzimuthCol.Width = 40;
            // 
            // ProfileLengthCol
            // 
            this.ProfileLengthCol.HeaderText = "L";
            this.ProfileLengthCol.Name = "ProfileLengthCol";
            this.ProfileLengthCol.ReadOnly = true;
            this.ProfileLengthCol.ToolTipText = "Длина профиля (м)";
            this.ProfileLengthCol.Width = 40;
            // 
            // MinHeightCol
            // 
            this.MinHeightCol.HeaderText = "MnH";
            this.MinHeightCol.Name = "MinHeightCol";
            this.MinHeightCol.ReadOnly = true;
            this.MinHeightCol.ToolTipText = "Минимальная высота (м)";
            this.MinHeightCol.Width = 40;
            // 
            // MaxHeightCol
            // 
            this.MaxHeightCol.HeaderText = "MxH";
            this.MaxHeightCol.Name = "MaxHeightCol";
            this.MaxHeightCol.ReadOnly = true;
            this.MaxHeightCol.ToolTipText = "Максимальная высота (м)";
            this.MaxHeightCol.Width = 40;
            // 
            // MinAngleCol
            // 
            this.MinAngleCol.HeaderText = "MnA";
            this.MinAngleCol.Name = "MinAngleCol";
            this.MinAngleCol.ReadOnly = true;
            this.MinAngleCol.ToolTipText = "Максимальный угол спуска (градусы)";
            this.MinAngleCol.Width = 40;
            // 
            // MaxAngleCol
            // 
            this.MaxAngleCol.HeaderText = "MxA";
            this.MaxAngleCol.Name = "MaxAngleCol";
            this.MaxAngleCol.ReadOnly = true;
            this.MaxAngleCol.ToolTipText = "Максимальный угол подъема (графусы)";
            this.MaxAngleCol.Width = 40;
            // 
            // VisiblePercentCol
            // 
            this.VisiblePercentCol.HeaderText = "VP";
            this.VisiblePercentCol.Name = "VisiblePercentCol";
            this.VisiblePercentCol.ReadOnly = true;
            this.VisiblePercentCol.ToolTipText = "Процент видимых участков";
            this.VisiblePercentCol.Width = 40;
            // 
            // SurfaceProfileChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.profilePropertiesTable);
            this.Controls.Add(this.observerHeightTextBox);
            this.Controls.Add(this.observerHeightLabel);
            this.Controls.Add(this.profileChart);
            this.Name = "SurfaceProfileChart";
            this.Size = new System.Drawing.Size(853, 275);
            this.Load += new System.EventHandler(this.SurfaceProfileChart_Load);
            this.Resize += new System.EventHandler(this.SurfaceProfileChart_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.profileChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.profilePropertiesTable)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart profileChart;
        private System.Windows.Forms.Label observerHeightLabel;
        private System.Windows.Forms.TextBox observerHeightTextBox;
        private System.Windows.Forms.DataGridView profilePropertiesTable;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProfileNumberCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn ObserverPointHeightCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn BearingAzimuthCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProfileLengthCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn MinHeightCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn MaxHeightCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn MinAngleCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn MaxAngleCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn VisiblePercentCol;
    }
}
