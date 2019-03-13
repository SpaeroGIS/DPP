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
            this.surfaceProfileChart1 = new SurfaceProfileChart.SurfaceProfileChartControl.SurfaceProfileChart();
            this.SuspendLayout();
            // 
            // surfaceProfileChart1
            // 
            this.surfaceProfileChart1.Location = new System.Drawing.Point(37, 21);
            this.surfaceProfileChart1.MaxAngel = 0D;
            this.surfaceProfileChart1.MaxHeight = 0D;
            this.surfaceProfileChart1.MinAngel = 0D;
            this.surfaceProfileChart1.MinHeight = 0D;
            this.surfaceProfileChart1.Name = "surfaceProfileChart1";
            this.surfaceProfileChart1.PathLength = 0D;
            this.surfaceProfileChart1.Size = new System.Drawing.Size(664, 221);
            this.surfaceProfileChart1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.surfaceProfileChart1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private SurfaceProfileChartControl.SurfaceProfileChart surfaceProfileChart1;
    }
}

