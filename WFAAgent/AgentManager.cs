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
            : this(ServerSocketType.WebSocket)
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
                case ServerSocketType.WebSocket:
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
                }
            }
            else
            {
                eventProcessor = EventProcessorManager.EventProcessors[eventName];
            }

            // TODO: ThreadPool 사용필요
            JObject data = messageObj[WebSocketEventConstant.Data] as JObject;

            EventData eventData = new EventData() { Data = data };
            if (ServerSocketType == ServerSocketType.WebSocket)
            {
                string sessionId = messageObj[WebSocketEventConstant.SessionID].ToObject<string>();
                eventData.SessionID = sessionId;
            }
            eventProcessor.DoProcess(eventData);
        }
    }
}
