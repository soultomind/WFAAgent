using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WFAAgent.Framework.Application;
using WFAAgent.Framework.Net;
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

        private void ButtonImageFileBinary_Click(object sender, EventArgs e)
        {
            string path = String.Empty;
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    path = dlg.FileName;
                }
            }

            MessageBox.Show("Path=" + path);
            byte[] data = ImageToBinary(path);
            MessageBox.Show("Data=" + data.Length);

            _RichTextBoxSendDataAgentTcpServer.Text = Encoding.UTF8.GetString(data);
            // TODO: 데이터 구성에 실제 Binary 데이터 및, 저장할 파일 이름, 파일 크기정보를 포함하여 
            //       웹 클라이언트로 전송하여 웹 클라이언트에서는 다시
            //       서버로 전송하여 서버에서 해당 데이터를 저장하는 플로우를 그려보자
            //       데이터 구조 = { Data : 실제바이너리 데이터, Type : File,General, FileName : Type이 File일 경우 파일 이름 명시, Base64 : Data Base64 처리 유무 }
            TcpClient.Send(DataPacket.AgentBinaryData, new AgentData(CallbackDataProcess, data));
        }

        private void _ButtonSendDataAgentTcpServer_Click(object sender, EventArgs e)
        {
            // TODO: 패킷 값을 통하여 객체를 생성해야 함
            string text = _RichTextBoxSendDataAgentTcpServer.Text;
            TcpClient.Send(DataPacket.AgentStringData, new AgentData(CallbackDataProcess, text));
        }

        private void _ButtonConnectAgentTcpServer_Click(object sender, EventArgs e)
        {
            TcpClient.Connect(CallbackDataProcess);
        }

        public static byte[] ImageToBinary(string path)
        {
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[fileStream.Length];
            fileStream.Read(buffer, 0, (int)fileStream.Length);
            fileStream.Close();
            return buffer;
        }
    }
}
