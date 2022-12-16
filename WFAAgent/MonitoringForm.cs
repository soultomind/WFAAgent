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
using WFAAgent.Framework.Application;
using WFAAgent.Framework.Win32;

namespace WFAAgent
{
    public partial class MonitoringForm : Form
    {
        
        public MonitoringForm(string[] args)
        {
            InitializeComponent();

            InitializeTaskbarTray();

            Text = String.Format("WFAAgent.MonitoringForm Administrator={0}", Toolkit.IsCurrentProcessAdministrator());
        }

        #region Properties
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        internal InfoDialog InfoDialog { get; set; }

        #endregion

        private void InitializeTaskbarTray()
        {
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;
        }

        private void MonitoringForm_Load(object sender, EventArgs e)
        {
            Text = String.Format("WFAAgent.MonitoringForm Administrator={0}", Toolkit.IsCurrentProcessAdministrator());

            LoadProcess();
        }

        private void MonitoringForm_Shown(object sender, EventArgs e)
        {
            Taskbar.RefreshTrayArea();

            ShownProcess();
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

        private void ShowConfigDlgToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
