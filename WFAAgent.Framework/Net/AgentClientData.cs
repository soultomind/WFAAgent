using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFAAgent.Framework.Application;

namespace WFAAgent.Framework.Net
{
    public class AgentClientData : DefaultData
    {
        public string AppId { get; set; }
        public int ProcessId { get; set; }

        public AgentClientData()
        {
            Type = GetType().Name;
        }

        public AgentClientData(CallbackDataProcess callbackDataProcess)
            : this()
        {
            AppId = callbackDataProcess.AppId;
            ProcessId = callbackDataProcess.ProcessId;
        }

        public override JObject ToJson(bool addProperty)
        {
            JObject retObj = base.ToJson(addProperty);
            if (addProperty)
            {
                retObj.Add(Constant.AppID, AppId);
                retObj.Add(Constant.ProcessId, ProcessId);
            }
            
            return retObj;
        }
    }
}
