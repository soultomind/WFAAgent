using Newtonsoft.Json.Linq;
using SuperSocket.SocketBase.Config;
using SuperSocket.WebSocket;
using System;
using System.Diagnostics;
using WFAAgent.Core;
using WFAAgent.Framework.Net.Sockets;
using WFAAgent.Message;

namespace WFAAgent.WebSocket
{
    public class AgentWebSocketServer
    {
        public WebSocketServer WSServer { get; private set; }
        public ListenerConfig ListenerConfig { get; private set; }
        public RootConfig RootConfig { get; private set; }
        public ServerConfig ServerConfig { get; private set; }

        public AgentTcpServer TCPServer { get; private set; }

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
            if (WSServer == null)
            {
                ListenerConfig = new ListenerConfig();

                RootConfig = new RootConfig();
                ServerConfig = new ServerConfig();
                ServerConfig.Name = "WFAAgent";
                ServerConfig.Ip = "127.0.0.1";
                ServerConfig.Port = 33000;
                ServerConfig.Mode = SuperSocket.SocketBase.SocketMode.Tcp;
                ServerConfig.ListenBacklog = 1000;
                WSServer = new WebSocketServer();
                WSServer.Setup(RootConfig, ServerConfig);
            }

            WSServer.NewSessionConnected += Server_NewSessionConnected;
            WSServer.NewMessageReceived += Server_NewMessageDataReceived;
            WSServer.NewDataReceived += Server_NewBinaryDataReceived;
            WSServer.SessionClosed += Server_SessionClosed;
            WSServer.Start();

            CallbackMessage("========== Server Start ==========");
            CallbackMessage("ServerInfo");
            CallbackMessage(WSServer.ToInfoString());
            CallbackMessage("========== Server Start ==========");
        }
        
        public void Stop()
        {
            if (WSServer != null)
            {
                WSServer.NewSessionConnected -= Server_NewSessionConnected;
                WSServer.NewMessageReceived -= Server_NewMessageDataReceived;
                WSServer.NewDataReceived -= Server_NewBinaryDataReceived;
                WSServer.SessionClosed -= Server_SessionClosed;
                WSServer.Stop();
            }
        }

        #region Server Event

        private void Server_NewSessionConnected(WebSocketSession session)
        {
            CallbackMessage("Server_NewSessionConnected=" + session.SessionID);
            CallbackMessage("Server.SessionCount=" + WSServer.SessionCount);
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
                    eventProcessor = EventProcessorManager.AddStartsWithByEventName(eventName);
                    if (eventProcessor is ProcessStartEventProcessor)
                    {
                        ((ProcessStartEventProcessor)eventProcessor).Started += AgentWebSocketServer_ProcessStarted;
                        ((ProcessStartEventProcessor)eventProcessor).Exited += AgentWebSocketServer_ProcessExited;
                    }
                }
                else
                {
                    eventProcessor = EventProcessorManager.EventProcessors[eventName];
                }
                
                // TODO: ThreadPool 사용필요
                JObject data = messageObj[WebSocketEventConstant.Data] as JObject;
                eventProcessor.DoProcess(new EventData() { Data = data, SessionID = session.SessionID });
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

        private void AgentWebSocketServer_ProcessStarted(object sender, EventArgs e)
        {
            WFAAgent.Core.ProcessStartInfo processStartInfo = sender as WFAAgent.Core.ProcessStartInfo;
            CallbackMessage("===== AgentWebSocketServer_ProcessStarted =====");
 
            if (TCPServer == null)
            {
                TCPServer = new AgentTcpServer();
            }

            JObject o = processStartInfo.ToStartedJson();
            string text = o.ToString();
            CallbackMessage(text);

            WebSocketSession session = WSServer.GetSessionByID(processStartInfo.SessionID);   
            if (session != null)
            {
                CallbackMessage("SessionID=" + processStartInfo.SessionID);
                // TODO: ProcessStartInfo 데이터 정의
                if (session.TrySend(text))
                {
                    CallbackMessage("전송성공");
                }
                else
                {
                    CallbackMessage("전송실패");
                }
            }
            else
            {
                CallbackMessage("세션을 찾을 수 없습니다.");
            }

            CallbackMessage("===== AgentWebSocketServer_ProcessStarted =====\n");
        }

        private void AgentWebSocketServer_ProcessExited(object sender, EventArgs e)
        {
            WFAAgent.Core.ProcessStartInfo processStartInfo = sender as WFAAgent.Core.ProcessStartInfo;
            CallbackMessage("===== AgentWebSocketServer_ProcessExited =====");

            JObject o = processStartInfo.ToExitedJson();
            string text = o.ToString();
            CallbackMessage(text);

            WebSocketSession session = WSServer.GetSessionByID(processStartInfo.SessionID);
            if (session != null)
            {
                CallbackMessage("SessionID=" + processStartInfo.SessionID);
                // TODO: ProcessStartInfo 데이터 정의
                if (session.TrySend(text))
                {
                    CallbackMessage("전송성공");
                }
                else
                {
                    CallbackMessage("전송실패");
                }
            }
            else
            {
                CallbackMessage("세션을 찾을 수 없습니다.");
            }

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
