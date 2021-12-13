using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework.Net
{
    public class DefaultData
    {
        public string EventName { get; set; }
        public string Type { get; set; }
        public DefaultData()
        {
            EventName = "DataReceived";
        }
    }
}
