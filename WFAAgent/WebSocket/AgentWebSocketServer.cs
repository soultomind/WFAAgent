using Newtonsoft.Json.Linq;
using SuperSocket.SocketBase.Config;
using SuperSocket.WebSocket;
using System;
using System.Collections.Generic;
using System.Configuration;
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

        private void CallbakMessage(string message)
        {
            MessageObjectReceived?.Invoke(message);
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
                ServerConfig.ListenBacklog = 1000;
                Server = new WebSocketServer();
                Server.Setup(RootConfig, ServerConfig);
            }

            Server.NewSessionConnected += Server_NewSessionConnected;
            Server.NewMessageReceived += Server_NewMessageDataReceived;
            Server.NewDataReceived += Server_NewBinaryDataReceived;
            Server.SessionClosed += Server_SessionClosed;
            Server.Start();

            CallbakMessage("Server Start");
            CallbakMessage("ServerInfo=" + Server.ToString());
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
            CallbakMessage("Server_NewSessionConnected=" + session.SessionID);
        }

        private void Server_NewMessageDataReceived(WebSocketSession session, string message)
        {
            CallbakMessage("Server_NewMessageDataReceived=" + message);

            try
            {
                JObject messageObj = JObject.Parse(message);
                string eventProcessorName = messageObj[WebSocketEvent.EventName].ToObject<string>();
            }
            catch (Newtonsoft.Json.JsonException ex)
            {
                session.Send("It's unknown data.");
                return;
            }
            catch (Exception ex)
            {
                session.Send(ex.Message);
            }
        }
        private void Server_NewBinaryDataReceived(WebSocketSession session, byte[] binaryData)
        {
            CallbakMessage("Server_NewBinaryDataReceived=" + binaryData);
        }
        private void Server_SessionClosed(WebSocketSession session, SuperSocket.SocketBase.CloseReason value)
        {
            CallbakMessage("Server_SessionClosed=" + value);
        }

        #endregion
    }
}
