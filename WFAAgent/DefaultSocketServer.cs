using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFAAgent.Core;
using WFAAgent.Framework.Net.Sockets;
using WFAAgent.Message;

namespace WFAAgent
{
    public abstract class DefaultSocketServer : IDefaultSocketServer
    {
        public abstract IAgentManager AgentManager { get; set; }

        public abstract event MessageObjectReceivedEventHandler MessageObjectReceived;

        public abstract void Start();
        public abstract void Stop();

        public abstract void OnProcessExited(ProcessInfo processInfo);
        public abstract void OnProcessStarted(ProcessInfo processInfo);
        

        public virtual void OnDataReceived(ushort type, string data)
        {
            // 추후에 해당 부분을 동적으로 처리할 수 있는 방법
            // SPRING Annotation 비슷한?
            // @Controller 선언시 등록되는..
            switch (type)
            {
                case DataContext.AcceptClient:
                    OnAcceptClientDataReceived(type, data);
                    break;
                case DataContext.User:
                    OnUserDataReceived(type, data);
                    break;
            }
        }

        public virtual void OnDataReceived(ushort type, byte[] data)
        {
            
        }

        public abstract void OnAcceptClientDataReceived(ushort type, string data);
        public abstract void OnUserDataReceived(ushort type, string data);
    }
}
