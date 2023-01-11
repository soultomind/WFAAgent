using WFAAgent.Core;
using WFAAgent.Server;

namespace WFAAgent.Server
{
    public class EventProcessState
    {
        public IEventProcessor EventProcessor { get; set; }
        public ClientEventData EventData { get; set; }
    }
}
