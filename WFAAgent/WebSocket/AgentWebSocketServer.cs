using Newtonsoft.Json.Linq;
using SuperSocket.SocketBase.Config;
using SuperSocket.WebSocket;
using System;
using WFAAgent.Core;
using WFAAgent.Message;

namespace WFAAgent.WebSocket
{
    public class AgentWebSocketServer : IDefaultSocketServer
    {
        public WebSocketServer WSServer { get; private set; }
        public ListenerConfig ListenerConfig { get; private set; }
        public RootConfig RootConfig { get; private set; }
        public ServerConfig ServerConfig { get; private set; }

        public IAgentManager AgentManager { get; set; }

        public event MessageObjectReceivedEventHandler MessageObjectReceived;

        
        public AgentWebSocketServer()
        {
            Setup();
        }

        private void CallbackMessage(string message)
        {
            MessageObjectReceived?.Invoke(message);
        }

        public void Setup()
        {
            WSServer = new WebSocketServer();

            ListenerConfig = new ListenerConfig();
            RootConfig = new RootConfig();
            ServerConfig = new ServerConfig();

            ServerConfig.Name = "WFAAgent";
            ServerConfig.Ip = "127.0.0.1";
            ServerConfig.Port = 33000;
            ServerConfig.Mode = SuperSocket.SocketBase.SocketMode.Tcp;
            ServerConfig.ListenBacklog = 1000;

            WSServer.Setup(RootConfig, ServerConfig);
        }
        public void Start()
        {
            if (WSServer == null)
            {
                Setup();
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
            WSServer.NewSessionConnected -= Server_NewSessionConnected;
            WSServer.NewMessageReceived -= Server_NewMessageDataReceived;
            WSServer.NewDataReceived -= Server_NewBinaryDataReceived;
            WSServer.SessionClosed -= Server_SessionClosed;
            WSServer.Stop();

            WSServer = null;
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
                JObject data = JObject.Parse(message);
                data.Add(WebSocketEventConstant.SessionID, session.SessionID);
                AgentManager.OnRequestClientDataReceived(data);
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

        private void Server_NewBinaryDataReceived(WebSocketSession session, byte[] binaryData)
        {
            CallbackMessage("Server_NewBinaryDataReceived=" + binaryData);
        }
        private void Server_SessionClosed(WebSocketSession session, SuperSocket.SocketBase.CloseReason value)
        {
            CallbackMessage("Server_SessionClosed=" + value);
        }

        #endregion

        public void OnProcessStarted(ProcessStartInfo processStartInfo)
        {
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
        }

        public void OnProcessExited(ProcessStartInfo processStartInfo)
        {
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
        }
    }
}
