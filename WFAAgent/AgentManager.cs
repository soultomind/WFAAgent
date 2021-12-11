using Newtonsoft.Json.Linq;
using SuperSocket.SocketBase.Config;
using SuperSocket.WebSocket;
using SuperSocket.WebSocket.SubProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WFAAgent.Core;
using WFAAgent.Framework;
using WFAAgent.Framework.Net.Sockets;
using WFAAgent.Message;
using WFAAgent.Tcp;
using WFAAgent.WebSocket;

namespace WFAAgent
{
    internal class AgentManager : IAgentManager
    {
        public event MessageObjectReceivedEventHandler MessageObjectReceived;

        public ServerSocketType ServerSocketType { get; private set; }
        public IDefaultSocketServer SocketServer { get; private set; }
        public EventProcessorManager EventProcessorManager { get; private set; }

        public AgentTcpServer TcpServer { get; private set; }

        public AgentManager()
            : this(ServerSocketType.Web)
        {

        }
        public AgentManager(ServerSocketType serverSocketType)
        {
            ServerSocketType = serverSocketType;

            EventProcessorManager = new EventProcessorManager();
        }

        internal void OnMessageObjectReceived(object messageObject)
        {
            MessageObjectReceived?.Invoke(messageObject);
        }

        public void StartServer()
        {
            switch (ServerSocketType)
            {
                case ServerSocketType.Web:
                    SocketServer = new AgentWebSocketServer();
                    break;
                case ServerSocketType.Tcp:
                    SocketServer = new AgentTcpSocketServer();
                    break;
            }

            SocketServer.AgentManager = this;
            SocketServer.MessageObjectReceived += OnMessageObjectReceived;
            SocketServer.Start();
        }

        public void StopServer()
        {
            SocketServer.Stop();
            SocketServer.MessageObjectReceived -= OnMessageObjectReceived;
            SocketServer = null;

            StopTcpServer();
        }

        private void TcpServer_Listen(object sender, ListenEventArgs e)
        {
            if (ServerSocketType == ServerSocketType.Web)
            {
                string data = new JObject()
                    .AddString(EventConstant.EventName, EventConstant.TcpServerListenEvent)
                    .AddInt(EventConstant.SocketHandle, (int)e.ServerSocket.Handle)
                    .AddInt(EventConstant.Port, ((IPEndPoint)e.ServerSocket.LocalEndPoint).Port)
                    .AddString(EventConstant.IPAddress, ((IPEndPoint)e.ServerSocket.LocalEndPoint).Address.ToString())
                    .ToString();

                ((AgentWebSocketServer)SocketServer).BroadCastTcpServerEvent(data);
            }
        }

        private void TcpServer_AcceptClient(object sender, AcceptClientEventArgs e)
        {
            if (ServerSocketType == ServerSocketType.Web)
            {
                string data = new JObject()
                    .AddString(EventConstant.EventName, EventConstant.TcpServerAcceptClientEvent)
                    .AddInt(EventConstant.SocketHandle, (int)e.ServerSocket.Handle)
                    .ToString();

                // TODO: 실행을 요청한 세션에 데이터 보내야 함
                // ((AgentWebSocketServer)SocketServer).BroadCastTcpServerEvent(data);
            }
        }

        private void TcpServer_DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Exception == null)
            {
                OnMessageObjectReceived("============ TcpServer_DataReceived");
                OnMessageObjectReceived(e.ToString());
                if (ServerSocketType == ServerSocketType.Web)
                {
                    // TODO: 해당 데이터 JSON Serialize 로 객체를 만들어서 처리하자
                    switch (e.Header.Type)
                    {
                        case DataContext.AcceptClient:
                            
                            break;
                        case DataContext.UserData:
                            break;
                    }
                }
                OnMessageObjectReceived("TcpServer_DataReceived ============");
            }
        }

        private void StartTcpServer()
        {
            if (TcpServer == null)
            {
                TcpServer = new AgentTcpServer();
                TcpServer.Listen += new ListenEventHandler(TcpServer_Listen);
                TcpServer.AcceptClient += new AcceptClientEventHandler(TcpServer_AcceptClient);
                TcpServer.DataReceived += new WFAAgent.Framework.Net.Sockets.DataReceivedEventhandler(TcpServer_DataReceived);
                TcpServer.Start();
            }
        }

        private void StopTcpServer()
        {
            if (TcpServer != null)
            {
                TcpServer.Listen -= new ListenEventHandler(TcpServer_Listen);
                TcpServer.AcceptClient -= new AcceptClientEventHandler(TcpServer_AcceptClient);
                TcpServer.DataReceived -= new WFAAgent.Framework.Net.Sockets.DataReceivedEventhandler(TcpServer_DataReceived);
                TcpServer.Stop();
                TcpServer = null;
            }
        }

        #region Process Event
        private void AgentWebSocketServer_ProcessStarted(object sender, EventArgs e)
        {
            WFAAgent.Core.ProcessInfo processStartInfo = sender as WFAAgent.Core.ProcessInfo;
            OnMessageObjectReceived("===== AgentWebSocketServer_ProcessStarted =====");

            SocketServer.OnProcessStarted(processStartInfo);

            OnMessageObjectReceived("===== AgentWebSocketServer_ProcessStarted =====\n");
        }

        private void AgentWebSocketServer_ProcessExited(object sender, EventArgs e)
        {
            WFAAgent.Core.ProcessInfo processStartInfo = sender as WFAAgent.Core.ProcessInfo;
            OnMessageObjectReceived("===== AgentWebSocketServer_ProcessExited =====");

            SocketServer.OnProcessExited(processStartInfo);

            OnMessageObjectReceived("===== AgentWebSocketServer_ProcessExited =====\n");
        }

        #endregion
        
        public void OnRequestClientDataReceived(JObject messageObj)
        {
            string eventName = messageObj[EventConstant.EventName].ToObject<string>();

            IEventProcessor eventProcessor = null;
            if (!EventProcessorManager.EventProcessors.ContainsKey(eventName))
            {
                // 최초의 프로세스 실행 요청일 경우 TCPServer를 시작한다.
                try
                {
                    StartTcpServer();
                }
                catch (Exception ex)
                {
                    throw new AgentTcpServerException(ex.Message, ex);
                }
                

                eventProcessor = EventProcessorManager.AddStartsWithByEventName(eventName);
                if (eventProcessor is ProcessStartEventProcessor)
                {
                    ((ProcessStartEventProcessor)eventProcessor).Started += AgentWebSocketServer_ProcessStarted;
                    ((ProcessStartEventProcessor)eventProcessor).Exited += AgentWebSocketServer_ProcessExited;
                    ((ProcessStartEventProcessor)eventProcessor).AgentTcpServerPort = TcpServer.Port;
                }
            }
            else
            {
                eventProcessor = EventProcessorManager.EventProcessors[eventName];
            }

            // TODO: ThreadPool 사용필요
            JObject data = messageObj[EventConstant.Data] as JObject;

            EventData eventData = new EventData() { Data = data };
            if (ServerSocketType == ServerSocketType.Web)
            {
                string sessionId = messageObj[EventConstant.SessionID].ToObject<string>();
                eventData.SessionID = sessionId;
            }
            eventProcessor.DoProcess(eventData);
        }
    }
}
