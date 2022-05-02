using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework.Application
{
    public class CallbackDataProcess
    {
        /// <summary>
        /// AppId
        /// <para><see cref="WFAAgent.Framework.AgentServerSocket.Web"/>=세션 연결시 생성되는 SessionId</para>
        /// <para><see cref="WFAAgent.Framework.AgentServerSocket.Tcp"/>=<see cref="Guid.NewGuid()"/>.ToString()</para>
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 통신할 Agent의 TCP/IP 소켓서버 포트번호
        /// </summary>
        public int AgentTcpServerPort { get; set; }

        /// <summary>
        /// 현재 프로세스 Id
        /// </summary>
        public int ProcessId { get; private set; }
        
        public CallbackDataProcess(Process currentProcess)
        {
            ProcessId = currentProcess.Id;
        }

        public override string ToString()
        {
            return new StringBuilder()
                .AppendFormat("AppId={0}, AgentTcpServerPort={1}", AppId, AgentTcpServerPort)
                .ToString();
        }

        public static CallbackDataProcess Parse(JObject argObj, Process currentProcess)
        {
            CallbackDataProcess o = new CallbackDataProcess(currentProcess);
            o.AppId = argObj[Constant.AppID].ToObject<string>();
            o.AgentTcpServerPort = argObj[Constant.AgentTcpServerPort].ToObject<int>();
            return o;
        }
    }
}
