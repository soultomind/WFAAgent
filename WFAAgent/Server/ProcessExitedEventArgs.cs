using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Server
{
    public class ProcessExitedEventArgs : EventArgs
    {
        public ProcessInfo ProcessInfo { get; private set; }

        public ProcessExitedEventArgs(ProcessInfo processInfo)
        {
            ProcessInfo = processInfo;
        }
    }
}
