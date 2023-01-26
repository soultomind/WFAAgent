using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Server
{
    public class ProcessEventSendDataEventArgs : EventArgs
    {
        public string Data { get; private set; }
        public string AppId { get; internal set; }
    }
}
