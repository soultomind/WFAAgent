using WFAAgent.Server;

namespace WFAAgent.Server
{
    public abstract class EventProcessor : IEventProcessor
    {
        public abstract void DoProcess(EventData eventData);
    }
}
