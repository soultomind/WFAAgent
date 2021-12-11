using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFAAgent.Framework.Application;

namespace WFAAgent.Framework.Net.Sockets
{
    public class AgentTcpClient
    {
        public ProcessStartArguments ProcessStartArguments { get; set; }
        public ClientSocket ClientSocket { get; private set; }

        private bool _IsAcceptClientForSendServer;

        public event ConnectedEventhandler Connected;
        public event DisconnectedEventHandler Disconnected;
        public event DataReceivedEventhandler DataReceived;

        public AgentTcpClient()
            : this("127.0.0.1")
        {

        }

        public AgentTcpClient(string ipString, int port = 33010)
        {
            ClientSocket = new ClientSocket(ipString, port);
            ClientSocket.Connected += ClientSocket_Connected;
            ClientSocket.Disconnected += ClientSocket_Disconnected;
            ClientSocket.ServerDataReceived += ClientSocket_DataReceived;
        }

        private void ClientSocket_Connected(object sender, ConnectedEventArgs e)
        {
            Connected?.Invoke(sender, e);
        }

        private void ClientSocket_Disconnected(object sender, DisconnectEventArgs e)
        {
            Disconnected?.Invoke(sender, e);
        }

        private void ClientSocket_DataReceived(object sender, DataReceivedEventArgs e)
        {
            DataReceived?.Invoke(sender, e);
        }

        public bool Connect()
        {
            if (ClientSocket.Connect())
            {
                if (!_IsAcceptClientForSendServer)
                {
                    string data = new JObject()
                        .AddInt(Constant.ProcessId, Process.GetCurrentProcess().Id)
                        .AddString(Constant.ArgAppID, ProcessStartArguments.AppId)
                        .ToString();
                    int sendBytes = Send(DataPacket.AcceptClient, data);
                    _IsAcceptClientForSendServer = true;
                }
            }
            return false;
        }

        public int Send(DataPacket dataPacket, string data)
        {
            return ClientSocket.Send(dataPacket, data);
        }
    }
}
