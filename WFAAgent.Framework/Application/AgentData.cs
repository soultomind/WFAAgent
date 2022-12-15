using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework.Application
{
    public class AgentData
    {
        public int ProcessId { get; set; }
        public string Data { get; set; }
    }

    public class AgentErrorData : AgentData
    {

    }

    public class AgentOutputData : AgentData
    {

    }
}
