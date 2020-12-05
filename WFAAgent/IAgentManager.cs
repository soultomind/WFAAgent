using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent
{
    public interface IAgentManager
    {
        void StartServer();
        void StopServer();
    }
}
