using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFAAgent.Core;
using WFAAgent.Framework.Net.Sockets;
using WFAAgent.Message;

namespace WFAAgent.Server
{
    public interface IDefaultSocketServer
    {
        event MessageObjectReceivedEventHandler MessageObjectReceived;
        IAgentManager AgentManager { get; set; }
        
        void Start();
        void Stop();

        void OnProcessStarted(ProcessInfo processInfo);
        void OnProcessExited(ProcessInfo processInfo);
        void OnAcceptClientDataReceived(AcceptClientEventArgs e);
        void OnClientReceivedData(DataReceivedEventArgs e);
        void OnClientSendData(DataSendEventArgs e);
    }
}
