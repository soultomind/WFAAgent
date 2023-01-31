using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using TestClient.UI;
using TestClientNet45;
using WFAAgent.Framework.Application;
using WFAAgent.Framework.Net.Sockets;
using WFAAgent.Framework.Win32;

namespace TestClient
{
    public partial class MainForm : Form
    {
        public CallbackDataProcess CallbackDataProcess { get; internal set; }
        private Exception ArgException { get; set; }

        private MainWnd _MainWnd;
        private AgentTcpClient TcpClient;

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
                    Process currentProcess = Process.GetCurrentProcess();
                    CallbackDataProcess = CallbackDataProcess.Parse(data, currentProcess);
                    Toolkit.TraceWriteLine("CallbackDataProcess=" + CallbackDataProcess.ToString());
                    if (CallbackDataProcess != null && CallbackDataProcess.UseCallBackData)
                    {
                        /*
                        TcpClient = new AgentTcpClient("127.0.0.1", CallbackDataProcess.AgentTcpServerPort);
                        TcpClient.Connected += TcpClient_Connected;
                        TcpClient.Disconnected += TcpClient_Disconnected;
                        TcpClient.DataReceived += TcpClient_DataReceived;
                        */
                    }

                    Main.AgentErrorDataSend("테스트 AgentErrorDataSend");
                    Main.AgentOutputDataSend("테스트 AgentOutputDataSend");
                }
                catch (Exception ex)
                {
                    ArgException = ex;
                }
            }
        }

        private void TcpClient_Connected(object sender, ConnectedEventArgs e)
        {
            Toolkit.TraceWriteLine("TcpClient_Connected");
        }

        private void TcpClient_Disconnected(object sender, DisconnectEventArgs e)
        {
            Toolkit.TraceWriteLine("TcpClient_Disconnected");
        }

        private void TcpClient_DataReceived(object sender, WFAAgent.Framework.Net.Sockets.DataReceivedEventArgs e)
        {
            Toolkit.TraceWriteLine("TcpClient_DataReceived");
            switch (e.Header.Type)
            {
                case DataContext.ProcessStartData:
                    Toolkit.TraceWriteLine("ProcessStartData=" + e.Data);
                    break;
                case DataContext.ProcessEventData:
                    Toolkit.TraceWriteLine("ProcessEventData=" + e.Data);
                    break;
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            Taskbar.RefreshTrayArea();

            if (ArgException != null)
            {
                MessageBox.Show(this, ArgException.Message, "[ 에러 발생 ]");
                return;
            }

            if (CallbackDataProcess != null && CallbackDataProcess.UseCallBackData)
            {
                if (TcpClient != null && TcpClient.Connect(CallbackDataProcess))
                {

                }
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
