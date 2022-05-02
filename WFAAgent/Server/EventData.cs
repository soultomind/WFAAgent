using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Server
{
    public class EventData
    {
        public JObject Data { get; set; }
        public string SessionId
        {
            get { return _SessionId; }
            set { _SessionId = value; }
        }
        private string _SessionId;

        public EventData()
        {

        }
    }
}
