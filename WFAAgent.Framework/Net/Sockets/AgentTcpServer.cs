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
        public event AcceptClientEventHandler AcceptClient;

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

        public void Start()
        {
            ServerSocket.Bind();
            ServerSocket.Listen();

            Listen?.Invoke(this, new ListenEventArgs(ServerSocket.Socket));
        }

        public void Start(int backlog)
        {
            ServerSocket.Bind();
            ServerSocket.Listen(backlog);

            Listen?.Invoke(this, new ListenEventArgs(ServerSocket.Socket));

            ServerSocket.Start();
        }

        public void Stop()
        {
            ServerSocket.Stop();
        }
    }
}
