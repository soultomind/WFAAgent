using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFAAgent.Core;
using WFAAgent.Framework.Net.Sockets;
using WFAAgent.Message;

namespace WFAAgent
{
    public abstract class DefaultSocketServer : IDefaultSocketServer
    {
        public abstract IAgentManager AgentManager { get; set; }

        public abstract event MessageObjectReceivedEventHandler MessageObjectReceived;

        public abstract void Start();
        public abstract void Stop();

        public abstract void OnProcessExited(ProcessInfo processInfo);
        public abstract void OnProcessStarted(ProcessInfo processInfo);
        

        public void OnDataReceived(DataReceivedEventArgs e)
        {

        }

        public void OnDataReceived(ushort type, string data)
        {
            
        }

        public void OnDataReceived(ushort type, byte[] data)
        {
            
        }
    }
}
