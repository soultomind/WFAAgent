using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent
{
    public class EventConstant
    {
        public const string EventName = "eventName";

        public const string Exception = "Exception";

        public const string ProcessStartedEvent = "ProcessStarted";
        public const string ProcessExitedEvent = "ProcessExited";

        public const string TcpServerListenEvent = "TcpServerListenEvent";
        public const string SocketHandle = "socketHandle";
        public const string Port = "port";
        public const string IPAddress = "ipAddress";

        public const string TcpServerAcceptClientEvent = "TcpServerAcceptClientEvent";


        public const string Data = "data";
        public const string SessionID = "sessionId";
    }
}
