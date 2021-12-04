using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework.Net.Sockets
{
    public class SocketEventArgs : EventArgs
    {
        public SocketEventArgs(Socket socket)
        {
            EventSocket = socket;
        }

        protected Socket EventSocket
        {
            get;
            private set;
        }
    }

    public class ListenEventArgs : SocketEventArgs
    {
        public ListenEventArgs(Socket socket) : base(socket)
        {

        }

        public Socket ServerSocket
        {
            get { return EventSocket; }
        }
    }

    public class AcceptClientEventArgs : SocketEventArgs
    {
        public AcceptClientEventArgs(Socket socket) : base(socket)
        {
        }

        public Socket ServerSocket
        {
            get { return EventSocket; }
        }

        public Socket ClientSocket
        {
            get;
            set;
        }
    }

    public class DisconnectEventArgs : SocketEventArgs
    {
        public DisconnectEventArgs(Socket socket, bool isServer) : base(socket)
        {
            IsServer = isServer;
        }

        public bool IsServer { get; set; }

        public Socket ServerSocket
        {
            get { return EventSocket; }
        }
    }

    public class AsyncSendSocketEventArgs : SocketEventArgs
    {
        public AsyncSendSocketEventArgs(Socket socket)
            : base(socket)
        {

        }

        public Exception Exception { get; internal set; }
        public int SendBytes { get; internal set; }
    }
}
