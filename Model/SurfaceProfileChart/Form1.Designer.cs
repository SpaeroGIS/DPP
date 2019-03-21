namespace SurfaceProfileChart
{
    partial class Form1
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.surfaceProfileChart2 = new SurfaceProfileChart.SurfaceProfileChartControl.SurfaceProfileChart();
            this.SuspendLayout();
            // 
            // surfaceProfileChart2
            // 
            this.surfaceProfileChart2.AutoSize = true;
            this.surfaceProfileChart2.Current = false;
            this.surfaceProfileChart2.Location = new System.Drawing.Point(85, 27);
            this.surfaceProfileChart2.MaxAngel = 0D;
            this.surfaceProfileChart2.MaxHeight = 0D;
            this.surfaceProfileChart2.MinAngel = 0D;
            this.surfaceProfileChart2.MinHeight = 0D;
            this.surfaceProfileChart2.Name = "surfaceProfileChart2";
            this.surfaceProfileChart2.PathLength = 0D;
            this.surfaceProfileChart2.SelectedProfileIndex = -1;
            this.surfaceProfileChart2.Size = new System.Drawing.Size(667, 224);
            this.surfaceProfileChart2.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.surfaceProfileChart2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SurfaceProfileChartControl.SurfaceProfileChart surfaceProfileChart1;
        private SurfaceProfileChartControl.SurfaceProfileChart surfaceProfileChart2;
    }
}

