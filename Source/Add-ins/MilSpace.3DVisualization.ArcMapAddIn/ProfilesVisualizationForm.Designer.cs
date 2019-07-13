namespace MilSpace.Visualization3D
{
    partial class ProfilesVisualizationForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProfilesVisualizationForm));
            this.GenerateButton = new System.Windows.Forms.Button();
            this.SurfaceLabel = new System.Windows.Forms.Label();
            this.SurfaceComboBox = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.PlantsComboBox = new System.Windows.Forms.ComboBox();
            this.PlantsLayerLabel = new System.Windows.Forms.Label();
            this.PlantsHightLablel = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.AddButton = new System.Windows.Forms.ToolStripButton();
            this.RemoveButton = new System.Windows.Forms.ToolStripButton();
            this.ProfilesListBox = new System.Windows.Forms.ListBox();
            this.ProfilesLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.BuildingsLayerLabel = new System.Windows.Forms.Label();
            this.BuildingsLayerComboBox = new System.Windows.Forms.ComboBox();
            this.TransportLayerLabel = new System.Windows.Forms.Label();
            this.TransportLayerComboBox = new System.Windows.Forms.ComboBox();
            this.HydroLayerLabel = new System.Windows.Forms.Label();
            this.HydroLayerComboBox = new System.Windows.Forms.ComboBox();
            this.PlantsHightComboBox = new System.Windows.Forms.ComboBox();
            this.BuildingsHightLabel = new System.Windows.Forms.Label();
            this.BuildingsHightComboBox = new System.Windows.Forms.ComboBox();
            this.TransportHightLabel = new System.Windows.Forms.Label();
            this.TransportHightTextBox = new System.Windows.Forms.TextBox();
            this.HydroHightLabel = new System.Windows.Forms.Label();
            this.HydroHightTextBox = new System.Windows.Forms.TextBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel6.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // GenerateButton
            // 
            this.GenerateButton.AutoSize = true;
            this.GenerateButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.GenerateButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.GenerateButton.Location = new System.Drawing.Point(0, 180);
            this.GenerateButton.Name = "GenerateButton";
            this.GenerateButton.Size = new System.Drawing.Size(260, 25);
            this.GenerateButton.TabIndex = 1;
            this.GenerateButton.Text = "Generate";
            this.GenerateButton.UseVisualStyleBackColor = true;
            // 
            // SurfaceLabel
            // 
            this.SurfaceLabel.AutoSize = true;
            this.SurfaceLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.SurfaceLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SurfaceLabel.Location = new System.Drawing.Point(0, 0);
            this.SurfaceLabel.Name = "SurfaceLabel";
            this.SurfaceLabel.Size = new System.Drawing.Size(49, 15);
            this.SurfaceLabel.TabIndex = 40;
            this.SurfaceLabel.Text = "Surface";
            // 
            // SurfaceComboBox
            // 
            this.SurfaceComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.SurfaceComboBox.FormattingEnabled = true;
            this.SurfaceComboBox.Location = new System.Drawing.Point(0, 15);
            this.SurfaceComboBox.Name = "SurfaceComboBox";
            this.SurfaceComboBox.Size = new System.Drawing.Size(284, 21);
            this.SurfaceComboBox.TabIndex = 41;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.SurfaceComboBox);
            this.panel1.Controls.Add(this.SurfaceLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(284, 44);
            this.panel1.TabIndex = 54;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.HydroLayerComboBox);
            this.panel2.Controls.Add(this.HydroLayerLabel);
            this.panel2.Controls.Add(this.TransportLayerComboBox);
            this.panel2.Controls.Add(this.TransportLayerLabel);
            this.panel2.Controls.Add(this.BuildingsLayerComboBox);
            this.panel2.Controls.Add(this.BuildingsLayerLabel);
            this.panel2.Controls.Add(this.PlantsComboBox);
            this.panel2.Controls.Add(this.PlantsLayerLabel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(192, 153);
            this.panel2.TabIndex = 55;
            // 
            // PlantsComboBox
            // 
            this.PlantsComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.PlantsComboBox.FormattingEnabled = true;
            this.PlantsComboBox.Location = new System.Drawing.Point(0, 15);
            this.PlantsComboBox.Name = "PlantsComboBox";
            this.PlantsComboBox.Size = new System.Drawing.Size(192, 21);
            this.PlantsComboBox.TabIndex = 41;
            // 
            // PlantsLayerLabel
            // 
            this.PlantsLayerLabel.AutoSize = true;
            this.PlantsLayerLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.PlantsLayerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PlantsLayerLabel.Location = new System.Drawing.Point(0, 0);
            this.PlantsLayerLabel.Name = "PlantsLayerLabel";
            this.PlantsLayerLabel.Size = new System.Drawing.Size(74, 15);
            this.PlantsLayerLabel.TabIndex = 40;
            this.PlantsLayerLabel.Text = "Plants Layer";
            // 
            // PlantsHightLablel
            // 
            this.PlantsHightLablel.AutoSize = true;
            this.PlantsHightLablel.Dock = System.Windows.Forms.DockStyle.Top;
            this.PlantsHightLablel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PlantsHightLablel.Location = new System.Drawing.Point(0, 0);
            this.PlantsHightLablel.Name = "PlantsHightLablel";
            this.PlantsHightLablel.Size = new System.Drawing.Size(36, 15);
            this.PlantsHightLablel.TabIndex = 52;
            this.PlantsHightLablel.Text = "Hight";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.HydroHightTextBox);
            this.panel3.Controls.Add(this.HydroHightLabel);
            this.panel3.Controls.Add(this.TransportHightTextBox);
            this.panel3.Controls.Add(this.TransportHightLabel);
            this.panel3.Controls.Add(this.BuildingsHightComboBox);
            this.panel3.Controls.Add(this.BuildingsHightLabel);
            this.panel3.Controls.Add(this.PlantsHightComboBox);
            this.panel3.Controls.Add(this.PlantsHightLablel);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(201, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(80, 153);
            this.panel3.TabIndex = 56;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.GenerateButton);
            this.panel6.Controls.Add(this.toolStrip1);
            this.panel6.Controls.Add(this.ProfilesListBox);
            this.panel6.Controls.Add(this.ProfilesLabel);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(0, 206);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(284, 205);
            this.panel6.TabIndex = 58;
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.toolStrip1.CanOverflow = false;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Right;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AddButton,
            this.RemoveButton});
            this.toolStrip1.Location = new System.Drawing.Point(260, 15);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(24, 190);
            this.toolStrip1.TabIndex = 60;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // AddButton
            // 
            this.AddButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.AddButton.Image = ((System.Drawing.Image)(resources.GetObject("AddButton.Image")));
            this.AddButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(21, 20);
            // 
            // RemoveButton
            // 
            this.RemoveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.RemoveButton.Image = ((System.Drawing.Image)(resources.GetObject("RemoveButton.Image")));
            this.RemoveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.RemoveButton.Name = "RemoveButton";
            this.RemoveButton.Size = new System.Drawing.Size(21, 20);
            // 
            // ProfilesListBox
            // 
            this.ProfilesListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProfilesListBox.FormattingEnabled = true;
            this.ProfilesListBox.Location = new System.Drawing.Point(0, 15);
            this.ProfilesListBox.Name = "ProfilesListBox";
            this.ProfilesListBox.Size = new System.Drawing.Size(284, 190);
            this.ProfilesListBox.TabIndex = 59;
            // 
            // ProfilesLabel
            // 
            this.ProfilesLabel.AutoSize = true;
            this.ProfilesLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.ProfilesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ProfilesLabel.Location = new System.Drawing.Point(0, 0);
            this.ProfilesLabel.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.ProfilesLabel.Name = "ProfilesLabel";
            this.ProfilesLabel.Size = new System.Drawing.Size(48, 15);
            this.ProfilesLabel.TabIndex = 40;
            this.ProfilesLabel.Text = "Profiles";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 44);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(284, 159);
            this.tableLayoutPanel1.TabIndex = 64;
            // 
            // BuildingsLayerLabel
            // 
            this.BuildingsLayerLabel.AutoSize = true;
            this.BuildingsLayerLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.BuildingsLayerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.BuildingsLayerLabel.Location = new System.Drawing.Point(0, 36);
            this.BuildingsLayerLabel.Name = "BuildingsLayerLabel";
            this.BuildingsLayerLabel.Size = new System.Drawing.Size(91, 15);
            this.BuildingsLayerLabel.TabIndex = 42;
            this.BuildingsLayerLabel.Text = "Buildings Layer";
            // 
            // BuildingsLayerComboBox
            // 
            this.BuildingsLayerComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.BuildingsLayerComboBox.FormattingEnabled = true;
            this.BuildingsLayerComboBox.Location = new System.Drawing.Point(0, 51);
            this.BuildingsLayerComboBox.Name = "BuildingsLayerComboBox";
            this.BuildingsLayerComboBox.Size = new System.Drawing.Size(192, 21);
            this.BuildingsLayerComboBox.TabIndex = 43;
            // 
            // TransportLayerLabel
            // 
            this.TransportLayerLabel.AutoSize = true;
            this.TransportLayerLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TransportLayerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TransportLayerLabel.Location = new System.Drawing.Point(0, 72);
            this.TransportLayerLabel.Name = "TransportLayerLabel";
            this.TransportLayerLabel.Size = new System.Drawing.Size(92, 15);
            this.TransportLayerLabel.TabIndex = 44;
            this.TransportLayerLabel.Text = "Transport Layer";
            // 
            // TransportLayerComboBox
            // 
            this.TransportLayerComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.TransportLayerComboBox.FormattingEnabled = true;
            this.TransportLayerComboBox.Location = new System.Drawing.Point(0, 87);
            this.TransportLayerComboBox.Name = "TransportLayerComboBox";
            this.TransportLayerComboBox.Size = new System.Drawing.Size(192, 21);
            this.TransportLayerComboBox.TabIndex = 45;
            // 
            // HydroLayerLabel
            // 
            this.HydroLayerLabel.AutoSize = true;
            this.HydroLayerLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.HydroLayerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.HydroLayerLabel.Location = new System.Drawing.Point(0, 108);
            this.HydroLayerLabel.Name = "HydroLayerLabel";
            this.HydroLayerLabel.Size = new System.Drawing.Size(72, 15);
            this.HydroLayerLabel.TabIndex = 46;
            this.HydroLayerLabel.Text = "Hydro Layer";
            // 
            // HydroLayerComboBox
            // 
            this.HydroLayerComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.HydroLayerComboBox.FormattingEnabled = true;
            this.HydroLayerComboBox.Location = new System.Drawing.Point(0, 123);
            this.HydroLayerComboBox.Name = "HydroLayerComboBox";
            this.HydroLayerComboBox.Size = new System.Drawing.Size(192, 21);
            this.HydroLayerComboBox.TabIndex = 47;
            // 
            // PlantsHightComboBox
            // 
            this.PlantsHightComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.PlantsHightComboBox.FormattingEnabled = true;
            this.PlantsHightComboBox.Location = new System.Drawing.Point(0, 15);
            this.PlantsHightComboBox.Name = "PlantsHightComboBox";
            this.PlantsHightComboBox.Size = new System.Drawing.Size(80, 21);
            this.PlantsHightComboBox.TabIndex = 55;
            // 
            // BuildingsHightLabel
            // 
            this.BuildingsHightLabel.AutoSize = true;
            this.BuildingsHightLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.BuildingsHightLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.BuildingsHightLabel.Location = new System.Drawing.Point(0, 36);
            this.BuildingsHightLabel.Name = "BuildingsHightLabel";
            this.BuildingsHightLabel.Size = new System.Drawing.Size(36, 15);
            this.BuildingsHightLabel.TabIndex = 56;
            this.BuildingsHightLabel.Text = "Hight";
            // 
            // BuildingsHightComboBox
            // 
            this.BuildingsHightComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.BuildingsHightComboBox.FormattingEnabled = true;
            this.BuildingsHightComboBox.Location = new System.Drawing.Point(0, 51);
            this.BuildingsHightComboBox.Name = "BuildingsHightComboBox";
            this.BuildingsHightComboBox.Size = new System.Drawing.Size(80, 21);
            this.BuildingsHightComboBox.TabIndex = 57;
            // 
            // TransportHightLabel
            // 
            this.TransportHightLabel.AutoSize = true;
            this.TransportHightLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TransportHightLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TransportHightLabel.Location = new System.Drawing.Point(0, 72);
            this.TransportHightLabel.Name = "TransportHightLabel";
            this.TransportHightLabel.Size = new System.Drawing.Size(36, 15);
            this.TransportHightLabel.TabIndex = 58;
            this.TransportHightLabel.Text = "Hight";
            // 
            // TransportHightTextBox
            // 
            this.TransportHightTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.TransportHightTextBox.Location = new System.Drawing.Point(0, 87);
            this.TransportHightTextBox.Name = "TransportHightTextBox";
            this.TransportHightTextBox.Size = new System.Drawing.Size(80, 20);
            this.TransportHightTextBox.TabIndex = 59;
            // 
            // HydroHightLabel
            // 
            this.HydroHightLabel.AutoSize = true;
            this.HydroHightLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.HydroHightLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.HydroHightLabel.Location = new System.Drawing.Point(0, 107);
            this.HydroHightLabel.Name = "HydroHightLabel";
            this.HydroHightLabel.Size = new System.Drawing.Size(36, 15);
            this.HydroHightLabel.TabIndex = 60;
            this.HydroHightLabel.Text = "Hight";
            // 
            // HydroHightTextBox
            // 
            this.HydroHightTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.HydroHightTextBox.Location = new System.Drawing.Point(0, 122);
            this.HydroHightTextBox.Name = "HydroHightTextBox";
            this.HydroHightTextBox.Size = new System.Drawing.Size(80, 20);
            this.HydroHightTextBox.TabIndex = 61;
            // 
            // splitter1
            // 
            this.splitter1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 203);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(284, 3);
            this.splitter1.TabIndex = 65;
            this.splitter1.TabStop = false;
            // 
            // ProfilesVisualizationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 411);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ProfilesVisualizationForm";
            this.Text = "Profiles Visualization Form";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProfilesVisualizationForm_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button GenerateButton;
        private System.Windows.Forms.Label SurfaceLabel;
        private System.Windows.Forms.ComboBox SurfaceComboBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox PlantsComboBox;
        private System.Windows.Forms.Label PlantsLayerLabel;
        private System.Windows.Forms.Label PlantsHightLablel;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton AddButton;
        private System.Windows.Forms.ToolStripButton RemoveButton;
        private System.Windows.Forms.ListBox ProfilesListBox;
        private System.Windows.Forms.Label ProfilesLabel;
        private System.Windows.Forms.ComboBox HydroLayerComboBox;
        private System.Windows.Forms.Label HydroLayerLabel;
        private System.Windows.Forms.ComboBox TransportLayerComboBox;
        private System.Windows.Forms.Label TransportLayerLabel;
        private System.Windows.Forms.ComboBox BuildingsLayerComboBox;
        private System.Windows.Forms.Label BuildingsLayerLabel;
        private System.Windows.Forms.TextBox HydroHightTextBox;
        private System.Windows.Forms.Label HydroHightLabel;
        private System.Windows.Forms.TextBox TransportHightTextBox;
        private System.Windows.Forms.Label TransportHightLabel;
        private System.Windows.Forms.ComboBox BuildingsHightComboBox;
        private System.Windows.Forms.Label BuildingsHightLabel;
        private System.Windows.Forms.ComboBox PlantsHightComboBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Splitter splitter1;
    }
}