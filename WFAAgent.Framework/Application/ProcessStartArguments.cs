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
        /// <summary>
        /// SessionID
        /// <para><see cref="WFAAgent.Framework.ServerSocketType.Web"/>=세션 연결시 생성되는 SessionID</para>
        /// <para><see cref="WFAAgent.Framework.ServerSocketType.Tcp"/>=<see cref="Guid.NewGuid()"/>.ToString()</para>
        /// </summary>
        public string SessionID { get; set; }
        /// <summary>
        /// 통신할 Agent의 TCP/IP 소켓서버 포트번호
        /// </summary>
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
