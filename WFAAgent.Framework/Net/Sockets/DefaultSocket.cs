using System;
using System.Net;
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

        public string IPString
        {
            get { return _IPString; }
            set { _IPString = value; }
        }
        private string _IPString = "127.0.0.1";

        public IPAddress IPAddress
        {
            get { return _IPAddress; }
        }
        public IPAddress _IPAddress;
        public int Port
        {
            get { return _Port; }
            set
            {
                if (!IsValidPort(value))
                {
                    throw new ArgumentException("port");
                }
                _Port = value;
            }
        }
        private int _Port = 0;

        public IPEndPoint IPEndPoint
        {
            get { return _IPEndPoint; }
        }
        private IPEndPoint _IPEndPoint;
        public DefaultSocket(string ipString, int port)
        {
            if (!IsValidIPString(ipString))
            {
                throw new ArgumentException(nameof(ipString));
            }

            IPString = ipString;
            Port = port;
        }

        public bool IsValidIPString(string ipString)
        {
            try
            {
                IPAddress.Parse(ipString);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool IsValidPort(int port)
        {
            if (MinPort <= port && MaxPort >= port)
            {
                return true;
            }
            return false;
        }

        protected void Initialize()
        {
            _IPAddress = System.Net.IPAddress.Parse(IPString);
            _IPEndPoint = new IPEndPoint(_IPAddress, Port);

            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
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
