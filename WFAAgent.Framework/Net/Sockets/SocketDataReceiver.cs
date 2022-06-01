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

        internal bool TryReadData(Header header, out byte[] data, out Exception exception)
        {
            bool isRead = true;
            byte[] buffer = ReadData(header, out isRead, out exception);
            if (isRead)
            {
                data = buffer;
            }
            else
            {
                data = null;
            }
            return isRead;
        }

        private byte[] ReadData(Header header, out bool isRead, out Exception ex)
        {
            isRead = false;
            ex = null;
            return null;
        }

        internal bool TryReadSocket(Header header, out byte[] data, out Exception exception)
        {
            bool isReceive = true;
            byte[] buffer = ReceiveSocketData(header, out isReceive, out exception);
            if (isReceive)
            {
                data = buffer;
            }
            else
            {
                data = null;
            }
            return isReceive;
        }

        private byte[] ReceiveSocketData(Header header, out bool isReceive, out Exception exception)
        {
            int size = header.DataLength;
            byte[] dataBuffer = new byte[size];
            int offset = 0;

            try
            {
                isReceive = true;
                int receiveBytes = 0;
                while (offset < size)
                {
                    receiveBytes = Socket.Receive(dataBuffer, offset, size - offset, SocketFlags.None);
                    if (receiveBytes == 0)
                    {
                        // TODO: 2. 데이터 읽는중 클라이언트 소켓 해제
                        isReceive = false;
                        break;
                    }
                    else
                    {
                        offset += receiveBytes;
                    }
                }
                
                exception = null;
            }
            catch (Exception ex)
            {
                isReceive = false;

                dataBuffer = null;
                exception = ex;
            }

            return dataBuffer;
        }
    }
}
