using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFAAgent.Core;
using WFAAgent.Message;

namespace WFAAgent.Tcp
{
    public class AgentTcpSocketServer : IDefaultSocketServer
    {
        public IAgentManager AgentManager { get; set; }

        public event MessageObjectReceivedEventHandler MessageObjectReceived;

        public void Start()
        {
            
        }

        public void Stop()
        {
            
        }

        public void OnProcessStarted(ProcessInfo processInfo)
        {

        }

        public void OnProcessExited(ProcessInfo processInfo)
        {

        }
    }
}
