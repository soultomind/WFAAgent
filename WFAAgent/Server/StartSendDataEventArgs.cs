using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Server
{
    public class StartSendDataEventArgs : EventArgs
    {
        public string Data { get; set; }
    }
}
