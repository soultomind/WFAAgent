using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFAAgent.Message;

namespace WFAAgent.Server
{
    public interface IAgentManager
    {
        event MessageObjectReceivedEventHandler MessageObjectReceived;

        void StartServer();
        void StopServer();

        void OnRequestClientDataReceived(JObject data);
    }
}
