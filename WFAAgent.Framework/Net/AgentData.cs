using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFAAgent.Framework.Application;

namespace WFAAgent.Framework.Net
{
    public class AgentData : AcceptClient
    {
        public string AppData { get; set; }
        public AgentData()
        {
            Type = GetType().Name;
        }

        public AgentData(CallbackDataProcess callbackDataProcess)
            : base(callbackDataProcess)
        {

        }

        public AgentData(CallbackDataProcess callbackDataProcess, string data)
           : this(callbackDataProcess)
        {
            AppData = data;
        }

        public override JObject ToJson()
        {
            JObject retObj = base.ToJson();
            retObj.Add(Constant.AppData, AppData);
            return retObj;
        }
    }
}
