namespace WFAAgent
{
    partial class MonitoringForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MonitoringForm));
            this.TrayContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItemShowConfigDlg = new System.Windows.Forms.ToolStripMenuItem();
            this._ToolStripMenuItemMonitoring = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.TrayNotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.TrayContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // TrayContextMenuStrip
            // 
            this.TrayContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemShowConfigDlg,
            this._ToolStripMenuItemMonitoring,
            this.ToolStripMenuItemExit});
            this.TrayContextMenuStrip.Name = "TrayContextMenuStrip";
            this.TrayContextMenuStrip.Size = new System.Drawing.Size(123, 70);
            // 
            // ToolStripMenuItemShowConfigDlg
            // 
            this.ToolStripMenuItemShowConfigDlg.Name = "ToolStripMenuItemShowConfigDlg";
            this.ToolStripMenuItemShowConfigDlg.Size = new System.Drawing.Size(122, 22);
            this.ToolStripMenuItemShowConfigDlg.Text = "환경설정";
            this.ToolStripMenuItemShowConfigDlg.Click += new System.EventHandler(this.ToolStripMenuItemShowConfigDlg_Click);
            // 
            // _ToolStripMenuItemMonitoring
            // 
            this._ToolStripMenuItemMonitoring.Name = "_ToolStripMenuItemMonitoring";
            this._ToolStripMenuItemMonitoring.Size = new System.Drawing.Size(122, 22);
            this._ToolStripMenuItemMonitoring.Text = "모니터링";
            this._ToolStripMenuItemMonitoring.Click += new System.EventHandler(this.ToolStripMenuItemMonitoring_Click);
            // 
            // ToolStripMenuItemExit
            // 
            this.ToolStripMenuItemExit.Name = "ToolStripMenuItemExit";
            this.ToolStripMenuItemExit.Size = new System.Drawing.Size(122, 22);
            this.ToolStripMenuItemExit.Text = "종료";
            this.ToolStripMenuItemExit.Click += new System.EventHandler(this.ToolStripMenuItemExit_Click);
            // 
            // TrayNotifyIcon
            // 
            this.TrayNotifyIcon.ContextMenuStrip = this.TrayContextMenuStrip;
            this.TrayNotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("TrayNotifyIcon.Icon")));
            this.TrayNotifyIcon.Text = "WFAAgent.Monitoring";
            this.TrayNotifyIcon.Visible = true;
            this.TrayNotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TrayNotifyIcon_MouseDoubleClick);
            // 
            // MonitoringForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 141);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MonitoringForm";
            this.Text = "WFAAgnet.Monitoring";
            this.Load += new System.EventHandler(this.MonitoringForm_Load);
            this.Shown += new System.EventHandler(this.MonitoringForm_Shown);
            this.TrayContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip TrayContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemShowConfigDlg;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemExit;
        private System.Windows.Forms.NotifyIcon TrayNotifyIcon;
        private System.Windows.Forms.ToolStripMenuItem _ToolStripMenuItemMonitoring;
    }
}