using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework.Net.Sockets
{
    public class AsyncSendSocketState
    {
        public Socket Socket
        {
            get { return _Socket; }
            set { _Socket = value; }
        }
        private Socket _Socket;

        public Exception Exception
        {
            get; internal set;
        }
        public int SendBytes
        {
            get; internal set;
        }
    }
}
