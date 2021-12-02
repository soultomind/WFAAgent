using SuperSocket.SocketBase.Config;
using SuperSocket.WebSocket;
using SuperSocket.WebSocket.SubProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFAAgent.Message;
using WFAAgent.WebSocket;

namespace WFAAgent
{
    internal class AgentManager : IAgentManager
    {
        private AgentWebSocketServer _AgentWebSocketServer;
        public event MessageObjectReceivedEventHandler MessageObjectReceived;
        public void StartServer()
        {
            if (_AgentWebSocketServer == null)
            {
                _AgentWebSocketServer = new AgentWebSocketServer();
                _AgentWebSocketServer.MessageObjectReceived += OnMessageObjectReceived;
            }
            
           _AgentWebSocketServer.Start();
        }

        public void StopServer()
        {
            _AgentWebSocketServer.Stop();
        }

        internal void OnMessageObjectReceived(object messageObject)
        {
            MessageObjectReceived?.Invoke(messageObject);
        }
    }
}
