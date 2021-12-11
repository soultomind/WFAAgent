using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFAAgent.Core;
using WFAAgent.Message;

namespace WFAAgent
{
    public interface IDefaultSocketServer
    {
        IAgentManager AgentManager { get; set; }
        event MessageObjectReceivedEventHandler MessageObjectReceived;

        void Start();
        void Stop();

        void OnProcessStarted(ProcessInfo processInfo);
        void OnProcessExited(ProcessInfo processInfo);


        
    }
}
