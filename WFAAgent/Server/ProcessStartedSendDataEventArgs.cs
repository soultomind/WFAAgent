using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Server
{
    public class ProcessStartedSendDataEventArgs
    {
        public ProcessInfo ProcessInfo { get; private set; }
        public string Data { get; private set; }

        public ProcessStartedSendDataEventArgs(ProcessInfo processInfo, string data)
        {
            ProcessInfo = processInfo;
            Data = data;
        }
    }
}
