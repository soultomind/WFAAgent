﻿using System;
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
}
