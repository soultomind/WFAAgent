using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Server
{
    public class ProcessStartedEventArgs : EventArgs
    {
        public ProcessInfo ProcessInfo { get; private set; }

        public ProcessStartedEventArgs(ProcessInfo processInfo)
        {
            ProcessInfo = processInfo;
        }
    }
}
