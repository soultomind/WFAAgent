using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework.Net
{
    public abstract class DefaultData
    {
        public string EventName { get; set; }
        public string Type { get; set; }
        public DefaultData()
        {
            EventName = "DataReceived";
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
