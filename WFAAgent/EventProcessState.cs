using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFAAgent.Core;
using WFAAgent.WebSocket;

namespace WFAAgent
{
    public class EventProcessState
    {
        public IEventProcessor EventProcessor { get; set; }
        public EventData EventData { get; set; }
    }
}
