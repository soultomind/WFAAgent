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

        public override void OnAcceptClientDataReceived(ushort type, string data)
        {

        }
        public override void OnAgentDataReceived(ushort type, string data)
        {

        }
    }
}
