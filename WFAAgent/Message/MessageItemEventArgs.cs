using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Message
{
    public class MessageItemEventArgs : EventArgs
    {
        public MessageItem MessageItem { get; set; }
    }
}
