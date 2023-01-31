using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Server
{
    public class StartAgentTcpServerEventArgs : EventArgs
    {
        public int Port { get; private set; }
        
        public StartAgentTcpServerEventArgs(int port)
        {
            Port = port;
        }
    }
}
