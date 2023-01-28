using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFAAgent.Core;
using WFAAgent.Framework.Net.Sockets;
using WFAAgent.Message;

namespace WFAAgent.Server
{
    public abstract class DefaultServerSocket : IDefaultSocketServer
    {
        public abstract IAgentManager AgentManager { get; set; }

        public abstract event MessageObjectReceivedEventHandler MessageObjectReceived;

        public abstract void Start();
        public abstract void Stop();

        public abstract void OnProcessExited(ProcessInfo processInfo);
        public abstract void OnProcessStarted(ProcessInfo processInfo);
        
        public virtual void OnClientReceivedData(DataReceivedEventArgs e)
        {
            switch (e.Header.Type)
            {
                case DataContext.AgentStringData:
                case DataContext.AgentBinaryData:
                    OnAgentDataReceived(e);
                    break;
            }
        }

        public abstract void OnAcceptClientDataReceived(AcceptClientEventArgs e);
        public abstract void OnAgentDataReceived(DataReceivedEventArgs e);

        public abstract void OnClientSendData(DataSendEventArgs e);
    }
}
