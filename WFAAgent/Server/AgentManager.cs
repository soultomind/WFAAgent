using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using WFAAgent.Core;
using WFAAgent.Framework;
using WFAAgent.Framework.Net;
using WFAAgent.Framework.Net.Sockets;
using WFAAgent.Message;

namespace WFAAgent.Server
{
    internal class AgentManager : IAgentManager
    {
        private static ILog Log = LogManager.GetLogger(typeof(AgentManager));

        public event MessageObjectReceivedEventHandler MessageObjectReceived;

        public AgentServerSocket AgentServerSocket { get; private set; }
        public IDefaultSocketServer DefaultServerSocket { get; private set; }
        public EventProcessorManager EventProcessorManager { get; private set; }

        public AgentTcpServer TcpServer { get; private set; }

        private ManualResetEvent SingleProcessEvent = 
            new ManualResetEvent(false);

        private ConcurrentDictionary<string, ManualResetEvent> MultiProcessEvent = 
            new ConcurrentDictionary<string, ManualResetEvent>();

        public AgentManager()
            : this(AgentServerSocket.Web)
        {

        }
        public AgentManager(AgentServerSocket agentServerSocket)
        {
            AgentServerSocket = agentServerSocket;

            EventProcessorManager = new EventProcessorManager();
        }

        internal void OnMessageObjectReceived(object messageObject)
        {
            // TODO: MessageItem 작업
            MessageObjectReceived?.Invoke(messageObject);
        }

        internal void OnDebugMessageItem(string message)
        {
            MessageItem messageItem = new MessageItem() { LogLevel = LogLevel.Debug, Message = message };
            OnMessageItem(messageItem);
        }

        internal void OnInfoMessageItem(string message)
        {
            MessageItem messageItem = new MessageItem() { LogLevel = LogLevel.Info, Message = message };
            OnMessageItem(messageItem);
        }

        internal void OnWarnMessageItem(string message)
        {
            MessageItem messageItem = new MessageItem() { LogLevel = LogLevel.Warn, Message = message };
            OnMessageItem(messageItem);
        }

        internal void OnErrorMessageItem(string message)
        {
            MessageItem messageItem = new MessageItem() { LogLevel = LogLevel.Error, Message = message };
            OnMessageItem(messageItem);
        }

        private void OnMessageItem(MessageItem messageItem)
        {
            MessageObjectReceived?.Invoke(messageItem);
        }

        public void StartServer()
        {
            switch (AgentServerSocket)
            {
                case AgentServerSocket.Web:
                    DefaultServerSocket = new AgentWebServerSocket();
                    break;
                case AgentServerSocket.Tcp:
                    DefaultServerSocket = new AgentTcpServerSocket();
                    break;
            }

            DefaultServerSocket.AgentManager = this;
            DefaultServerSocket.MessageObjectReceived += OnMessageObjectReceived;

            try
            {
                DefaultServerSocket.Start();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                throw new DefaultServerSocketException("Start Failed ServerSocket", ex);
            }
            
            try
            {
                // StartTcpServer();
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
                DefaultServerSocket.Stop();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
            }

            DefaultServerSocket.MessageObjectReceived -= OnMessageObjectReceived;
            DefaultServerSocket.AgentManager = null;

            DefaultServerSocket = null;

            try
            {
                StopTcpServer();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                throw new AgentTcpServerException("Stop Failed AgentTcpServer", ex);
            }
        }

        private void TcpServer_Listen(object sender, ListenEventArgs e)
        {
            OnMessageObjectReceived("============ TcpServer_Listen");
            if (AgentServerSocket == AgentServerSocket.Web)
            {
                string data = new JObject()
                    .AddString(EventConstant.EventName, EventConstant.TcpServerListenEvent)
                    .AddInt(EventConstant.SocketHandle, (int)e.ServerSocket.Handle)
                    .AddInt(EventConstant.Port, ((IPEndPoint)e.ServerSocket.LocalEndPoint).Port)
                    .AddString(EventConstant.IPAddress, ((IPEndPoint)e.ServerSocket.LocalEndPoint).Address.ToString())
                    .ToString();

                ((AgentWebServerSocket)DefaultServerSocket).BroadCastTcpServerEvent(data);
            }
            OnMessageObjectReceived("TcpServer_Listen ============");
        }

