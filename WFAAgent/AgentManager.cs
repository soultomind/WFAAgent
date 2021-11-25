using SuperSocket.SocketBase.Config;
using SuperSocket.WebSocket;
using SuperSocket.WebSocket.SubProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFAAgent.WebSocket;

namespace WFAAgent
{
    internal class AgentManager : IAgentManager
    {
        private AgentWebSocketServer server;
        public void StartServer()
        {
            if (server == null)
            {
                server = new AgentWebSocketServer();
            }
            
            server.Start();
        }

        public void StopServer()
        {
            server.Stop();
        }
    }
}
