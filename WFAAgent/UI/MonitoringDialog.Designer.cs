namespace WFAAgent.Dialogs
{
    partial class MonitoringDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MonitoringDialog));
            this._RichTextBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // _RichTextBox
            // 
            this._RichTextBox.BackColor = System.Drawing.Color.Black;
            this._RichTextBox.ForeColor = System.Drawing.Color.White;
            this._RichTextBox.Location = new System.Drawing.Point(12, 12);
            this._RichTextBox.Name = "_RichTextBox";
            this._RichTextBox.Size = new System.Drawing.Size(560, 225);
            this._RichTextBox.TabIndex = 0;
            this._RichTextBox.Text = "";
            // 
            // MonitoringDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 411);
            this.Controls.Add(this._RichTextBox);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MonitoringDlg";
            this.Text = "WFAAgent - Monitoring";
            this.Load += new System.EventHandler(this.MonitoringDlg_Load);
            this.Shown += new System.EventHandler(this.MonitoringDlg_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox _RichTextBox;
    }
}