using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework.Net.Sockets
{
    public class AgentTcpClientException : AgentTcpSocketException
    {
        public AgentTcpClientException(string message)
            : base(message)
        {

        }

        public AgentTcpClientException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
