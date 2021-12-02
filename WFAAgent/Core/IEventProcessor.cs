using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFAAgent.WebSocket;

namespace WFAAgent.Core
{
    public interface IEventProcessor
    {
        void DoProcess(EventData eventData);
    }
}
