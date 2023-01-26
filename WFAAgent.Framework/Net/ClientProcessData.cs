using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework.Net
{
    public class ClientProcessData
    {
        public string AppId { get; set; }
        public int ProcessId { get; set; }

        public IntPtr SocketHandle { get; set; }
        public string Data { get; set; }
    }
}
