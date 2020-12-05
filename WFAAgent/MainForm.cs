using System;
using System.ComponentModel;
using System.Windows.Forms;
using WFAAgent.Dialogs;

namespace WFAAgent
{
    public partial class MainForm : Form
    {
        private bool isAgentStart;
        private Exception agentStartException;
        private IAgentManager agentManager;
        public MainForm()
        {
            InitializeComponent();

            InitializeTray();

            agentManager = new AgentManager();
        }

        private void InitializeTray()
        {
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                agentManager.StartServer();
                isAgentStart = true;
            }
            catch (Exception ex)
            {
                isAgentStart = false;
                agentStartException = ex;
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            Win32Util.RefreshTrayArea();

            if (!isAgentStart)
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
