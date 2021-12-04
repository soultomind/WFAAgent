using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework.Net.Sockets
{
    public class AgentTcpServer
    {
        public int Port { get; private set; }
        public ServerSocket ServerSocket { get; private set; }

        public event ListenEventHandler Listen;
        public event AcceptClientEventHandler AcceptClient;

        public AgentTcpServer(int port = 33010)
        {
            Port = port;
        }

        public void Start()
        {
            ServerSocket = new ServerSocket();
        }

        public void Stop()
        {

        }
    }
}
