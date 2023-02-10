using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework.Net.Sockets
{
    public class SocketDataReceiver
    {
        public Socket Socket { get; internal set; }
        public byte[] Data { get; internal set; }
        public SocketDataReceiver(Socket socket)
        {
            Socket = socket;
        }

        public SocketDataReceiver(byte[] data)
        {
            Data = data;
        }

        internal bool TryReadSocket(Header header, out byte[] data, out Exception exception)
        {
            byte[] buffer = null;
            Exception ex = null;
            if (TryReceiveSocketData(header, out buffer, out ex))
            {
                data = buffer;
                exception = null;
            }
            else
            {
                data = null;
                exception = ex;
            }

            return data != null;
        }

        private bool TryReceiveSocketData(Header header, out byte[] buffer, out Exception exception)
        {
            int size = header.DataLength;
            buffer = new byte[size];
            int offset = 0;

            try
            {
                int receiveBytes = 0;
                while (offset < size)
                {
                    receiveBytes = Socket.Receive(buffer, offset, size - offset, SocketFlags.None);
                    if (receiveBytes == 0)
                    {
                        throw new AgentTcpClientException("");
                    }
                    else
                    {
                        offset += receiveBytes;
                    }
                }
                
                exception = null;
                return true;
            }
            catch (Exception ex)
            {
                buffer = null;
                exception = ex;
                return false;
            }
        }
    }
}
