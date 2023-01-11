using WFAAgent.Server;

namespace WFAAgent.Server
{
    public interface IEventProcessor
    {
        void DoProcess(ClientEventData eventData);
    }
}
