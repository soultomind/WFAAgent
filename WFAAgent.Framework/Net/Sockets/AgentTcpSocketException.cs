using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework.Net.Sockets
{
    public class AgentTcpSocketException : Exception
    {
        public AgentTcpSocketException(string message)
            : base(message)
        {

        }

        public AgentTcpSocketException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }

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
