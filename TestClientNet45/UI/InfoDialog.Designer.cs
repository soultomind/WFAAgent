namespace TestClient.UI
{
    partial class InfoDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InfoDialog));
            this.GroupBox = new System.Windows.Forms.GroupBox();
            this._LabelValueVersion = new System.Windows.Forms.Label();
            this._LabelTitleVersion = new System.Windows.Forms.Label();
            this._LabelValueAppId = new System.Windows.Forms.Label();
            this._LabelTitleAppId = new System.Windows.Forms.Label();
            this.GroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // GroupBox
            // 
            this.GroupBox.Controls.Add(this._LabelValueAppId);
            this.GroupBox.Controls.Add(this._LabelTitleAppId);
            this.GroupBox.Controls.Add(this._LabelValueVersion);
            this.GroupBox.Controls.Add(this._LabelTitleVersion);
            this.GroupBox.Location = new System.Drawing.Point(12, 8);
            this.GroupBox.Name = "GroupBox";
            this.GroupBox.Size = new System.Drawing.Size(260, 120);
            this.GroupBox.TabIndex = 1;
            this.GroupBox.TabStop = false;
            // 
            // _LabelValueVersion
            // 
            this._LabelValueVersion.AutoSize = true;
            this._LabelValueVersion.Location = new System.Drawing.Point(80, 28);
            this._LabelValueVersion.Name = "_LabelValueVersion";
            this._LabelValueVersion.Size = new System.Drawing.Size(44, 15);
            this._LabelValueVersion.TabIndex = 2;
            this._LabelValueVersion.Text = "1.0.0.0";
            // 
            // _LabelTitleVersion
            // 
            this._LabelTitleVersion.AutoSize = true;
            this._LabelTitleVersion.Location = new System.Drawing.Point(32, 28);
            this._LabelTitleVersion.Name = "_LabelTitleVersion";
            this._LabelTitleVersion.Size = new System.Drawing.Size(42, 15);
            this._LabelTitleVersion.TabIndex = 1;
            this._LabelTitleVersion.Text = "버전 : ";
            // 
            // _LabelValueAppId
            // 
            this._LabelValueAppId.AutoSize = true;
            this._LabelValueAppId.Location = new System.Drawing.Point(80, 53);
            this._LabelValueAppId.Name = "_LabelValueAppId";
            this._LabelValueAppId.Size = new System.Drawing.Size(44, 15);
            this._LabelValueAppId.TabIndex = 4;
            this._LabelValueAppId.Text = "1.0.0.0";
            // 
            // _LabelTitleAppId
            // 
            this._LabelTitleAppId.AutoSize = true;
            this._LabelTitleAppId.Location = new System.Drawing.Point(32, 53);
            this._LabelTitleAppId.Name = "_LabelTitleAppId";
            this._LabelTitleAppId.Size = new System.Drawing.Size(50, 15);
            this._LabelTitleAppId.TabIndex = 3;
            this._LabelTitleAppId.Text = "AppId : ";
            // 
            // InfoDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 137);
            this.Controls.Add(this.GroupBox);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "InfoDialog";
            this.Text = "TestClient";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InfoDialog_FormClosing);
            this.Load += new System.EventHandler(this.InfoDialog_Load);
            this.Shown += new System.EventHandler(this.InfoDialog_Shown);
            this.GroupBox.ResumeLayout(false);
            this.GroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox GroupBox;
        private System.Windows.Forms.Label _LabelValueVersion;
        private System.Windows.Forms.Label _LabelTitleVersion;
        private System.Windows.Forms.Label _LabelValueAppId;
        private System.Windows.Forms.Label _LabelTitleAppId;
    }
}