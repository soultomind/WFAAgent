using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Server
{
    public class ClientEventData
    {
        public JObject Data { get; set; }
        public string AppId { get; set; }

        public ClientEventData()
        {

        }
    }
}
