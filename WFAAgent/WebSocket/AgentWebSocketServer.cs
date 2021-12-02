using Newtonsoft.Json.Linq;
using SuperSocket.SocketBase.Config;
using SuperSocket.WebSocket;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFAAgent.Core;
using WFAAgent.Message;

namespace WFAAgent.WebSocket
{
    public class AgentWebSocketServer
    {
        public WebSocketServer Server { get; private set; }
        public ListenerConfig ListenerConfig { get; private set; }
        public RootConfig RootConfig { get; private set; }
        public ServerConfig ServerConfig { get; private set; }

        public EventProcessorManager EventProcessorManager { get; private set; }
        public event MessageObjectReceivedEventHandler MessageObjectReceived;

        
        public AgentWebSocketServer()
        {
            EventProcessorManager = new EventProcessorManager();
        }

        private void CallbackMessage(string message)
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

            CallbackMessage("Server Start");
            CallbackMessage("ServerInfo=" + Server.ToString());
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
            CallbackMessage("Server_NewSessionConnected=" + session.SessionID);
        }

        private void Server_NewMessageDataReceived(WebSocketSession session, string message)
        {
            CallbackMessage("Server_NewMessageDataReceived=" + message);

            try
            {
                JObject messageObj = JObject.Parse(message);
                string eventName = messageObj[WebSocketEventConstant.EventName].ToObject<string>();

                IEventProcessor eventProcessor = null;
                if (!EventProcessorManager.EventProcessors.ContainsKey(eventName))
                {
                    eventProcessor = EventProcessorManager.AddStartsWithByEventNameEventProcessor(eventName);
                    if (eventProcessor is ProcessStartEventProcessor)
                    {
                        ((ProcessStartEventProcessor)eventProcessor).Exited += AgentWebSocketServer_ProcessExited;
                    }
                }
                else
                {
                    eventProcessor = EventProcessorManager.EventProcessors[eventName];
                }

                JObject data = messageObj[WebSocketEventConstant.Data] as JObject;
                eventProcessor.DoProcess(new EventData() { Data = data });
            }
            catch (Newtonsoft.Json.JsonException ex)
            {
                session.Send("It's unknown data. Cause=" + ex.Message);
            }
            catch (Exception ex)
            {
                session.Send(ex.Message);
            }
        }

        private void AgentWebSocketServer_ProcessExited(object sender, EventArgs e)
        {
            Process process = sender as Process;
            CallbackMessage("\n===== AgentWebSocketServer_ProcessExited =====");
            CallbackMessage("FileName=" + process.StartInfo.FileName);
            CallbackMessage("ExitTime=" + process.ExitTime);
            CallbackMessage("===== AgentWebSocketServer_ProcessExited =====\n");
        }

        private void Server_NewBinaryDataReceived(WebSocketSession session, byte[] binaryData)
        {
            CallbackMessage("Server_NewBinaryDataReceived=" + binaryData);
        }
        private void Server_SessionClosed(WebSocketSession session, SuperSocket.SocketBase.CloseReason value)
        {
            CallbackMessage("Server_SessionClosed=" + value);
        }

        #endregion
    }
}
