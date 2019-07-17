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
            this.HydroLayerComboBox = new System.Windows.Forms.ComboBox();
            this.HydroLayerLabel = new System.Windows.Forms.Label();
            this.TransportLayerComboBox = new System.Windows.Forms.ComboBox();
            this.TransportLayerLabel = new System.Windows.Forms.Label();
            this.BuildingsLayerComboBox = new System.Windows.Forms.ComboBox();
            this.BuildingsLayerLabel = new System.Windows.Forms.Label();
            this.PlantsComboBox = new System.Windows.Forms.ComboBox();
            this.PlantsLayerLabel = new System.Windows.Forms.Label();
            this.PlantsHightLablel = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.HydroHightTextBox = new System.Windows.Forms.TextBox();
            this.HydroHightLabel = new System.Windows.Forms.Label();
            this.TransportHightTextBox = new System.Windows.Forms.TextBox();
            this.TransportHightLabel = new System.Windows.Forms.Label();
            this.BuildingsHightComboBox = new System.Windows.Forms.ComboBox();
            this.BuildingsHightLabel = new System.Windows.Forms.Label();
            this.PlantsHightComboBox = new System.Windows.Forms.ComboBox();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.AddButton = new System.Windows.Forms.ToolStripButton();
            this.RemoveButton = new System.Windows.Forms.ToolStripButton();
            this.ProfilesListBox = new System.Windows.Forms.ListBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.ProfilesTabPage = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.listView1 = new System.Windows.Forms.ListView();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel4.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.ProfilesTabPage.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // GenerateButton
            // 
            this.GenerateButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GenerateButton.AutoSize = true;
            this.GenerateButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.GenerateButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.GenerateButton.Location = new System.Drawing.Point(143, 4);
            this.GenerateButton.Name = "GenerateButton";
            this.GenerateButton.Size = new System.Drawing.Size(95, 32);
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
            this.SurfaceLabel.Size = new System.Drawing.Size(72, 22);
            this.SurfaceLabel.TabIndex = 40;
            this.SurfaceLabel.Text = "Surface";
            // 
            // SurfaceComboBox
            // 
            this.SurfaceComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.SurfaceComboBox.FormattingEnabled = true;
            this.SurfaceComboBox.Location = new System.Drawing.Point(0, 22);
            this.SurfaceComboBox.Name = "SurfaceComboBox";
            this.SurfaceComboBox.Size = new System.Drawing.Size(252, 21);
            this.SurfaceComboBox.TabIndex = 41;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.SurfaceComboBox);
            this.panel1.Controls.Add(this.SurfaceLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(254, 44);
            this.panel1.TabIndex = 54;
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.Controls.Add(this.HydroLayerComboBox);
            this.panel2.Controls.Add(this.HydroLayerLabel);
            this.panel2.Controls.Add(this.TransportLayerComboBox);
            this.panel2.Controls.Add(this.TransportLayerLabel);
            this.panel2.Controls.Add(this.BuildingsLayerComboBox);
            this.panel2.Controls.Add(this.BuildingsLayerLabel);
            this.panel2.Controls.Add(this.PlantsComboBox);
            this.panel2.Controls.Add(this.PlantsLayerLabel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(4, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(169, 172);
            this.panel2.TabIndex = 55;
            // 
            // HydroLayerComboBox
            // 
            this.HydroLayerComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.HydroLayerComboBox.FormattingEnabled = true;
            this.HydroLayerComboBox.Location = new System.Drawing.Point(0, 151);
            this.HydroLayerComboBox.Name = "HydroLayerComboBox";
            this.HydroLayerComboBox.Size = new System.Drawing.Size(169, 21);
            this.HydroLayerComboBox.TabIndex = 47;
            // 
            // HydroLayerLabel
            // 
            this.HydroLayerLabel.AutoSize = true;
            this.HydroLayerLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.HydroLayerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.HydroLayerLabel.Location = new System.Drawing.Point(0, 129);
            this.HydroLayerLabel.Name = "HydroLayerLabel";
            this.HydroLayerLabel.Size = new System.Drawing.Size(108, 22);
            this.HydroLayerLabel.TabIndex = 46;
            this.HydroLayerLabel.Text = "Hydro Layer";
            // 
            // TransportLayerComboBox
            // 
            this.TransportLayerComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.TransportLayerComboBox.FormattingEnabled = true;
            this.TransportLayerComboBox.Location = new System.Drawing.Point(0, 108);
            this.TransportLayerComboBox.Name = "TransportLayerComboBox";
            this.TransportLayerComboBox.Size = new System.Drawing.Size(169, 21);
            this.TransportLayerComboBox.TabIndex = 45;
            // 
            // TransportLayerLabel
            // 
            this.TransportLayerLabel.AutoSize = true;
            this.TransportLayerLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TransportLayerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TransportLayerLabel.Location = new System.Drawing.Point(0, 86);
            this.TransportLayerLabel.Name = "TransportLayerLabel";
            this.TransportLayerLabel.Size = new System.Drawing.Size(138, 22);
            this.TransportLayerLabel.TabIndex = 44;
            this.TransportLayerLabel.Text = "Transport Layer";
            // 
            // BuildingsLayerComboBox
            // 
            this.BuildingsLayerComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.BuildingsLayerComboBox.FormattingEnabled = true;
            this.BuildingsLayerComboBox.Location = new System.Drawing.Point(0, 65);
            this.BuildingsLayerComboBox.Name = "BuildingsLayerComboBox";
            this.BuildingsLayerComboBox.Size = new System.Drawing.Size(169, 21);
            this.BuildingsLayerComboBox.TabIndex = 43;
            // 
            // BuildingsLayerLabel
            // 
            this.BuildingsLayerLabel.AutoSize = true;
            this.BuildingsLayerLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.BuildingsLayerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.BuildingsLayerLabel.Location = new System.Drawing.Point(0, 43);
            this.BuildingsLayerLabel.Name = "BuildingsLayerLabel";
            this.BuildingsLayerLabel.Size = new System.Drawing.Size(133, 22);
            this.BuildingsLayerLabel.TabIndex = 42;
            this.BuildingsLayerLabel.Text = "Buildings Layer";
            // 
            // PlantsComboBox
            // 
            this.PlantsComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.PlantsComboBox.FormattingEnabled = true;
            this.PlantsComboBox.Location = new System.Drawing.Point(0, 22);
            this.PlantsComboBox.Name = "PlantsComboBox";
            this.PlantsComboBox.Size = new System.Drawing.Size(169, 21);
            this.PlantsComboBox.TabIndex = 41;
            // 
            // PlantsLayerLabel
            // 
            this.PlantsLayerLabel.AutoSize = true;
            this.PlantsLayerLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.PlantsLayerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PlantsLayerLabel.Location = new System.Drawing.Point(0, 0);
            this.PlantsLayerLabel.Name = "PlantsLayerLabel";
            this.PlantsLayerLabel.Size = new System.Drawing.Size(110, 22);
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
            this.PlantsHightLablel.Size = new System.Drawing.Size(52, 22);
            this.PlantsHightLablel.TabIndex = 52;
            this.PlantsHightLablel.Text = "Hight";
            // 
            // panel3
            // 
            this.panel3.AutoSize = true;
            this.panel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel3.Controls.Add(this.HydroHightTextBox);
            this.panel3.Controls.Add(this.HydroHightLabel);
            this.panel3.Controls.Add(this.TransportHightTextBox);
            this.panel3.Controls.Add(this.TransportHightLabel);
            this.panel3.Controls.Add(this.BuildingsHightComboBox);
            this.panel3.Controls.Add(this.BuildingsHightLabel);
            this.panel3.Controls.Add(this.PlantsHightComboBox);
            this.panel3.Controls.Add(this.PlantsHightLablel);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(180, 4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(70, 172);
            this.panel3.TabIndex = 56;
            // 
            // HydroHightTextBox
            // 
            this.HydroHightTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.HydroHightTextBox.Location = new System.Drawing.Point(0, 150);
            this.HydroHightTextBox.Name = "HydroHightTextBox";
            this.HydroHightTextBox.Size = new System.Drawing.Size(70, 20);
            this.HydroHightTextBox.TabIndex = 61;
            // 
            // HydroHightLabel
            // 
            this.HydroHightLabel.AutoSize = true;
            this.HydroHightLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.HydroHightLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.HydroHightLabel.Location = new System.Drawing.Point(0, 128);
            this.HydroHightLabel.Name = "HydroHightLabel";
            this.HydroHightLabel.Size = new System.Drawing.Size(52, 22);
            this.HydroHightLabel.TabIndex = 60;
            this.HydroHightLabel.Text = "Hight";
            // 
            // TransportHightTextBox
            // 
            this.TransportHightTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.TransportHightTextBox.Location = new System.Drawing.Point(0, 108);
            this.TransportHightTextBox.Name = "TransportHightTextBox";
            this.TransportHightTextBox.Size = new System.Drawing.Size(70, 20);
            this.TransportHightTextBox.TabIndex = 59;
            // 
            // TransportHightLabel
            // 
            this.TransportHightLabel.AutoSize = true;
            this.TransportHightLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TransportHightLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TransportHightLabel.Location = new System.Drawing.Point(0, 86);
            this.TransportHightLabel.Name = "TransportHightLabel";
            this.TransportHightLabel.Size = new System.Drawing.Size(52, 22);
            this.TransportHightLabel.TabIndex = 58;
            this.TransportHightLabel.Text = "Hight";
            // 
            // BuildingsHightComboBox
            // 
            this.BuildingsHightComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.BuildingsHightComboBox.FormattingEnabled = true;
            this.BuildingsHightComboBox.Location = new System.Drawing.Point(0, 65);
            this.BuildingsHightComboBox.Name = "BuildingsHightComboBox";
            this.BuildingsHightComboBox.Size = new System.Drawing.Size(70, 21);
            this.BuildingsHightComboBox.TabIndex = 57;
            // 
            // BuildingsHightLabel
            // 
            this.BuildingsHightLabel.AutoSize = true;
            this.BuildingsHightLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.BuildingsHightLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.BuildingsHightLabel.Location = new System.Drawing.Point(0, 43);
            this.BuildingsHightLabel.Name = "BuildingsHightLabel";
            this.BuildingsHightLabel.Size = new System.Drawing.Size(52, 22);
            this.BuildingsHightLabel.TabIndex = 56;
            this.BuildingsHightLabel.Text = "Hight";
            // 
            // PlantsHightComboBox
            // 
            this.PlantsHightComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.PlantsHightComboBox.FormattingEnabled = true;
            this.PlantsHightComboBox.Location = new System.Drawing.Point(0, 22);
            this.PlantsHightComboBox.Name = "PlantsHightComboBox";
            this.PlantsHightComboBox.Size = new System.Drawing.Size(70, 21);
            this.PlantsHightComboBox.TabIndex = 55;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.panel4);
            this.panel6.Controls.Add(this.toolStrip1);
            this.panel6.Controls.Add(this.ProfilesListBox);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(3, 3);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(238, 224);
            this.panel6.TabIndex = 58;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.GenerateButton);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(0, 192);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(238, 32);
            this.panel4.TabIndex = 61;
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.toolStrip1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AddButton,
            this.RemoveButton});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(238, 31);
            this.toolStrip1.TabIndex = 60;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // AddButton
            // 
            this.AddButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.AddButton.Image = ((System.Drawing.Image)(resources.GetObject("AddButton.Image")));
            this.AddButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(28, 28);
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // RemoveButton
            // 
            this.RemoveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.RemoveButton.Image = ((System.Drawing.Image)(resources.GetObject("RemoveButton.Image")));
            this.RemoveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.RemoveButton.Name = "RemoveButton";
            this.RemoveButton.Size = new System.Drawing.Size(28, 28);
            // 
            // ProfilesListBox
            // 
            this.ProfilesListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProfilesListBox.FormattingEnabled = true;
            this.ProfilesListBox.ItemHeight = 22;
            this.ProfilesListBox.Location = new System.Drawing.Point(0, 0);
            this.ProfilesListBox.Name = "ProfilesListBox";
            this.ProfilesListBox.Size = new System.Drawing.Size(238, 224);
            this.ProfilesListBox.TabIndex = 59;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
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
            this.tableLayoutPanel1.Size = new System.Drawing.Size(254, 180);
            this.tableLayoutPanel1.TabIndex = 64;
            // 
            // splitter1
            // 
            this.splitter1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 224);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(254, 3);
            this.splitter1.TabIndex = 65;
            this.splitter1.TabStop = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.ProfilesTabPage);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tabControl1.Location = new System.Drawing.Point(0, 227);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(254, 267);
            this.tabControl1.TabIndex = 66;
            // 
            // ProfilesTabPage
            // 
            this.ProfilesTabPage.BackColor = System.Drawing.SystemColors.Control;
            this.ProfilesTabPage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ProfilesTabPage.Controls.Add(this.panel6);
            this.ProfilesTabPage.Location = new System.Drawing.Point(4, 31);
            this.ProfilesTabPage.Name = "ProfilesTabPage";
            this.ProfilesTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.ProfilesTabPage.Size = new System.Drawing.Size(246, 232);
            this.ProfilesTabPage.TabIndex = 0;
            this.ProfilesTabPage.Text = "Profiles";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabPage2.Controls.Add(this.listView1);
            this.tabPage2.Location = new System.Drawing.Point(4, 31);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(246, 232);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            // 
            // listView1
            // 
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(3, 3);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(238, 224);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // ProfilesVisualizationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(254, 494);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ProfilesVisualizationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
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
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.ProfilesTabPage.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage ProfilesTabPage;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.ListView listView1;
    }
}