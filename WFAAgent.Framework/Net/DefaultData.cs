using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFAAgent.Framework.Application;

namespace WFAAgent.Framework.Net
{
    public abstract class DefaultData
    {
        public string EventName { get; set; }
        public string Type { get; set; } = String.Empty;
        public DefaultData()
        {
            EventName = "TcpClientDataReceived";
        }

        public virtual JObject ToJson()
        {
            JObject retObj = new JObject();
            //retObj.Add(Constant.DataTypeString, GetType().Name);
            return retObj;
        }

        public static string ToStringSerializeObject(DefaultData value)
        {
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            string data = JsonConvert.SerializeObject(value, new JsonSerializerSettings()
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            });

            return data;
        }
    }
}
