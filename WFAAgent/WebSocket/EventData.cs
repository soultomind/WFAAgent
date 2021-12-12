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
        public string SessionID
        {
            get { return _SessionID; }
            set { _SessionID = value; }
        }
        private string _SessionID { get; set; }

        public EventData()
        {

        }
    }
}
