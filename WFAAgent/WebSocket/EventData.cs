using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.WebSocket
{
    public class EventData
    {
        public JObject Data { get; set; }
        public string SessionID { get; set; }
    }
}
