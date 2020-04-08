using System;
using System.Windows.Forms;
using WFAAgent.Dialogs;

namespace WFAAgent
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            InitializeTray();
        }

        private void InitializeTray()
        {
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            Win32Util.RefreshTrayArea();
        }

        public InfoDialog InfoDialog { get; set; }
        public MonitoringDlg MonitoringDlg { get; set; }

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
            if (MonitoringDlg == null)
            {
                MonitoringDlg = new MonitoringDlg();
                MonitoringDlg.FormClosed += MonitoringDlg_FormClosed;
            }

            MonitoringDlg.Show();
        }

        private void MonitoringDlg_FormClosed(object sender, FormClosedEventArgs e)
        {
            MonitoringDlg = null;
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
