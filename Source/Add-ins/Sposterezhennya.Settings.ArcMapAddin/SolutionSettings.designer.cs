namespace MilSpace.Settings
{
    partial class SolutionSettingsForm
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("строка подключения");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("пользователь");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("рабочая геобаза");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("");
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.tbConfiguration = new System.Windows.Forms.TabPage();
            this.panel10 = new System.Windows.Forms.Panel();
            this.listView1 = new System.Windows.Forms.ListView();
            this.panel9 = new System.Windows.Forms.Panel();
            this.lblSeanseInfo = new System.Windows.Forms.Label();
            this.tbGraphics = new System.Windows.Forms.TabPage();
            this.panel11 = new System.Windows.Forms.Panel();
            this.chckListBoxShowGraphics = new System.Windows.Forms.CheckedListBox();
            this.panel12 = new System.Windows.Forms.Panel();
            this.lblShowGraphics = new System.Windows.Forms.Label();
            this.panel8 = new System.Windows.Forms.Panel();
            this.chckListBoxClearGraphics = new System.Windows.Forms.CheckedListBox();
            this.panel7 = new System.Windows.Forms.Panel();
            this.lblClearGraphics = new System.Windows.Forms.Label();
            this.tbSurface = new System.Windows.Forms.TabPage();
            this.panel6 = new System.Windows.Forms.Panel();
            this.btnConnectToMap = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.lblSurfaceInfo = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.cmbDEMLayer = new System.Windows.Forms.ComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblDEM = new System.Windows.Forms.Label();
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.panel1.SuspendLayout();
            this.tbConfiguration.SuspendLayout();
            this.panel10.SuspendLayout();
            this.panel9.SuspendLayout();
            this.tbGraphics.SuspendLayout();
            this.panel11.SuspendLayout();
            this.panel12.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel7.SuspendLayout();
            this.tbSurface.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.mainTabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.panel1.Controls.Add(this.btnApply);
            this.panel1.Controls.Add(this.btnExit);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 331);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(4);
            this.panel1.Size = new System.Drawing.Size(504, 30);
            this.panel1.TabIndex = 0;
            // 
            // btnApply
            // 
            this.btnApply.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnApply.Location = new System.Drawing.Point(284, 4);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(108, 22);
            this.btnApply.TabIndex = 1;
            this.btnApply.Text = "применить";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.BtnApply_Click);
            // 
            // btnExit
            // 
            this.btnExit.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnExit.Location = new System.Drawing.Point(392, 4);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(108, 22);
            this.btnExit.TabIndex = 0;
            this.btnExit.Text = "выйти";
            this.btnExit.UseVisualStyleBackColor = true;
            // 
            // tbConfiguration
            // 
            this.tbConfiguration.Controls.Add(this.panel10);
            this.tbConfiguration.Controls.Add(this.panel9);
            this.tbConfiguration.Location = new System.Drawing.Point(4, 25);
            this.tbConfiguration.Name = "tbConfiguration";
            this.tbConfiguration.Size = new System.Drawing.Size(496, 302);
            this.tbConfiguration.TabIndex = 2;
            this.tbConfiguration.Text = "конфигурация (сеанс)";
            this.tbConfiguration.UseVisualStyleBackColor = true;
            // 
            // panel10
            // 
            this.panel10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.panel10.Controls.Add(this.listView1);
            this.panel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel10.Location = new System.Drawing.Point(0, 30);
            this.panel10.Name = "panel10";
            this.panel10.Padding = new System.Windows.Forms.Padding(8);
            this.panel10.Size = new System.Drawing.Size(496, 272);
            this.panel10.TabIndex = 3;
            // 
            // listView1
            // 
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.HideSelection = false;
            this.listView1.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4});
            this.listView1.Location = new System.Drawing.Point(8, 8);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(480, 256);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.List;
            // 
            // panel9
            // 
            this.panel9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.panel9.Controls.Add(this.lblSeanseInfo);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel9.Location = new System.Drawing.Point(0, 0);
            this.panel9.Name = "panel9";
            this.panel9.Padding = new System.Windows.Forms.Padding(4);
            this.panel9.Size = new System.Drawing.Size(496, 30);
            this.panel9.TabIndex = 2;
            // 
            // lblSeanseInfo
            // 
            this.lblSeanseInfo.AutoSize = true;
            this.lblSeanseInfo.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblSeanseInfo.Location = new System.Drawing.Point(4, 4);
            this.lblSeanseInfo.Name = "lblSeanseInfo";
            this.lblSeanseInfo.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.lblSeanseInfo.Size = new System.Drawing.Size(185, 19);
            this.lblSeanseInfo.TabIndex = 0;
            this.lblSeanseInfo.Text = "сведения о сеансе работы";
            // 
            // tbGraphics
            // 
            this.tbGraphics.Controls.Add(this.panel11);
            this.tbGraphics.Controls.Add(this.panel12);
            this.tbGraphics.Controls.Add(this.panel8);
            this.tbGraphics.Controls.Add(this.panel7);
            this.tbGraphics.Location = new System.Drawing.Point(4, 25);
            this.tbGraphics.Name = "tbGraphics";
            this.tbGraphics.Padding = new System.Windows.Forms.Padding(3);
            this.tbGraphics.Size = new System.Drawing.Size(496, 302);
            this.tbGraphics.TabIndex = 1;
            this.tbGraphics.Text = "графика";
            this.tbGraphics.UseVisualStyleBackColor = true;
            // 
            // panel11
            // 
            this.panel11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.panel11.Controls.Add(this.chckListBoxShowGraphics);
            this.panel11.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel11.Location = new System.Drawing.Point(3, 174);
            this.panel11.Name = "panel11";
            this.panel11.Padding = new System.Windows.Forms.Padding(4);
            this.panel11.Size = new System.Drawing.Size(490, 87);
            this.panel11.TabIndex = 6;
            // 
            // chckListBoxShowGraphics
            // 
            this.chckListBoxShowGraphics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chckListBoxShowGraphics.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.chckListBoxShowGraphics.FormattingEnabled = true;
            this.chckListBoxShowGraphics.Items.AddRange(new object[] {
            "вся графика решения",
            "графика Калькулятора Координат",
            "графика Профилей",
            "грфика расчета Видимости"});
            this.chckListBoxShowGraphics.Location = new System.Drawing.Point(4, 4);
            this.chckListBoxShowGraphics.Name = "chckListBoxShowGraphics";
            this.chckListBoxShowGraphics.Size = new System.Drawing.Size(482, 79);
            this.chckListBoxShowGraphics.TabIndex = 0;
            // 
            // panel12
            // 
            this.panel12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.panel12.Controls.Add(this.lblShowGraphics);
            this.panel12.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel12.Location = new System.Drawing.Point(3, 144);
            this.panel12.Name = "panel12";
            this.panel12.Padding = new System.Windows.Forms.Padding(4);
            this.panel12.Size = new System.Drawing.Size(490, 30);
            this.panel12.TabIndex = 5;
            // 
            // lblShowGraphics
            // 
            this.lblShowGraphics.AutoSize = true;
            this.lblShowGraphics.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblShowGraphics.Location = new System.Drawing.Point(4, 4);
            this.lblShowGraphics.Name = "lblShowGraphics";
            this.lblShowGraphics.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.lblShowGraphics.Size = new System.Drawing.Size(164, 19);
            this.lblShowGraphics.TabIndex = 0;
            this.lblShowGraphics.Text = "(2) отобразить графику";
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.panel8.Controls.Add(this.chckListBoxClearGraphics);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel8.Location = new System.Drawing.Point(3, 33);
            this.panel8.Name = "panel8";
            this.panel8.Padding = new System.Windows.Forms.Padding(4);
            this.panel8.Size = new System.Drawing.Size(490, 111);
            this.panel8.TabIndex = 2;
            // 
            // chckListBoxClearGraphics
            // 
            this.chckListBoxClearGraphics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chckListBoxClearGraphics.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.chckListBoxClearGraphics.FormattingEnabled = true;
            this.chckListBoxClearGraphics.Items.AddRange(new object[] {
            "вся графика проекта",
            "вся графика решения",
            "вся графика проекта, но не решения",
            "графика Калькулятора Координат",
            "графика Профилей",
            "грфика расчета Видимости"});
            this.chckListBoxClearGraphics.Location = new System.Drawing.Point(4, 4);
            this.chckListBoxClearGraphics.Name = "chckListBoxClearGraphics";
            this.chckListBoxClearGraphics.Size = new System.Drawing.Size(482, 103);
            this.chckListBoxClearGraphics.TabIndex = 0;
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.panel7.Controls.Add(this.lblClearGraphics);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel7.Location = new System.Drawing.Point(3, 3);
            this.panel7.Name = "panel7";
            this.panel7.Padding = new System.Windows.Forms.Padding(4);
            this.panel7.Size = new System.Drawing.Size(490, 30);
            this.panel7.TabIndex = 1;
            // 
            // lblClearGraphics
            // 
            this.lblClearGraphics.AutoSize = true;
            this.lblClearGraphics.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblClearGraphics.Location = new System.Drawing.Point(4, 4);
            this.lblClearGraphics.Name = "lblClearGraphics";
            this.lblClearGraphics.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.lblClearGraphics.Size = new System.Drawing.Size(148, 19);
            this.lblClearGraphics.TabIndex = 0;
            this.lblClearGraphics.Text = "(1) очистить графику";
            // 
            // tbSurface
            // 
            this.tbSurface.Controls.Add(this.panel6);
            this.tbSurface.Controls.Add(this.panel4);
            this.tbSurface.Controls.Add(this.panel3);
            this.tbSurface.Controls.Add(this.panel2);
            this.tbSurface.Location = new System.Drawing.Point(4, 25);
            this.tbSurface.Name = "tbSurface";
            this.tbSurface.Padding = new System.Windows.Forms.Padding(3);
            this.tbSurface.Size = new System.Drawing.Size(496, 302);
            this.tbSurface.TabIndex = 0;
            this.tbSurface.Text = "поверхность";
            this.tbSurface.UseVisualStyleBackColor = true;
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.panel6.Controls.Add(this.btnConnectToMap);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel6.Location = new System.Drawing.Point(3, 191);
            this.panel6.Name = "panel6";
            this.panel6.Padding = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.panel6.Size = new System.Drawing.Size(490, 30);
            this.panel6.TabIndex = 3;
            // 
            // btnConnectToMap
            // 
            this.btnConnectToMap.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnConnectToMap.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnConnectToMap.Location = new System.Drawing.Point(4, 2);
            this.btnConnectToMap.Name = "btnConnectToMap";
            this.btnConnectToMap.Size = new System.Drawing.Size(158, 26);
            this.btnConnectToMap.TabIndex = 0;
            this.btnConnectToMap.Text = "подключить к карте";
            this.btnConnectToMap.UseVisualStyleBackColor = true;
            this.btnConnectToMap.Click += new System.EventHandler(this.BtnConnectToMap_Click);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.panel4.Controls.Add(this.listBox1);
            this.panel4.Controls.Add(this.panel5);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(3, 67);
            this.panel4.Name = "panel4";
            this.panel4.Padding = new System.Windows.Forms.Padding(8);
            this.panel4.Size = new System.Drawing.Size(490, 124);
            this.panel4.TabIndex = 2;
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 16;
            this.listBox1.Items.AddRange(new object[] {
            "расположение",
            "пространственное разрешение",
            "площадь ()",
            "размер в км",
            "размер в пикселах"});
            this.listBox1.Location = new System.Drawing.Point(8, 38);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(474, 78);
            this.listBox1.TabIndex = 1;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.panel5.Controls.Add(this.lblSurfaceInfo);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(8, 8);
            this.panel5.Name = "panel5";
            this.panel5.Padding = new System.Windows.Forms.Padding(4);
            this.panel5.Size = new System.Drawing.Size(474, 30);
            this.panel5.TabIndex = 0;
            // 
            // lblSurfaceInfo
            // 
            this.lblSurfaceInfo.AutoSize = true;
            this.lblSurfaceInfo.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblSurfaceInfo.Location = new System.Drawing.Point(4, 4);
            this.lblSurfaceInfo.Name = "lblSurfaceInfo";
            this.lblSurfaceInfo.Size = new System.Drawing.Size(191, 17);
            this.lblSurfaceInfo.TabIndex = 0;
            this.lblSurfaceInfo.Text = "информация о поверхности";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.panel3.Controls.Add(this.cmbDEMLayer);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(3, 33);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(4);
            this.panel3.Size = new System.Drawing.Size(490, 34);
            this.panel3.TabIndex = 1;
            // 
            // cmbDEMLayer
            // 
            this.cmbDEMLayer.Dock = System.Windows.Forms.DockStyle.Left;
            this.cmbDEMLayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDEMLayer.FormattingEnabled = true;
            this.cmbDEMLayer.Location = new System.Drawing.Point(4, 4);
            this.cmbDEMLayer.Name = "cmbDEMLayer";
            this.cmbDEMLayer.Size = new System.Drawing.Size(368, 24);
            this.cmbDEMLayer.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.panel2.Controls.Add(this.lblDEM);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(4);
            this.panel2.Size = new System.Drawing.Size(490, 30);
            this.panel2.TabIndex = 0;
            // 
            // lblDEM
            // 
            this.lblDEM.AutoSize = true;
            this.lblDEM.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblDEM.Location = new System.Drawing.Point(4, 4);
            this.lblDEM.Name = "lblDEM";
            this.lblDEM.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.lblDEM.Size = new System.Drawing.Size(281, 19);
            this.lblDEM.TabIndex = 0;
            this.lblDEM.Text = "выбор поверрхности (DEM) для расчетов";
            // 
            // mainTabControl
            // 
            this.mainTabControl.Controls.Add(this.tbSurface);
            this.mainTabControl.Controls.Add(this.tbGraphics);
            this.mainTabControl.Controls.Add(this.tbConfiguration);
            this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.mainTabControl.Location = new System.Drawing.Point(0, 0);
            this.mainTabControl.Margin = new System.Windows.Forms.Padding(0);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(504, 331);
            this.mainTabControl.TabIndex = 1;
            // 
            // SolutionSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 361);
            this.Controls.Add(this.mainTabControl);
            this.Controls.Add(this.panel1);
            this.Name = "SolutionSettingsForm";
            this.Text = "Спостереження. Настройки";
            this.panel1.ResumeLayout(false);
            this.tbConfiguration.ResumeLayout(false);
            this.panel10.ResumeLayout(false);
            this.panel9.ResumeLayout(false);
            this.panel9.PerformLayout();
            this.tbGraphics.ResumeLayout(false);
            this.panel11.ResumeLayout(false);
            this.panel12.ResumeLayout(false);
            this.panel12.PerformLayout();
            this.panel8.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.tbSurface.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.mainTabControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.TabPage tbConfiguration;
        private System.Windows.Forms.TabPage tbGraphics;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.CheckedListBox chckListBoxShowGraphics;
        private System.Windows.Forms.Panel panel12;
        private System.Windows.Forms.Label lblShowGraphics;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.CheckedListBox chckListBoxClearGraphics;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Label lblClearGraphics;
        private System.Windows.Forms.TabPage tbSurface;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Button btnConnectToMap;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label lblSurfaceInfo;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ComboBox cmbDEMLayer;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblDEM;
        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Label lblSeanseInfo;
    }
}

