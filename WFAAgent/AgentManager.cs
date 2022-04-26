using log4net;
using Newtonsoft.Json.Linq;
using SuperSocket.SocketBase.Config;
using SuperSocket.WebSocket;
using SuperSocket.WebSocket.SubProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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
        private static ILog Log = LogManager.GetLogger(typeof(AgentManager));

        public event MessageObjectReceivedEventHandler MessageObjectReceived;

        public ServerSocketType ServerSocketType { get; private set; }
        public IDefaultSocketServer ServerSocket { get; private set; }
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
                    ServerSocket = new AgentWebServerSocket();
                    break;
                case ServerSocketType.Tcp:
                    ServerSocket = new AgentTcpServerSocket();
                    break;
            }

            ServerSocket.AgentManager = this;
            ServerSocket.MessageObjectReceived += OnMessageObjectReceived;

            try
            {
                ServerSocket.Start();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                throw new DefaultServerSocketException("Start Failed ServerSocket", ex);
            }
            
            try
            {
                StartTcpServer();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                throw new AgentTcpServerException("Start Failed AgentTcpServer", ex);
            }
        }

        public void StopServer()
        {
            try
            {
                ServerSocket.Stop();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
            }
            
            ServerSocket.MessageObjectReceived -= OnMessageObjectReceived;
            ServerSocket.AgentManager = null;

            ServerSocket = null;

            StopTcpServer();
        }

        private void TcpServer_Listen(object sender, ListenEventArgs e)
        {
            OnMessageObjectReceived("============ TcpServer_Listen");
            if (ServerSocketType == ServerSocketType.Web)
            {
                string data = new JObject()
                    .AddString(EventConstant.EventName, EventConstant.TcpServerListenEvent)
                    .AddInt(EventConstant.SocketHandle, (int)e.ServerSocket.Handle)
                    .AddInt(EventConstant.Port, ((IPEndPoint)e.ServerSocket.LocalEndPoint).Port)
                    .AddString(EventConstant.IPAddress, ((IPEndPoint)e.ServerSocket.LocalEndPoint).Address.ToString())
                    .ToString();

                ((AgentWebServerSocket)ServerSocket).BroadCastTcpServerEvent(data);
            }
            OnMessageObjectReceived("TcpServer_Listen ============");
        }

        private void TcpServer_AcceptClient(object sender, AcceptClientEventArgs e)
        {
            OnMessageObjectReceived("============ TcpServer_AcceptClient");
            if (ServerSocketType == ServerSocketType.Web)
            {
                string data = new JObject()
                    .AddString(EventConstant.EventName, EventConstant.TcpServerAcceptClientEvent)
                    .AddInt(EventConstant.SocketHandle, (int)e.ClientSocket.Handle)
                    .AddInt(EventConstant.Port, ((IPEndPoint)e.ClientSocket.RemoteEndPoint).Port)
                    .AddString(EventConstant.IPAddress, ((IPEndPoint)e.ClientSocket.RemoteEndPoint).Address.ToString())
                    .ToString();

                ((AgentWebServerSocket)ServerSocket).BroadCastTcpServerEvent(data);
            }
            OnMessageObjectReceived("TcpServer_AcceptClient ============");
        }

        private void TcpServer_DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Exception == null)
            {
                OnMessageObjectReceived("============ TcpServer_DataReceived");
                OnMessageObjectReceived(e.ToString());
                switch (e.Header.TransmissionData)
                {
                    case TransmissionData.Text:
                        ServerSocket.OnDataReceived(e.Header.Type, e.Data);
                        break;
                    case TransmissionData.Binary:
                        ServerSocket.OnDataReceived(e.Header.Type, e.RawData);
                        break;
                }
                OnMessageObjectReceived("TcpServer_DataReceived ============");
            }
            else
            {
                OnMessageObjectReceived("TcpServer_DataReceived Error");
                OnMessageObjectReceived("Cause=" + e.Exception.Message);
                OnMessageObjectReceived(e.Exception.StackTrace);
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
            OnMessageObjectReceived("===== AgentWebSocketServer_ProcessStarted =====");

            ServerSocket.OnProcessStarted(sender as ProcessInfo);

            OnMessageObjectReceived("===== AgentWebSocketServer_ProcessStarted =====\n");
        }

        private void AgentWebSocketServer_ProcessExited(object sender, EventArgs e)
        {
            OnMessageObjectReceived("===== AgentWebSocketServer_ProcessExited =====");

            ServerSocket.OnProcessExited(sender as ProcessInfo);

            OnMessageObjectReceived("===== AgentWebSocketServer_ProcessExited =====\n");
        }

        #endregion
        
        public void OnRequestClientDataReceived(JObject messageObj)
        {
            string eventName = messageObj[EventConstant.EventName].ToObject<string>();

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

            
            JObject data = messageObj[EventConstant.Data] as JObject;

            EventData eventData = new EventData() { Data = data };
            switch (ServerSocketType)
            {
                case ServerSocketType.Web:
                    string sessionId = messageObj[EventConstant.SessionID].ToObject<string>();
                    eventData.SessionId = sessionId;
                    break;
                case ServerSocketType.Tcp:
                    eventData.SessionId = Guid.NewGuid().ToString();
                    break;
            }

            ThreadPool.QueueUserWorkItem(
                DoProcess, 
                new EventProcessState()
                {
                    EventProcessor = eventProcessor,
                    EventData = eventData
                }
            );
        }

        private void DoProcess(object state)
        {
            EventProcessState eventProcessState = state as EventProcessState;
            eventProcessState.EventProcessor.DoProcess(eventProcessState.EventData);
        }
    }
}
