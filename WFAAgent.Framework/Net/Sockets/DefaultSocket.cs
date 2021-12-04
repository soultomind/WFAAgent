using System;
using System.Net.Sockets;

namespace WFAAgent.Framework.Net.Sockets
{
    public abstract class DefaultSocket
    {
        public const int MinPort = 1024;
        public const int MaxPort = 49151;
        
        public Socket Socket
        {
            get { return _Socket; }
            protected set { _Socket = value; }
        }
        private volatile Socket _Socket;
        public DefaultSocket()
        {
            
        }

        public bool IsValidIPString(string ipString)
        {
            return true;
        }

        public bool IsValidPort(int port)
        {
            if (MinPort <= port && MaxPort >= port)
            {
                return true;
            }
            return false;
        }

        protected void Error(Exception ex)
        {
            if (ex is SocketException)
            {
                int errorCode = ((SocketException)ex).ErrorCode;
            }
            else
            {
                
            }
        }
    }
}
