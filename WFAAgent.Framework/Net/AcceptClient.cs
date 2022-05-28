using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFAAgent.Framework.Application;

namespace WFAAgent.Framework.Net
{
    public class AcceptClient : DefaultData
    {
        public string AppId { get; set; }
        public int ProcessId { get; set; }

        public AcceptClient()
        {
            Type = GetType().Name;
        }

        public AcceptClient(CallbackDataProcess callbackDataProcess)
            : this()
        {
            AppId = callbackDataProcess.AppId;
            ProcessId = callbackDataProcess.ProcessId;
        }

        public override JObject ToJson()
        {
            JObject retObj = base.ToJson();
            //retObj.Add(Constant.AppID, AppId);
            retObj.Add(Constant.ProcessId, ProcessId);
            return retObj;
        }

        public JObject ToCustomJson()
        {
            return ToJson();
        }
    }
}
