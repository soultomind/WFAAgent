using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework.Net.Sockets
{
    public abstract class SocketEventArgs : EventArgs
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

    public class ServerSocketEventArgs : SocketEventArgs
    {
        public ServerSocketEventArgs(Socket socket) : base(socket)
        {
        }
    }
    public class ServerListenEventArgs : ServerSocketEventArgs
    {
        public ServerListenEventArgs(Socket socket) : base(socket)
        {

        }

        public Socket ServerSocket
        {
            get { return EventSocket; }
        }
    }

    public class ServerClientAcceptEventArgs : ServerSocketEventArgs
    {
        public ServerClientAcceptEventArgs(Socket socket) : base(socket)
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

    public class ServerClientDisconnectEventArgs : ServerSocketEventArgs
    {
        public ServerClientDisconnectEventArgs(Socket socket) : base(socket)
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
}
