using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WFAAgent.Framework.Application;
using WFAAgent.Framework.Net.Sockets;

namespace TestClientNet45
{
    public partial class MainWnd : Form
    {
        internal AgentTcpClient TcpClient { get; set; }
        internal CallbackDataProcess CallbackDataProcess;
        public MainWnd()
        {
            InitializeComponent();

            TcpClient = new AgentTcpClient();
            TcpClient.Connected += TcpClient_Connected;
            TcpClient.Disconnected += TcpClient_Disconnected;
            TcpClient.DataReceived += TcpClient_DataReceived;
        }

        private void MainWnd_Load(object sender, EventArgs e)
        {

        }

        private void MainWnd_Shown(object sender, EventArgs e)
        {

        }

        private void MainWnd_FormClosed(object sender, FormClosedEventArgs e)
        {
            TcpClient.Disconnect();
        }

        private void TcpClient_Connected(object sender, ConnectedEventArgs e)
        {
            RichTextBoxReceiveDataAgentTcpServer_AppendText("========= TcpClient_Connected ");
            RichTextBoxReceiveDataAgentTcpServer_AppendText("TcpClient_Connected =========");
        }

        private void TcpClient_Disconnected(object sender, DisconnectEventArgs e)
        {
            RichTextBoxReceiveDataAgentTcpServer_AppendText("========= TcpClient_Disconnected ");
            RichTextBoxReceiveDataAgentTcpServer_AppendText("TcpClient_Disconnected =========");
        }

        private void TcpClient_DataReceived(object sender, DataReceivedEventArgs e)
        {
            RichTextBoxReceiveDataAgentTcpServer_AppendText("========= TcpClient_DataReceived ");
            RichTextBoxReceiveDataAgentTcpServer_AppendText(e.Data);
            RichTextBoxReceiveDataAgentTcpServer_AppendText("TcpClient_DataReceived =========");
        }

        private void RichTextBoxReceiveDataAgentTcpServer_AppendText(string text)
        {
            if (InvokeRequired)
            {
                _RichTextBoxReceiveDataAgentTcpServer.Invoke(new Action(() =>
                {
                    _RichTextBoxReceiveDataAgentTcpServer.AppendText(text);
                    _RichTextBoxReceiveDataAgentTcpServer.AppendText(Environment.NewLine);
                    _RichTextBoxReceiveDataAgentTcpServer.ScrollToCaret();
                }));
            }
            else
            {
                _RichTextBoxReceiveDataAgentTcpServer.AppendText(text);
                _RichTextBoxReceiveDataAgentTcpServer.AppendText(Environment.NewLine);
                _RichTextBoxReceiveDataAgentTcpServer.ScrollToCaret();
            }
        }

        private void _ButtonSendDataAgentTcpServer_Click(object sender, EventArgs e)
        {
            JObject data = CallbackDataProcess.ToUserDataJson(_RichTextBoxSendDataAgentTcpServer.Text);
            TcpClient.Send(DataPacket.UserData, data.ToString());
        }

        private void _ButtonConnectAgentTcpServer_Click(object sender, EventArgs e)
        {
            TcpClient.Connect(CallbackDataProcess);
        }
    }
}
