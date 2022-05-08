using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using WFAAgent.Core;
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

            WebSocketSession session = WSServer.GetSessionByID(processInfo.SessionId);
            if (session != null)
            {
                CallbackMessage("SessionID=" + processInfo.SessionId);
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

            WebSocketSession session = WSServer.GetSessionByID(processInfo.SessionId);
            if (session != null)
            {
                CallbackMessage("SessionID=" + processInfo.SessionId);
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
                CallbackMessage(processInfo.SessionId + " 세션을 찾을 수 없습니다.");
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

        public override void OnAcceptClientDataReceived(ushort type, string data)
        {
            CallbackMessage("============ OnAcceptClientDataReceived");
            CallbackMessage(data);

            // JsonConvert.DeserializeObject 활용을 위해서는
            // 모든 프로퍼티 get, set 접근지정자 public 필요
            AcceptClient value = JsonConvert.DeserializeObject<AcceptClient>(data);
            WebSocketSession session = WSServer.GetSessionByID(value.AppId);
            if (session != null)
            {
                data = AcceptClient.ToStringSerializeObject(value);
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
                CallbackMessage(value.AppId + " 세션을 찾을 수 없습니다.");
            }
            CallbackMessage("OnAcceptClientDataReceived ============");
        }
        public override void OnAgentDataReceived(ushort type, string data)
        {
            CallbackMessage("============ OnAgentDataReceived");
            CallbackMessage(data);

            AgentData value = JsonConvert.DeserializeObject<AgentData>(data);
            WebSocketSession session = WSServer.GetSessionByID(value.AppId);
            if (session != null)
            {
                data = AgentData.ToStringSerializeObject(value);
                switch (type)
                {
                    case DataContext.AgentStringData:
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
                        break;
                    case DataContext.UnknownData:
                        throw new InvalidOperationException(nameof(DataContext.UnknownData));
                }

            }
            else
            {
                CallbackMessage(value.AppId + " 세션을 찾을 수 없습니다.");
            }
            CallbackMessage("============ OnAgentDataReceived");
        }
    }
}
