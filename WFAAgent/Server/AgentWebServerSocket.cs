using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.WebSocket;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using WFAAgent.Core;
using WFAAgent.Framework;
using WFAAgent.Framework.Net;
using WFAAgent.Framework.Net.Sockets;
using WFAAgent.Message;

namespace WFAAgent.Server
{
    public class AgentWebServerSocket : DefaultServerSocket
    {
        public WebSocketServer WSServer { get; private set; }
        public ListenerConfig ListenerConfig { get; private set; }
        public RootConfig RootConfig { get; private set; }
        public ServerConfig ServerConfig { get; private set; }

        public override IAgentManager AgentManager { get; set; }

        public override event MessageObjectReceivedEventHandler MessageObjectReceived
        {
            add
            {
                _MessageObjectReceived += value;
            }
            remove
            {
                _MessageObjectReceived -= value;
            }
        }
        private event MessageObjectReceivedEventHandler _MessageObjectReceived;


        public AgentWebServerSocket()
        {
            Setup();
        }

        private void CallbackMessage(string message)
        {
            // TODO: MessageItem 작업
            _MessageObjectReceived?.Invoke(message);
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
        public override void Start()
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
        
        public override void Stop()
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
            Toolkit.TraceWriteLine("SessionID.Length=" + session.SessionID.Length);
            CallbackMessage("Server.SessionCount=" + WSServer.SessionCount);
        }

        private void Server_NewMessageDataReceived(WebSocketSession session, string message)
        {
            CallbackMessage("Server_NewMessageDataReceived=" + message);

            try
            {
                JObject data = JObject.Parse(message);
                data.Add(EventConstant.SessionID, session.SessionID);
                AgentManager.OnRequestClientDataReceived(data);
            }
            catch (AgentTcpServerException ex)
            {
                session.Send(ex.Message);
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

        public override void OnProcessStarted(ProcessInfo processInfo)
        {
            JObject o = processInfo.ToStartedJson();
            string text = o.ToString();
            CallbackMessage(text);

            WebSocketSession session = WSServer.GetSessionByID(processInfo.AppId);
            if (session != null)
            {
                CallbackMessage("SessionID=" + processInfo.AppId);
                // TODO: processInfo 데이터 정의
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

        public override void OnProcessExited(ProcessInfo processInfo)
        {
            JObject o = processInfo.ToExitedJson();
            string text = o.ToString();
            CallbackMessage(text);

            WebSocketSession session = WSServer.GetSessionByID(processInfo.AppId);
            if (session != null)
            {
                CallbackMessage("SessionID=" + processInfo.AppId);
                // TODO: processInfo 데이터 정의
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
                CallbackMessage(processInfo.AppId + " 세션을 찾을 수 없습니다.");
            }
        }

        private WebSocketSession GetSessionById(string sessionId)
        {
            WebSocketSession session = null;
            IEnumerator<WebSocketSession> sessions = WSServer.GetAllSessions().GetEnumerator();
            while (sessions.MoveNext())
            {
                if (sessions.Current.SessionID.Equals(sessionId))
                {
                    session = sessions.Current;
                    break;
                }
            }
            return session;
        }

        public void SendWebClient(string sessionId, string data)
        {
            WebSocketSession session = GetSessionById(sessionId);
            if (session != null)
            {
                session.Send(data);
            }
        }

        public void BroadCastTcpServerEvent(string data)
        {
            // BroadCast 지만 실질적으로는 처음 wsExecue 를 호출한 세션만 해당함
            IEnumerator<WebSocketSession> sessions = WSServer.GetAllSessions().GetEnumerator();
            while (sessions.MoveNext())
            {
                sessions.Current.Send(data);
            }
        }

        public void SendTcpServerEvent(string data)
        {

        }

        public override void OnAcceptClientDataReceived(AcceptClientEventArgs e)
        {
            CallbackMessage("============ OnAcceptClientDataReceived");

            string data = new JObject()
                    .AddString(EventConstant.EventName, EventConstant.TcpServerAcceptClientEvent)

                    .AddInt(EventConstant.ProcessId, e.ProcessId)

                    .AddInt(EventConstant.SocketHandle, (int)e.ClientSocket.Handle)
                    .AddInt(EventConstant.Port, ((IPEndPoint)e.ClientSocket.RemoteEndPoint).Port)
                    .AddString(EventConstant.IPAddress, ((IPEndPoint)e.ClientSocket.RemoteEndPoint).Address.ToString())

                    .ToString();

            CallbackMessage(data);
            
            WebSocketSession session = WSServer.GetSessionByID(e.AppId);
            if (session != null)
            {
                if (session.TrySend(data))
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
                CallbackMessage(e.AppId + " 세션을 찾을 수 없습니다.");
            }
            CallbackMessage("OnAcceptClientDataReceived ============");
        }
        public override void OnAgentDataReceived(DataReceivedEventArgs e)
        {
            CallbackMessage("============ OnAgentDataReceived");

            if (e.Header.TransmissionData == TransmissionData.Text)
            {
                switch (e.Header.Type)
                {
                    case DataContext.AgentStringData:
                        OnAgentStringDataReceived(e);
                        break;
                    case DataContext.AgentBinaryData:
                        OnAgentBinaryDataReceived(e);
                        break;
                }
            }
            else
            {
                // Do nothing
                OnAgentBinaryDataReceived(e);
            }
            
            CallbackMessage("============ OnAgentDataReceived");
        }

        private void OnAgentStringDataReceived(DataReceivedEventArgs e)
        {
            string data = e.Data;
            CallbackMessage(data);

            JObject client = JObject.Parse(data);
            string appId = e.Header.AppId;
            WebSocketSession session = WSServer.GetSessionByID(appId);
            if (session != null)
            {
                AgentStringData value = JsonConvert.DeserializeObject<AgentStringData>(data);
                data = AgentStringData.ToStringSerializeObject(value);
                if (session.TrySend(data))
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
                CallbackMessage(appId + " 세션을 찾을 수 없습니다.");
            }
        }

        private void OnAgentBinaryDataReceived(DataReceivedEventArgs e)
        {
            byte[] data = e.RawData;
            // CallbackMessage(data);

            string appId = e.Header.AppId;
            WebSocketSession session = WSServer.GetSessionByID(appId);
            if (session != null)
            {
                if (session.TrySend(data, 0, data.Length))
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
                CallbackMessage(appId + " 세션을 찾을 수 없습니다.");
            }
        }

        public override void OnClientSendData(DataSendEventArgs e)
        {

        }
    }
}
