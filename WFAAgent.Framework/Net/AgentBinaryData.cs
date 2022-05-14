using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFAAgent.Framework.Application;

namespace WFAAgent.Framework.Net
{
    public class AgentBinaryData : AcceptClient
    {
        public byte[] AppBinaryData { get; set; } = null;
        public bool IsFile { get; set; }
        public string Extension { get; set; } = String.Empty;

        public AgentBinaryData()
        {
            Type = GetType().Name;
        }

        private AgentBinaryData(CallbackDataProcess callbackDataProcess)
            : base(callbackDataProcess)
        {

        }

        /// <summary>
        /// 바이너리 데이터 생성자
        /// </summary>
        /// <param name="callbackDataProcess"></param>
        /// <param name="data"></param>
        public AgentBinaryData(CallbackDataProcess callbackDataProcess, byte[] data)
           : this(callbackDataProcess)
        {
            AppBinaryData = data;
        }
    }
}
