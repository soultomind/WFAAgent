namespace WFAAgent
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
            this.VersionValueLabel = new System.Windows.Forms.Label();
            this.VersionTitleLabel = new System.Windows.Forms.Label();
            this.GroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // GroupBox
            // 
            this.GroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupBox.Controls.Add(this.VersionValueLabel);
            this.GroupBox.Controls.Add(this.VersionTitleLabel);
            this.GroupBox.Location = new System.Drawing.Point(12, 5);
            this.GroupBox.Name = "GroupBox";
            this.GroupBox.Size = new System.Drawing.Size(260, 120);
            this.GroupBox.TabIndex = 0;
            this.GroupBox.TabStop = false;
            // 
            // VersionValueLabel
            // 
            this.VersionValueLabel.AutoSize = true;
            this.VersionValueLabel.Location = new System.Drawing.Point(80, 28);
            this.VersionValueLabel.Name = "VersionValueLabel";
            this.VersionValueLabel.Size = new System.Drawing.Size(44, 15);
            this.VersionValueLabel.TabIndex = 2;
            this.VersionValueLabel.Text = "1.0.0.0";
            // 
            // VersionTitleLabel
            // 
            this.VersionTitleLabel.AutoSize = true;
            this.VersionTitleLabel.Location = new System.Drawing.Point(32, 28);
            this.VersionTitleLabel.Name = "VersionTitleLabel";
            this.VersionTitleLabel.Size = new System.Drawing.Size(42, 15);
            this.VersionTitleLabel.TabIndex = 1;
            this.VersionTitleLabel.Text = "버전 : ";
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
            this.Text = "WFAAgent";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InfoDialog_FormClosing);
            this.Load += new System.EventHandler(this.InfoDialog_Load);
            this.Shown += new System.EventHandler(this.InfoDialog_Shown);
            this.GroupBox.ResumeLayout(false);
            this.GroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox GroupBox;
        private System.Windows.Forms.Label VersionValueLabel;
        private System.Windows.Forms.Label VersionTitleLabel;
    }
}