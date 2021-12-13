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

            try
            {
                SocketServer.Start();
            }
            catch (Exception ex)
            {
                throw new DefaultServerSocketException(ex.Message, ex);
            }
            
            try
            {
                StartTcpServer();
            }
            catch (Exception ex)
            {
                throw new AgentTcpServerException(ex.Message, ex);
            }
        }

        public void StopServer()
        {
            SocketServer.Stop();
            SocketServer.MessageObjectReceived -= OnMessageObjectReceived;
            SocketServer.AgentManager = null;

            SocketServer = null;

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

                ((AgentWebSocketServer)SocketServer).BroadCastTcpServerEvent(data);
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

                ((AgentWebSocketServer)SocketServer).BroadCastTcpServerEvent(data);
            }
            OnMessageObjectReceived("TcpServer_AcceptClient ============");
        }

        private void TcpServer_DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Exception == null)
            {
                OnMessageObjectReceived("============ TcpServer_DataReceived");
                OnMessageObjectReceived(e.ToString());
                if (e.Exception == null)
                {
                    switch (e.Header.TransmissionData)
                    {
                        case TransmissionData.Text:
                            SocketServer.OnDataReceived(e.Header.Type, e.Data);
                            break;
                        case TransmissionData.Binary:
                            SocketServer.OnDataReceived(e.Header.Type, e.RawData);
                            break;
                    }
                }
                else
                {
                    OnMessageObjectReceived("Error Message=" + e.Exception.Message);
                    OnMessageObjectReceived(e.Exception.StackTrace);
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
            OnMessageObjectReceived("===== AgentWebSocketServer_ProcessStarted =====");

            SocketServer.OnProcessStarted(sender as ProcessInfo);

            OnMessageObjectReceived("===== AgentWebSocketServer_ProcessStarted =====\n");
        }

        private void AgentWebSocketServer_ProcessExited(object sender, EventArgs e)
        {
            OnMessageObjectReceived("===== AgentWebSocketServer_ProcessExited =====");

            SocketServer.OnProcessExited(sender as ProcessInfo);

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

            // TODO: ThreadPool 사용필요
            eventProcessor.DoProcess(eventData);
        }
    }
}
