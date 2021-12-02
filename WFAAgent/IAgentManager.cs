using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFAAgent.Message;

namespace WFAAgent
{
    public interface IAgentManager
    {
        void StartServer();
        void StopServer();

        event MessageObjectReceivedEventHandler MessageObjectReceived;
    }
}
