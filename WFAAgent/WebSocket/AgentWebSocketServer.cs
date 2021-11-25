using SuperSocket.SocketBase.Config;
using SuperSocket.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.WebSocket
{
    public class AgentWebSocketServer
    {
        private WebSocketServer webSocketServer;
        public void Start()
        {
            if (webSocketServer == null)
            {
                ListenerConfig listenerConfig = new ListenerConfig();
                
                ServerConfig serverConfig = new ServerConfig();
                
                webSocketServer = new WebSocketServer();
                webSocketServer.Setup(serverConfig);
            }
            webSocketServer.Start();
        }

        public void Stop()
        {
            webSocketServer.Stop();
        }
    }
}
