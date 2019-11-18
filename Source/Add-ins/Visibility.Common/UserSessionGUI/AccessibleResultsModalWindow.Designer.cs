namespace MilSpace.Visibility
{
    partial class AccessibleProfilesModalWindow
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
            if(disposing && (components != null))
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
            this.lvProfilesSets = new System.Windows.Forms.ListView();
            this.gbFilters = new System.Windows.Forms.GroupBox();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnFilter = new System.Windows.Forms.Button();
            this.cmbGraphType = new System.Windows.Forms.ComboBox();
            this.lblTo = new System.Windows.Forms.Label();
            this.toDate = new System.Windows.Forms.DateTimePicker();
            this.lblFrom = new System.Windows.Forms.Label();
            this.fromDate = new System.Windows.Forms.DateTimePicker();
            this.lblDateText = new System.Windows.Forms.Label();
            this.txtCreator = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.gbFilters.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvProfilesSets
            // 
            this.lvProfilesSets.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvProfilesSets.FullRowSelect = true;
            this.lvProfilesSets.HideSelection = false;
            this.lvProfilesSets.Location = new System.Drawing.Point(10, 124);
            this.lvProfilesSets.Name = "lvProfilesSets";
            this.lvProfilesSets.Size = new System.Drawing.Size(583, 224);
            this.lvProfilesSets.TabIndex = 0;
            this.lvProfilesSets.UseCompatibleStateImageBehavior = false;
            // 
            // gbFilters
            // 
            this.gbFilters.Controls.Add(this.btnReset);
            this.gbFilters.Controls.Add(this.btnFilter);
            this.gbFilters.Controls.Add(this.cmbGraphType);
            this.gbFilters.Controls.Add(this.lblTo);
            this.gbFilters.Controls.Add(this.toDate);
            this.gbFilters.Controls.Add(this.lblFrom);
            this.gbFilters.Controls.Add(this.fromDate);
            this.gbFilters.Controls.Add(this.lblDateText);
            this.gbFilters.Controls.Add(this.txtCreator);
            this.gbFilters.Controls.Add(this.txtName);
            this.gbFilters.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbFilters.Location = new System.Drawing.Point(10, 5);
            this.gbFilters.Name = "gbFilters";
            this.gbFilters.Size = new System.Drawing.Size(583, 113);
            this.gbFilters.TabIndex = 1;
            this.gbFilters.TabStop = false;
            this.gbFilters.Text = "Filters";
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReset.Location = new System.Drawing.Point(505, 83);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(72, 20);
            this.btnReset.TabIndex = 10;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.BtnReset_Click);
            // 
            // btnFilter
            // 
            this.btnFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFilter.Location = new System.Drawing.Point(419, 83);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(80, 20);
            this.btnFilter.TabIndex = 9;
            this.btnFilter.Text = "Filter";
            this.btnFilter.UseVisualStyleBackColor = true;
            this.btnFilter.Click += new System.EventHandler(this.BtnFilter_Click);
            // 
            // cmbGraphType
            // 
            this.cmbGraphType.ForeColor = System.Drawing.Color.Black;
            this.cmbGraphType.FormattingEnabled = true;
            this.cmbGraphType.Location = new System.Drawing.Point(13, 82);
            this.cmbGraphType.Name = "cmbGraphType";
            this.cmbGraphType.Size = new System.Drawing.Size(172, 21);
            this.cmbGraphType.TabIndex = 7;
            this.cmbGraphType.Text = "Type";
            // 
            // lblTo
            // 
            this.lblTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTo.AutoSize = true;
            this.lblTo.Location = new System.Drawing.Point(474, 55);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(16, 13);
            this.lblTo.TabIndex = 6;
            this.lblTo.Text = "to";
            // 
            // toDate
            // 
            this.toDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.toDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.toDate.Location = new System.Drawing.Point(496, 52);
            this.toDate.Name = "toDate";
            this.toDate.Size = new System.Drawing.Size(80, 20);
            this.toDate.TabIndex = 5;
            // 
            // lblFrom
            // 
            this.lblFrom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFrom.AutoSize = true;
            this.lblFrom.Location = new System.Drawing.Point(351, 55);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(27, 13);
            this.lblFrom.TabIndex = 4;
            this.lblFrom.Text = "from";
            // 
            // fromDate
            // 
            this.fromDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fromDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.fromDate.Location = new System.Drawing.Point(384, 52);
            this.fromDate.Name = "fromDate";
            this.fromDate.Size = new System.Drawing.Size(80, 20);
            this.fromDate.TabIndex = 3;
            // 
            // lblDateText
            // 
            this.lblDateText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDateText.AutoSize = true;
            this.lblDateText.Location = new System.Drawing.Point(351, 22);
            this.lblDateText.Name = "lblDateText";
            this.lblDateText.Size = new System.Drawing.Size(73, 13);
            this.lblDateText.TabIndex = 2;
            this.lblDateText.Text = "Creation date:";
            // 
            // txtCreator
            // 
            this.txtCreator.ForeColor = System.Drawing.Color.DimGray;
            this.txtCreator.Location = new System.Drawing.Point(13, 52);
            this.txtCreator.Name = "txtCreator";
            this.txtCreator.Size = new System.Drawing.Size(172, 20);
            this.txtCreator.TabIndex = 1;
            this.txtCreator.Text = "Creator";
            this.txtCreator.Enter += new System.EventHandler(this.TxtCreator_Enter);
            this.txtCreator.Leave += new System.EventHandler(this.TxtCreator_Leave);
            // 
            // txtName
            // 
            this.txtName.ForeColor = System.Drawing.Color.DimGray;
            this.txtName.Location = new System.Drawing.Point(13, 22);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(172, 20);
            this.txtName.TabIndex = 0;
            this.txtName.Text = "Name";
            this.txtName.Enter += new System.EventHandler(this.TxtName_Enter);
            this.txtName.Leave += new System.EventHandler(this.TxtName_Leave);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(10, 354);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(583, 25);
            this.panel1.TabIndex = 2;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCancel.Location = new System.Drawing.Point(515, 0);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(68, 25);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnOk.Location = new System.Drawing.Point(0, 0);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(118, 25);
            this.btnOk.TabIndex = 10;
            this.btnOk.Text = "Add to session";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.BtnOk_Click);
            // 
            // AccessibleProfilesModalWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(603, 389);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.gbFilters);
            this.Controls.Add(this.lvProfilesSets);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AccessibleProfilesModalWindow";
            this.Padding = new System.Windows.Forms.Padding(10, 5, 10, 10);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SavedProfilesModalWindow";
            this.gbFilters.ResumeLayout(false);
            this.gbFilters.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvProfilesSets;
        private System.Windows.Forms.GroupBox gbFilters;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtCreator;
        private System.Windows.Forms.Label lblDateText;
        private System.Windows.Forms.DateTimePicker fromDate;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.DateTimePicker toDate;
        private System.Windows.Forms.Label lblFrom;
        private System.Windows.Forms.ComboBox cmbGraphType;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnReset;
    }
}