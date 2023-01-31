using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework.Net.Sockets
{
    public class AgentTcpServerException : AgentTcpSocketException
    {
        public AgentTcpServerException(string message)
            : base(message)
        {

        }

        public AgentTcpServerException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
