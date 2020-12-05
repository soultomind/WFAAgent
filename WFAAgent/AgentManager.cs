using SuperSocket.SocketBase.Config;
using SuperSocket.WebSocket;
using SuperSocket.WebSocket.SubProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent
{
    internal class AgentManager : IAgentManager
    {
        private WebSocketServer webSocketServer;
        public void StartServer()
        {
            if (webSocketServer == null)
            {
                var serverConfig = new ServerConfig();
                
                webSocketServer = new WebSocketServer();
            }
            
            webSocketServer.Start();
        }

        public void StopServer()
        {
            webSocketServer.Stop();
        }
    }
}
