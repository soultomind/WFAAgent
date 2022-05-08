using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Threading;
using WFAAgent.Core;
using WFAAgent.Framework;
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
            MessageObjectReceived?.Invoke(messageObject);
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

            StopTcpServer();
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
                string data = new JObject()
                    .AddString(EventConstant.EventName, EventConstant.TcpServerAcceptClientEvent)
                    .AddInt(EventConstant.SocketHandle, (int)e.ClientSocket.Handle)
                    .AddInt(EventConstant.Port, ((IPEndPoint)e.ClientSocket.RemoteEndPoint).Port)
                    .AddString(EventConstant.IPAddress, ((IPEndPoint)e.ClientSocket.RemoteEndPoint).Address.ToString())
                    .ToString();

                ((AgentWebServerSocket)DefaultServerSocket).BroadCastTcpServerEvent(data);
            }
            OnMessageObjectReceived("TcpServer_AcceptClient ============");
        }

        private void TcpServer_DataReceivedClient(object sender, DataReceivedEventArgs e)
        {
            if (e.Exception == null)
            {
                OnMessageObjectReceived("============ TcpServer_DataReceivedClient");
                OnMessageObjectReceived(e.ToString());
                switch (e.Header.TransmissionData)
                {
                    case TransmissionData.Text:
                        DefaultServerSocket.OnDataReceived(e.Header.Type, e.Data);
                        break;
                    case TransmissionData.Binary:
                        DefaultServerSocket.OnDataReceived(e.Header.Type, e.RawData);
                        break;
                }
                OnMessageObjectReceived("TcpServer_DataReceivedClient ============");
            }
            else
            {
                OnMessageObjectReceived("TcpServer_DataReceivedClient Error");
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
                TcpServer.DataReceived += new WFAAgent.Framework.Net.Sockets.DataReceivedEventhandler(TcpServer_DataReceivedClient);
                TcpServer.Start();
            }
        }

        private void StopTcpServer()
        {
            if (TcpServer != null)
            {
                TcpServer.Listen -= new ListenEventHandler(TcpServer_Listen);
                TcpServer.AcceptClient -= new AcceptClientEventHandler(TcpServer_AcceptClient);
                TcpServer.DataReceived -= new WFAAgent.Framework.Net.Sockets.DataReceivedEventhandler(TcpServer_DataReceivedClient);
                TcpServer.Stop();
                TcpServer = null;
            }
        }

        #region Process Event
        private void AgentWebSocketServer_ProcessStarted(object sender, EventArgs e)
        {
            OnMessageObjectReceived("===== AgentWebSocketServer_ProcessStarted =====");

            DefaultServerSocket.OnProcessStarted(sender as ProcessInfo);

            OnMessageObjectReceived("===== AgentWebSocketServer_ProcessStarted =====\n");
        }

        private void AgentWebSocketServer_ProcessExited(object sender, EventArgs e)
        {
            OnMessageObjectReceived("===== AgentWebSocketServer_ProcessExited =====");

            DefaultServerSocket.OnProcessExited(sender as ProcessInfo);

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
            switch (AgentServerSocket)
            {
                case AgentServerSocket.Web:
                    string sessionId = messageObj[EventConstant.SessionID].ToObject<string>();
                    eventData.SessionId = sessionId;
                    break;
                case AgentServerSocket.Tcp:
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
