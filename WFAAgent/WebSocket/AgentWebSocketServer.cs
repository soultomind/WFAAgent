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
        public WebSocketServer Server { get; private set; }
        public ListenerConfig ListenerConfig { get; private set; }
        public RootConfig RootConfig { get; private set; }
        public ServerConfig ServerConfig { get; private set; }

        public event MessageObjectReceivedEventHandler MessageObjectReceived;


        public AgentWebSocketServer()
        {

        }
        public void Start()
        {
            if (Server == null)
            {
                ListenerConfig = new ListenerConfig();

                RootConfig = new RootConfig();
                ServerConfig = new ServerConfig();
                ServerConfig.Name = "WFAAgent";
                ServerConfig.Ip = "127.0.0.1";
                ServerConfig.Port = 33000;
                ServerConfig.Mode = SuperSocket.SocketBase.SocketMode.Tcp;
                Server = new WebSocketServer();
                Server.Setup(RootConfig, ServerConfig);
            }

            Server.NewSessionConnected += Server_NewSessionConnected;
            Server.NewMessageReceived += Server_NewMessageReceived;
            Server.NewDataReceived += Server_NewDataReceived;
            Server.SessionClosed += Server_SessionClosed;
            Server.Start();
        }
        
        public void Stop()
        {
            if (Server != null)
            {
                Server.Stop();
            }
        }

        #region Server Event

        private void Server_NewSessionConnected(WebSocketSession session)
        {
            MessageObjectReceived?.Invoke("Server_NewSessionConnected=" + session.SessionID);
        }

        private void Server_NewMessageReceived(WebSocketSession session, string value)
        {
            MessageObjectReceived?.Invoke("Server_NewMessageReceived=" + value);
        }
        private void Server_NewDataReceived(WebSocketSession session, byte[] value)
        {
            MessageObjectReceived?.Invoke("Server_NewMessageReceived=" + value);
        }
        private void Server_SessionClosed(WebSocketSession session, SuperSocket.SocketBase.CloseReason value)
        {
            MessageObjectReceived?.Invoke("Server_SessionClosed=" + value);
        }

        #endregion
    }
}
