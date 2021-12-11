namespace TestClient
{
    partial class MainForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.TrayNotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.TrayContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._ToolStripMenuItemExecuteMainWnd = new System.Windows.Forms.ToolStripMenuItem();
            this._ToolStripMenuItemAppExit = new System.Windows.Forms.ToolStripMenuItem();
            this.TrayContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // TrayNotifyIcon
            // 
            this.TrayNotifyIcon.ContextMenuStrip = this.TrayContextMenuStrip;
            this.TrayNotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("TrayNotifyIcon.Icon")));
            this.TrayNotifyIcon.Text = "TestClient";
            this.TrayNotifyIcon.Visible = true;
            this.TrayNotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TrayNotifyIcon_MouseDoubleClick);
            // 
            // TrayContextMenuStrip
            // 
            this.TrayContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._ToolStripMenuItemExecuteMainWnd,
            this._ToolStripMenuItemAppExit});
            this.TrayContextMenuStrip.Name = "ContextMenuStrip";
            this.TrayContextMenuStrip.Size = new System.Drawing.Size(99, 48);
            // 
            // _ToolStripMenuItemExecuteMainWnd
            // 
            this._ToolStripMenuItemExecuteMainWnd.Name = "_ToolStripMenuItemExecuteMainWnd";
            this._ToolStripMenuItemExecuteMainWnd.Size = new System.Drawing.Size(98, 22);
            this._ToolStripMenuItemExecuteMainWnd.Text = "실행";
            this._ToolStripMenuItemExecuteMainWnd.Click += new System.EventHandler(this.ToolStripMenuItemExecuteMainWnd_Click);
            // 
            // _ToolStripMenuItemAppExit
            // 
            this._ToolStripMenuItemAppExit.Name = "_ToolStripMenuItemAppExit";
            this._ToolStripMenuItemAppExit.Size = new System.Drawing.Size(98, 22);
            this._ToolStripMenuItemAppExit.Text = "종료";
            this._ToolStripMenuItemAppExit.Click += new System.EventHandler(this.ToolStripMenuItemAppExit_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 141);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MainForm";
            this.Text = "TestClient";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.TrayContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon TrayNotifyIcon;
        private System.Windows.Forms.ContextMenuStrip TrayContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem _ToolStripMenuItemExecuteMainWnd;
        private System.Windows.Forms.ToolStripMenuItem _ToolStripMenuItemAppExit;
    }
}

