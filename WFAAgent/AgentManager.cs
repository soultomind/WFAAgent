using Newtonsoft.Json.Linq;
using SuperSocket.SocketBase.Config;
using SuperSocket.WebSocket;
using SuperSocket.WebSocket.SubProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFAAgent.Core;
using WFAAgent.Framework.Net.Sockets;
using WFAAgent.Message;
using WFAAgent.Tcp;
using WFAAgent.WebSocket;

namespace WFAAgent
{
    internal class AgentManager : IAgentManager
    {
        public ServerSocketType ServerSocketType { get; internal set; }
        public EventProcessorManager EventProcessorManager { get; private set; }

        public IDefaultSocketServer SocketServer { get; private set; }
        public AgentTcpServer TcpServer { get; private set; }

        public event MessageObjectReceivedEventHandler MessageObjectReceived;

        public AgentManager()
            : this(ServerSocketType.Web)
        {

        }
        public AgentManager(ServerSocketType serverSocketType)
        {
            ServerSocketType = serverSocketType;

            EventProcessorManager = new EventProcessorManager();
        }

        #region Process Event
        private void AgentWebSocketServer_ProcessStarted(object sender, EventArgs e)
        {
            WFAAgent.Core.ProcessStartInfo processStartInfo = sender as WFAAgent.Core.ProcessStartInfo;
            OnMessageObjectReceived("===== AgentWebSocketServer_ProcessStarted =====");

            SocketServer.OnProcessStarted(processStartInfo);

            OnMessageObjectReceived("===== AgentWebSocketServer_ProcessStarted =====\n");
        }

        private void AgentWebSocketServer_ProcessExited(object sender, EventArgs e)
        {
            WFAAgent.Core.ProcessStartInfo processStartInfo = sender as WFAAgent.Core.ProcessStartInfo;
            OnMessageObjectReceived("===== AgentWebSocketServer_ProcessExited =====");

            SocketServer.OnProcessExited(processStartInfo);

            OnMessageObjectReceived("===== AgentWebSocketServer_ProcessExited =====\n");
        }

        #endregion
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

            if (TcpServer == null)
            {
                TcpServer = new AgentTcpServer();
                TcpServer.Listen += new ListenEventHandler(TcpServer_Listen);
                TcpServer.AcceptClient += new AcceptClientEventHandler(TcpServer_AcceptClient);
                TcpServer.Start();
            }
            
        }

        public void StopServer()
        {
            SocketServer.Stop();
            SocketServer.MessageObjectReceived -= OnMessageObjectReceived;
            SocketServer = null;

            TcpServer.Stop();
            TcpServer.Listen -= new ListenEventHandler(TcpServer_Listen);
            TcpServer.AcceptClient -= new AcceptClientEventHandler(TcpServer_AcceptClient);
            TcpServer = null;
        }

        private void TcpServer_Listen(object sender, ListenEventArgs e)
        {
            
        }

        private void TcpServer_AcceptClient(object sender, AcceptClientEventArgs e)
        {
            
        }

        public void OnRequestClientDataReceived(JObject messageObj)
        {
            string eventName = messageObj[WebSocketEventConstant.EventName].ToObject<string>();

            IEventProcessor eventProcessor = null;
            if (!EventProcessorManager.EventProcessors.ContainsKey(eventName))
            {
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
            JObject data = messageObj[WebSocketEventConstant.Data] as JObject;

            EventData eventData = new EventData() { Data = data };
            if (ServerSocketType == ServerSocketType.Web)
            {
                string sessionId = messageObj[WebSocketEventConstant.SessionID].ToObject<string>();
                eventData.SessionID = sessionId;
            }
            eventProcessor.DoProcess(eventData);
        }
    }
}