        private void TcpServer_AcceptClient(object sender, AcceptClientEventArgs e)
        {
            OnMessageObjectReceived("============ TcpServer_AcceptClient");
            if (AgentServerSocket == AgentServerSocket.Web)
            {
                DefaultServerSocket.OnAcceptClientDataReceived(e);

                if (EventProcessorManager.ProcessStartDataEventProcessor.IsMultiProcess)
                {

                }
                else
                {
                    IntPtr handle = e.ClientSocket.Handle;
                    EventProcessorManager.ProcessStartDataEventProcessor.SingleProcessInfo.SocketHandle = handle;
                    SingleProcessEvent.Set();
                    OnMessageObjectReceived("SingleProcess Set()");
                }
            }
            OnMessageObjectReceived("TcpServer_AcceptClient ============");
        }

        private void TcpServer_DataReceivedClient(object sender, DataReceivedEventArgs e)
        {
            if (e.Exception == null)
            {
                OnMessageObjectReceived("============ TcpServer_DataReceivedClient");
                OnMessageObjectReceived(e.ToString());
                DefaultServerSocket.OnClientReceivedData(e);
                OnMessageObjectReceived("TcpServer_DataReceivedClient ============");
            }
            else
            {
                OnMessageObjectReceived("TcpServer_DataReceivedClient Error");
                OnMessageObjectReceived("Cause=" + e.Exception.Message);
                OnMessageObjectReceived(e.Exception.StackTrace);
            }
        }

        private void TcpServer_DisconnectedClient(object sender, DisconnectEventArgs e)
        {
            OnMessageObjectReceived("============ TcpServer_DisconnectedClient");
            
            if (AgentServerSocket == AgentServerSocket.Web)
            {
                string data = new JObject()
                   .AddString(EventConstant.EventName, EventConstant.TcpClientDisconnectEvent)
                   .AddInt(EventConstant.SocketHandle, (int)e.EventSocket.Handle)
                   .AddInt(EventConstant.Port, ((IPEndPoint)e.EventSocket.RemoteEndPoint).Port)
                   .AddString(EventConstant.IPAddress, ((IPEndPoint)e.EventSocket.RemoteEndPoint).Address.ToString())
                   .ToString();

                ((AgentWebServerSocket)DefaultServerSocket).SendWebClient(e.AppId, data);
            }

            OnMessageObjectReceived("TcpServer_Listen TcpServer_DisconnectedClient");
        }

        private void StartTcpServer(int port)
        {
            if (TcpServer == null)
            {
                TcpServer = new AgentTcpServer("127.0.0.1", port);
                TcpServer.Listen += new ListenEventHandler(TcpServer_Listen);
                TcpServer.AcceptClient += new AcceptClientEventHandler(TcpServer_AcceptClient);
                TcpServer.ClientDataReceived += new WFAAgent.Framework.Net.Sockets.DataReceivedEventhandler(TcpServer_DataReceivedClient);
                TcpServer.DisconnectedClient += new DisconnectedEventHandler(TcpServer_DisconnectedClient);
                TcpServer.Start();
            }
        }

        private void StopTcpServer()
        {
            if (TcpServer != null)
            {
                TcpServer.Listen -= new ListenEventHandler(TcpServer_Listen);
                TcpServer.AcceptClient -= new AcceptClientEventHandler(TcpServer_AcceptClient);
                TcpServer.ClientDataReceived -= new WFAAgent.Framework.Net.Sockets.DataReceivedEventhandler(TcpServer_DataReceivedClient);
                TcpServer.DisconnectedClient -= new DisconnectedEventHandler(TcpServer_DisconnectedClient);
                TcpServer.Stop();
                TcpServer = null;
            }
        }

        #region Process Event


        private void AgentManager_StartAgentTcpServer(object sender, StartAgentTcpServerEventArgs e)
        {
            try
            {
                StartTcpServer(e.Port);
            }
            catch (Exception ex)
            {
                OnMessageObjectReceived(ex.ToString());
            }
        }

