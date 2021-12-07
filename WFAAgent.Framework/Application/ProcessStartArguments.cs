using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework.Application
{
    public class ProcessStartArguments
    {
        public string SessionID { get; set; }
        public int AgentTcpServerPort { get; set; }
        public static ProcessStartArguments Parse(JObject argObj)
        {
            ProcessStartArguments o = new ProcessStartArguments();
            o.SessionID = argObj[Context.ArgSessionID].ToObject<string>();
            o.AgentTcpServerPort = argObj[Context.ArgAgentTcpServerPort].ToObject<int>();
            return o;
        }

        public override string ToString()
        {
            return new StringBuilder()
                .AppendFormat("SessionID={0}, AgentTcpServerPort={1}", SessionID, AgentTcpServerPort)
                .ToString();
        }
    }
}
