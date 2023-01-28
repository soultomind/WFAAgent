using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WFAAgent.Core;
using WFAAgent.Framework.Application;
using WFAAgent.Framework.Win32;
using WFAAgent.Monitoring;

namespace WFAAgent
{
    public partial class MonitoringForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        internal InfoDialog InfoDialog { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        internal TerminalDialog TerminalDialog { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        internal bool IsCurrentProcessExecuteAdministrator { get; set; }

        private ServerProcessWatcher _serverProcessWatcher;

        public MonitoringForm(string[] args)
        {
            InitializeComponent();

            InitializeTaskbarTray();
        }

        private void InitializeTaskbarTray()
        {
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;
        }

        private void MonitoringForm_Load(object sender, EventArgs e)
        {
            IsCurrentProcessExecuteAdministrator = Toolkit.IsCurrentProcessExecuteAdministrator();
            Text = String.Format("WFAAgent.MonitoringForm Administrator={0}", IsCurrentProcessExecuteAdministrator);

            // 윈도우즈 서비스를 활용하여 프로세스가 죽었을때 다시 서비스에서 프로그램을 실행하는 구조 대신에 아래와 같이 
            // 관리자권한(모니터링), 관리자권한,일반(서버) 로 구성하여 개발해보고자 한다.

            if (IsCurrentProcessExecuteAdministrator)
            {
                TerminalDialog = new TerminalDialog();
                TerminalDialog.Text = "TermialDialog." + Execute.Monitoring.ToString();
                TerminalDialog.InitializeMinimized();
            }

            _serverProcessWatcher = new ServerProcessWatcher();
            _serverProcessWatcher.MessageItem += ServerProcessWatcher_MessageItem;
            _serverProcessWatcher.FormLoadEventProcess(IsCurrentProcessExecuteAdministrator);
        }

        private void ServerProcessWatcher_MessageItem(object sender, Message.MessageItemEventArgs e)
        {
            if (TerminalDialog != null && !TerminalDialog.IsDisposed)
            {
                TerminalDialog.AppendText(e.MessageItem.MakeMessage());
            }
        }

        private void MonitoringForm_Shown(object sender, EventArgs e)
        {
            Taskbar.RefreshTrayArea();

            if (IsCurrentProcessExecuteAdministrator)
            {
                _serverProcessWatcher.FormShownEventProcess();
            }
            else
            {
                Close();
            }
        }

        private void TrayNotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (InfoDialog == null)
            {
                InfoDialog = new InfoDialog();
                InfoDialog.Width += 20;
                InfoDialog.Text = String.Format("{0}.{1}", Application.ProductName, Execute.Monitoring.ToString());
            }

            if (Main.HasOpenForm(InfoDialog.Text))
            {
                InfoDialog.Visible = true;
            }
            else
            {
                InfoDialog.Show();
            }
            InfoDialog.Activate();
        }

        private void ToolStripMenuItemShowConfigDlg_Click(object sender, EventArgs e)
        {

        }

        private void ToolStripMenuItemMonitoring_Click(object sender, EventArgs e)
        {
            if (TerminalDialog != null && !TerminalDialog.IsDisposed)
            {
                TerminalDialog.InitializeNormal(Screen.PrimaryScreen.Bounds.Location);
                TerminalDialog.Activate();
            }
        }

        private void ToolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
