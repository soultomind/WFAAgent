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
        public string AppStringData { get; set; }
        public byte[] AppBinaryData { get; set; }
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
            AppStringData = data;
        }

        public AgentData(CallbackDataProcess callbackDataProcess, byte[] data)
           : this(callbackDataProcess)
        {
            AppBinaryData = data;
        }

        public override JObject ToJson()
        {
            JObject retObj = base.ToJson();
            if (String.IsNullOrEmpty(AppStringData))
            {
                string base64 = Convert.ToBase64String(AppBinaryData);
                retObj.Add(Constant.AppBinaryData, base64);
            }
            else
            {
                retObj.Add(Constant.AppStringData, AppStringData);
            }
            
            return retObj;
        }
    }
}
