using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFAAgent.WebSocket;

namespace WFAAgent.Core
{
    public abstract class EventProcessor : IEventProcessor
    {
        public abstract void DoProcess(EventData eventData);
    }
}
