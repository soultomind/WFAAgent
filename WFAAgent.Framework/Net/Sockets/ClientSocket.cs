using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace WFAAgent.Framework.Net.Sockets
{
    public class ClientSocket : DefaultSocket
    {
        public string IPString
        {
            get { return _IPString; }
            set { _IPString = value; }
        }
        private string _IPString = "127.0.0.1";
        public int Port
        {
            get { return _Port; }
            set { _Port = value; }
        }
        private int _Port = 0;
        public event DisconnectedEventHandler Disconnected;
        public event AsyncSendCompletedEventHandler AsyncSendCompleted;
        private Thread _Thread;

        private static object DisconnectLock = new object();
        public ClientSocket()
        {
            
        }


        public bool CanUseSocket
        {
            get { return Socket != null && Socket.Connected; }
        }

        public bool Connect(int port)
        {
            return Connect(IPString, port);
        }

        public bool Connect(string ipString, int port)
        {
            try
            {
                // TODO: IPString, 정규식 체크 필요
                if (!IsValidIPString(ipString))
                {
                    throw new ArgumentException("This is an invalid ipString.");
                }
                IPString = ipString;
                if (!IsValidPort(port))
                {
                    throw new ArgumentException("Usage port range is 1024-49151");
                }
                Port = port;

                Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ipAddress = IPAddress.Parse(IPString);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, Port);
                Socket.Connect(remoteEP);

                _Thread = new Thread(Receive);
                _Thread.Start();
                return true;
            }
            catch (Exception ex)
            {
                Error(ex);
            }
            return false;
        }

        public void Disconnect()
        {
            lock (DisconnectLock)
            {
                if (Socket != null)
                {
                    try
                    {
                        Socket.Disconnect(false);
                        if (Disconnected != null)
                        {
                            Disconnected(this, new DisconnectEventArgs(Socket, false));
                        }                        

                        Socket.Close();
                        Socket = null;
                    }
                    catch (Exception ex)
                    {
                        Error(ex);
                    }
                }
            }
        }

        private void Receive()
        {
            // 추후에 8바이트 먼져 읽은 후 해당 헤더에서 실제 헤더 길이 구해서 더 읽어야 함
            byte[] dataPacketHeaderBuffer = DataContext.NewDefaultDataPacketHeaderBuffer();
            while (true)
            {
                Array.Clear(dataPacketHeaderBuffer, 0, dataPacketHeaderBuffer.Length);
                int receiveBytes = Socket.Receive(dataPacketHeaderBuffer, dataPacketHeaderBuffer.Length, SocketFlags.None);
                if (receiveBytes == 0)
                {
                    // TOOO: 연결 해제 작업
                }
                else
                {
                    // TODO: 패킷을 읽어서 데이터 길이를 구한다음에 해당 길이만큼 읽어야함
                    Header header = DataPacket.ToHeader(dataPacketHeaderBuffer);

                    byte[] data = new byte[header.DataLength];
                    int offset = 0;

                    
                }
            }
        }

        #region Sync
        private int Socket_Send(DataPacket packet, string data)
        {
            byte[] sendData = packet.CreateData(data);
            return Socket.Send(sendData, 0, sendData.Length, SocketFlags.None);
        }
        private int Socket_Send(DataPacket packet, byte[] data)
        {
            byte[] sendData = packet.CreateData(data);
            return Socket.Send(sendData, 0, sendData.Length, SocketFlags.None);
        }
        public bool Send(DataPacket packet, string data)
        {
            if (CanUseSocket)
            {
                try
                {
                    int sendBytes = Socket_Send(packet, data);
                    if (sendBytes > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Error(ex);
                    throw;
                }
            }
            return false;
        }

        public bool Send(DataPacket packet, byte[] data)
        {
            if (CanUseSocket)
            {
                int sendBytes = Socket_Send(packet, data);
                if (sendBytes > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        #endregion

        #region Async
        private IAsyncResult Socket_BeginSend(DataPacket packet, string data)
        {
            byte[] sendData = packet.CreateData(data);
            return Socket.BeginSend(sendData, 0, sendData.Length, SocketFlags.None, new AsyncCallback(AsyncSendData), new AsyncSendSocketState() { Socket = Socket });
        }
        private IAsyncResult Socket_BeginSend(DataPacket packet, byte[] data)
        {
            byte[] sendData = packet.CreateData(data);
            return Socket.BeginSend(sendData, 0, sendData.Length, SocketFlags.None, new AsyncCallback(AsyncSendData), new AsyncSendSocketState() { Socket = Socket });
        }

        public bool SendAsync(DataPacket packet, string data)
        {
            if (CanUseSocket)
            {
                try
                {
                    IAsyncResult asyncResult = Socket_BeginSend(packet, data);
                }
                catch (Exception ex)
                {
                    Error(ex);
                    throw;
                }
            }
            return false;
        }

        public bool SendAsync(DataPacket packet, byte[] data)
        {
            if (CanUseSocket)
            {
                try
                {
                    IAsyncResult asyncResult = Socket_BeginSend(packet, data);
                }
                catch (Exception ex)
                {
                    Error(ex);
                    throw;
                }
            }
            return false;
        }

        private void AsyncSendData(IAsyncResult asyncResult)
        {
            AsyncSendSocketState state = asyncResult.AsyncState as AsyncSendSocketState;

            try
            {
                int sendBytes = state.Socket.EndSend(asyncResult);
                state.SendBytes = sendBytes;
            }
            catch (Exception ex)
            {
                state.Exception = ex;
                Error(ex);
            }

            AsyncSendCompleted?.Invoke(this, new AsyncSendSocketEventArgs(state.Socket) { Exception = state.Exception, SendBytes = state.SendBytes });
        }
        #endregion
    }
}
