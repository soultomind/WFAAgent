using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework.Net
{
    public class AcceptClient : DefaultData
    {
        public int ProcessId { get; set; }
        public string AppId { get; set; }

        public AcceptClient()
        {
            Type = GetType().Name;
        }
    }
}
