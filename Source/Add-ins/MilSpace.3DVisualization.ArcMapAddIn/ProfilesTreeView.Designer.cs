namespace MilSpace.Visualization3D
{
    partial class ProfilesTreeView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProfilesTreeView));
            this.UserSessionsProfilesTreeView = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // UserSessionsProfilesTreeView
            // 
            this.UserSessionsProfilesTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UserSessionsProfilesTreeView.Location = new System.Drawing.Point(0, 0);
            this.UserSessionsProfilesTreeView.Name = "UserSessionsProfilesTreeView";
            this.UserSessionsProfilesTreeView.Size = new System.Drawing.Size(228, 281);
            this.UserSessionsProfilesTreeView.TabIndex = 0;
            // 
            // ProfilesTreeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(228, 281);
            this.Controls.Add(this.UserSessionsProfilesTreeView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ProfilesTreeView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ProfilesTreeView";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView UserSessionsProfilesTreeView;
    }
}