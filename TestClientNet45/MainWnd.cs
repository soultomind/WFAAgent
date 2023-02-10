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
        }

        private void MainWnd_Load(object sender, EventArgs e)
        {

        }

        private void MainWnd_Shown(object sender, EventArgs e)
        {

        }

        private void MainWnd_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (TcpClient == null)
            {
                return;
            }

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
                _RichTextBoxReceiveDataAgentTcpServer.Invoke(new Action<string>(RichTextBoxReceiveDataAgentTcpServer_AppendText), text);
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
            if (TcpClient == null)
            {
                return;
            }

            string path = String.Empty;
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    path = dlg.FileName;
                }
            }

            byte[] data = null;
            using (Image image = Image.FromFile(path))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, image.RawFormat);
                    data = ms.ToArray();
                }
            }

            _RichTextBoxSendDataAgentTcpServer.Text = Encoding.UTF8.GetString(data);
            // TODO: 데이터 구성에 실제 Binary 데이터 및, 저장할 파일 이름, 파일 크기정보를 포함하여 
            //       웹 클라이언트로 전송하여 웹 클라이언트에서는 다시
            //       서버로 전송하여 서버에서 해당 데이터를 저장하는 플로우를 그려보자
            
            TcpClient.Send(DataPacket.AgentStringData,
                new AgentStringData(CallbackDataProcess, data)
                {
                    IsBase64 = true,
                    BinaryData = true,
                    Extension = new FileInfo(path).Extension
                }
            );


            /*
            TcpClient.Send(DataPacket.AgentBinaryData,
                new AgentBinaryData(CallbackDataProcess, data)
            );
            */
        }

        private void _ButtonSendDataAgentTcpServer_Click(object sender, EventArgs e)
        {
            if (TcpClient != null)
            {
                string text = _RichTextBoxSendDataAgentTcpServer.Text;
                TcpClient.Send(DataPacket.AgentStringData,
                    new AgentStringData(CallbackDataProcess, text)
                    {
                        IsBase64 = false,
                        BinaryData = false
                    }
                );

                _RichTextBoxSendDataAgentTcpServer.Text = "";
            }
        }

        private void _ButtonConnectAgentTcpServer_Click(object sender, EventArgs e)
        {
            if (CallbackDataProcess == null)
            {
                throw new InvalidOperationException();
            }

            TcpClient = new AgentTcpClient(_TextBoxAgentTcpServerPort.Text, CallbackDataProcess.AgentTcpServerPort);
            TcpClient.Connected += TcpClient_Connected;
            TcpClient.Disconnected += TcpClient_Disconnected;
            TcpClient.DataReceived += TcpClient_DataReceived;
            TcpClient.Connect(CallbackDataProcess);
        }
    }
}
