using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework.Net.Sockets
{
    public class AgentTcpServer
    {
        public ServerSocket ServerSocket { get; private set; }

        public event ListenEventHandler Listen;

        public event DataReceivedEventhandler ClientDataReceived;
        public event AcceptClientEventHandler AcceptClient;
        public event DisconnectedEventHandler DisconnectedClient;
        

        public AgentTcpServer()
            : this("127.0.0.1")
        {

        }
        public AgentTcpServer(string ipString, int port = 33010)
        {
            ServerSocket = new ServerSocket(ipString, port);
        }

        public string IPString
        {
            get { return ServerSocket.IPString; }
        }

        public int Port
        {
            get { return ServerSocket.Port; }
        }

        private void ServerSocket_AcceptClient(object sender, AcceptClientEventArgs e)
        {
            AcceptClient?.Invoke(sender, e);
        }
        private void ServerSocket_ClientDataReceived(object sender, DataReceivedEventArgs e)
        {
            ClientDataReceived?.Invoke(sender, e);
        }

        private void ServerSocket_DisconnectedClient(object sender, DisconnectEventArgs e)
        {
            DisconnectedClient?.Invoke(sender, e);
        }

        public void Start()
        {
            ServerSocket.Initialize();
            ServerSocket.Socket.NoDelay = true;
            if (ServerSocket.Bind())
            {
                ServerSocket.AcceptClient += ServerSocket_AcceptClient;
                ServerSocket.ClientDataReceived += ServerSocket_ClientDataReceived;
                ServerSocket.DisconnectedClient += ServerSocket_DisconnectedClient;
                ServerSocket.Listen();
                Listen?.Invoke(this, new ListenEventArgs(ServerSocket.Socket));
                ServerSocket.Start();
            }
        }

        public void Start(int backlog)
        {
            ServerSocket.Initialize();
            ServerSocket.Socket.NoDelay = true;
            if (ServerSocket.Bind())
            {
                ServerSocket.AcceptClient += ServerSocket_AcceptClient;
                ServerSocket.ClientDataReceived += ServerSocket_ClientDataReceived;
                ServerSocket.DisconnectedClient += ServerSocket_DisconnectedClient;
                ServerSocket.Listen(backlog);
                Listen?.Invoke(this, new ListenEventArgs(ServerSocket.Socket));
                ServerSocket.Start();
            }
        }

        public void Stop()
        {
            ServerSocket.Stop();
        }
    }
}
