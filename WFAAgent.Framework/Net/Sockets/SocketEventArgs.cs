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

    public class ConnectedEventArgs : SocketEventArgs
    {
        public ConnectedEventArgs(Socket socket) 
            : base(socket)
        {
            
        }

        public Socket ClientSocket
        {
            get { return EventSocket; }
        }
    }

    public class DisconnectEventArgs : SocketEventArgs
    {
        public DisconnectEventArgs(Socket socket, bool isServer) 
            : base(socket)
        {
            IsServer = isServer;
        }

        public bool IsServer { get; set; }
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

    public class DataReceivedEventArgs
    {
        public Header Header { get; set; }
        public Exception Exception { get; set; }
        public byte[] RawData { get; private set; }
        public string Data { get; private set; }

        internal void SetData(Header header, byte[] dataBuffer)
        {
            Header = header;
            switch (header.TransmissionData)
            {
                case TransmissionData.Binary:
                    RawData = dataBuffer;
                    break;
                case TransmissionData.Text:
                    Data = Encoding.UTF8.GetString(dataBuffer);
                    break;
            }
        }

        public override string ToString()
        {
            switch (Header.TransmissionData)
            {
                case TransmissionData.Binary:
                    return new StringBuilder()
                        .AppendFormat("[Header]").AppendLine()
                        .AppendFormat("Type={0}", Header.Type).AppendLine()
                        .AppendFormat("RawData={0}", RawData).AppendLine()
                        .ToString();
                case TransmissionData.Text:
                    return new StringBuilder()
                        .AppendFormat("[Header]").AppendLine()
                        .AppendFormat("Type={0}", Header.Type).AppendLine()
                        .AppendFormat("Data={0}", Data).AppendLine()
                        .ToString();
                default:
                    return String.Empty;
            }
        }
    }
}
