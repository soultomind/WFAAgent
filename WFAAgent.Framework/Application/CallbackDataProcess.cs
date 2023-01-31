using Newtonsoft.Json;
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
        /// 콜백 데이터 여부
        /// </summary>
        public bool UseCallBackData { get; set; }

        /// <summary>
        /// 현재 프로세스 Id
        /// </summary>
        [JsonIgnoreAttribute]
        public int ProcessId { get; private set; }
        
        public CallbackDataProcess()
        {

        }

        public CallbackDataProcess(Process currentProcess)
        {
            ProcessId = currentProcess.Id;
        }

        public override string ToString()
        {
            return new StringBuilder()
                .AppendFormat("AppId={0}, AgentTcpServerPort={1}, UseCallBackData={2}", AppId, AgentTcpServerPort, UseCallBackData)
                .ToString();
        }

        public static CallbackDataProcess Parse(JObject argObj, Process currentProcess)
        {
            // JsonConvert.DeserializeObject 사용시 디폴트 생성자 선언이 되어 있어야함
            CallbackDataProcess o = JsonConvert.DeserializeObject<CallbackDataProcess>(argObj.ToString());
            o.ProcessId = currentProcess.Id;
            return o;
        }
    }
}
