using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Server
{
    public class ProcessStartedSendDataEventArgs
    {
        public string Data { get; private set; }
        public ProcessInfo ProcessInfo { get; internal set; }

        public ProcessStartedSendDataEventArgs(string data)
        {
            Data = data;
        }
    }
}
