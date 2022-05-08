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
        public CallbackDataProcess CallbackDataProcess { get; set; }
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
                if (CallbackDataProcess == null)
                {
                    throw new InvalidOperationException(nameof(CallbackDataProcess));
                }

                if (!_IsAcceptClientForSendServer)
                {
                    int sendBytes = Send(DataPacket.AcceptClient, new AcceptClient(CallbackDataProcess));
                    _IsAcceptClientForSendServer = true;
                }
                return true;
            }
            return false;
        }

        public bool Connect(CallbackDataProcess callbackDataProcess)
        {
            this.CallbackDataProcess = callbackDataProcess;
            return Connect();
        }

        public void Disconnect()
        {
            if (ClientSocket.CanUseSocket)
            {
                ClientSocket.Disconnect();
            }
        }

        public int Send(DataPacket dataPacket, string data)
        {
            return ClientSocket.Send(dataPacket, data);
        }

        public int Send(DataPacket dataPacket, byte[] data)
        {
            return ClientSocket.Send(dataPacket, data);
        }

        public int Send(DataPacket dataPacket, DefaultData data)
        {
            return Send(dataPacket, data.ToJson().ToString());
        }
    }
}