        private void AgentManager_ProcessStarted(object sender, ProcessStartedEventArgs e)
        {
            OnMessageObjectReceived("===== AgentWebSocketServer_ProcessStarted =====");

            DefaultServerSocket.OnProcessStarted(e.ProcessInfo);

            OnMessageObjectReceived("===== AgentWebSocketServer_ProcessStarted =====\n");
        }

        private void AgentManager_ProcessExited(object sender, ProcessExitedEventArgs e)
        {
            OnMessageObjectReceived("===== AgentWebSocketServer_ProcessExited =====");

            DefaultServerSocket.OnProcessExited(e.ProcessInfo);

            OnMessageObjectReceived("===== AgentWebSocketServer_ProcessExited =====\n");
        }


        private void AgentTcpServer_ProcessStartSendData(object sender, ProcessStartedSendDataEventArgs e)
        {
            OnMessageObjectReceived("===== AgentWebSocketServer_ProcessExited =====");

            if (EventProcessorManager.ProcessStartDataEventProcessor.IsMultiProcess)
            {

            }
            else
            {
                SingleProcessEvent.WaitOne();
                OnMessageObjectReceived("SingleProcess WaitOne()");
                TcpServer.SendProcessStartData(new ProcessStartData()
                {
                    AppId = e.ProcessInfo.AppId,
                    Data = e.Data,
                    ProcessId = e.ProcessInfo.Process.Id,
                    SocketHandle = e.ProcessInfo.SocketHandle
                });
            }

            OnMessageObjectReceived("===== AgentWebSocketServer_ProcessExited =====");
        }

        private void AgentManager_ProcessEventSendData(object sender, ProcessEventSendDataEventArgs e)
        {
            OnMessageObjectReceived("===== AgentManager_ProcessEventSendData =====");

            if (EventProcessorManager.ProcessStartDataEventProcessor.IsMultiProcess)
            {

            }
            else
            {
                ProcessInfo singleProcessInfo = EventProcessorManager.ProcessStartDataEventProcessor.SingleProcessInfo;
                TcpServer.SendProcessEventData(new ProcessEventData()
                {
                    AppId = singleProcessInfo.AppId,
                    Data = e.Data,
                    ProcessId = singleProcessInfo.Process.Id,
                    SocketHandle = singleProcessInfo.SocketHandle
                });
            }

            OnMessageObjectReceived("===== AgentManager_ProcessEventSendData =====");
        }

        #endregion

        public void OnRequestClientDataReceived(JObject messageObj)
        {
            string eventName = messageObj[EventConstant.EventName].ToObject<string>();

            IEventProcessor eventProcessor = null;
            if (!EventProcessorManager.EventProcessors.ContainsKey(eventName))
            {
                eventProcessor = EventProcessorManager.AddStartsWithByEventName(eventName);
                if (eventProcessor is ProcessStartDataEventProcessor)
                {
                    ((ProcessStartDataEventProcessor)eventProcessor).ProcessStarted += AgentManager_ProcessStarted;
                    ((ProcessStartDataEventProcessor)eventProcessor).ProcessExited += AgentManager_ProcessExited;
                    ((ProcessStartDataEventProcessor)eventProcessor).ProcessStartedSendData += AgentTcpServer_ProcessStartSendData;
                    ((ProcessStartDataEventProcessor)eventProcessor).StartAgentTcpServer += AgentManager_StartAgentTcpServer;
                }
                else if (eventProcessor is ProcessEventDataEventProcessor)
                {
                    ((ProcessEventDataEventProcessor)eventProcessor).ProcessEventSendData += AgentManager_ProcessEventSendData;
                }
            }
            else
            {
                eventProcessor = EventProcessorManager.EventProcessors[eventName];
            }

            
            JObject data = messageObj[EventConstant.Data] as JObject;

            ClientEventData eventData = new ClientEventData() { Data = data };
            switch (AgentServerSocket)
            {
                case AgentServerSocket.Web:
                    string sessionId = messageObj[EventConstant.SessionID].ToObject<string>();
                    eventData.AppId = sessionId;
                    break;
                case AgentServerSocket.Tcp:
                    eventData.AppId = Guid.NewGuid().ToString();
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
