using CommonLibrary;
using System;
using System.ComponentModel;
using System.Windows.Forms;
using WFAAgent.Dialogs;

namespace WFAAgent
{
    public partial class MainForm : Form
    {
        private bool _IsAgentStart;
        private Exception _AgentStartException;
        private IAgentManager _AgentManager;
        public MainForm()
        {
            InitializeComponent();

            InitializeTaskbarTray();

            _AgentManager = new AgentManager();
            _AgentManager.MessageObjectReceived += AgentManager_MessageObjectReceived; ;
        }

        private void AgentManager_MessageObjectReceived(object messageObject)
        {
#if DEBUG
            Toolkit.DebugWriteLine(messageObject.ToString());
#else
            Toolkit.TraceWriteLine(messageObject.ToString());
#endif

        }

        private void InitializeTaskbarTray()
        {
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                _AgentManager.StartServer();
                _IsAgentStart = true;
            }
            catch (Exception ex)
            {
                _IsAgentStart = false;
                _AgentStartException = ex;
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            Taskbar.RefreshTrayArea();

            if (!_IsAgentStart)
            {
                // TODO: 에외 출력
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        internal InfoDialog InfoDialog { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        internal MonitoringDlg MonitoringDialog { get; set; }

        private void TrayNotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (InfoDialog == null)
            {
                InfoDialog = new InfoDialog();
            }

            if (Main.OpenForm(InfoDialog.Text))
            {
                InfoDialog.ReShow();
            }
            else
            {
                InfoDialog.Show();
            }
        }

        private void ShowMonitoringDlgToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MonitoringDialog == null)
            {
                MonitoringDialog = new MonitoringDlg();
                MonitoringDialog.FormClosed += MonitoringDlg_FormClosed;
            }

            MonitoringDialog.Show();
        }

        private void MonitoringDlg_FormClosed(object sender, FormClosedEventArgs e)
        {
            MonitoringDialog = null;
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


    }
}
