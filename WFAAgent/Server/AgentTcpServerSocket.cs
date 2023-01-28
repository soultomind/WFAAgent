using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFAAgent.Core;
using WFAAgent.Framework.Net.Sockets;

namespace WFAAgent.Server
{
    public class AgentTcpServerSocket : DefaultServerSocket
    {
        public override IAgentManager AgentManager { get; set; }

        public override event MessageObjectReceivedEventHandler MessageObjectReceived;

        public override void Start()
        {
            
        }

        public override void Stop()
        {
            
        }

        public override void OnProcessStarted(ProcessInfo processInfo)
        {

        }

        public override void OnProcessExited(ProcessInfo processInfo)
        {

        }

        public override void OnAcceptClientDataReceived(AcceptClientEventArgs e)
        {

        }
        public override void OnAgentDataReceived(DataReceivedEventArgs e)
        {

        }

        public override void OnClientSendData(DataSendEventArgs e)
        {

        }
    }
}
