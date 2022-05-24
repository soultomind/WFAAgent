using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using TestClient.UI;
using TestClientNet45;
using WFAAgent.Framework.Application;
using WFAAgent.Framework.Win32;

namespace TestClient
{
    public partial class MainForm : Form
    {
        public CallbackDataProcess CallbackDataProcess { get; internal set; }
        private Exception ArgException { get; set; }

        private MainWnd _MainWnd;
        public MainForm()
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

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (Main.Args != null && Main.Args.Length > 0)
            {
                try
                {
                    string arg0 = Main.Args[0];
                    Toolkit.TraceWriteLine("Arg0=" + arg0);
                    byte[] bArg0 = Convert.FromBase64String(arg0);
                    arg0 = Encoding.UTF8.GetString(bArg0);
                    Toolkit.TraceWriteLine("Base64Decode=" + arg0);
                    JObject data = JObject.Parse(arg0);
                    CallbackDataProcess = CallbackDataProcess.Parse(data, Process.GetCurrentProcess());
                    Toolkit.TraceWriteLine("CallbackDataProcess=" + CallbackDataProcess.ToString());
                }
                catch (Exception ex)
                {
                    ArgException = ex;
                }
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            Taskbar.RefreshTrayArea();

            if (ArgException != null)
            {
                MessageBox.Show(this, ArgException.Message, "[ 에러 발생 ]");
            }
        }

        private void TrayNotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (InfoDialog == null)
            {
                InfoDialog = new InfoDialog(CallbackDataProcess.AppId);
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

        public InfoDialog InfoDialog { get; set; }
        private void ToolStripMenuItemExecuteMainWnd_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("실행");
            if (_MainWnd == null)
            {
                _ToolStripMenuItemExecuteMainWnd.Enabled = false;
                _MainWnd = new MainWnd();
                _MainWnd.CallbackDataProcess = CallbackDataProcess;
                _MainWnd.Text = Application.ProductName + " MainWnd AgentTcpServerPort=" + CallbackDataProcess.AgentTcpServerPort;
                _MainWnd.FormClosed += _MainWnd_FormClosed;
                _MainWnd.Show();
            }
        }

        private void _MainWnd_FormClosed(object sender, FormClosedEventArgs e)
        {
            _MainWnd = null;
            _ToolStripMenuItemExecuteMainWnd.Enabled = true;
        }

        private void ToolStripMenuItemAppExit_Click(object sender, EventArgs e)
        {
            if (_MainWnd != null)
            {
                _MainWnd.Close();
            }

            Application.Exit();
        }
    }
}
