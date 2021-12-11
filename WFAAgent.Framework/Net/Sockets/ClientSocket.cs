using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace WFAAgent.Framework.Net.Sockets
{
    public class ClientSocket : DefaultSocket
    {
        public event ConnectedEventhandler Connected;
        public event DisconnectedEventHandler Disconnected;
        public event DataReceivedEventhandler ServerDataReceived;
        public event AsyncSendCompletedEventHandler AsyncSendCompleted;

        private Thread _Thread;

        private static object DisconnectLock = new object();
        public ClientSocket(string ipString, int port)
            : base(ipString, port)
        {
            
        }


        public bool CanUseSocket
        {
            get { return Socket != null && Socket.Connected; }
        }

        public bool Connect()
        {
            try
            {
                Initialize();
                                
                Socket.Connect(IPEndPoint);
                Connected?.Invoke(this, new ConnectedEventArgs(Socket));

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
                        Disconnected?.Invoke(this, new DisconnectEventArgs(Socket, false));

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
            int receiveBytes = 0;
            byte[] dataPacketHeaderBuffer = DataContext.NewDefaultDataPacketHeaderBuffer();
            while (true)
            {
                Array.Clear(dataPacketHeaderBuffer, 0, dataPacketHeaderBuffer.Length);
                receiveBytes = Socket.Receive(dataPacketHeaderBuffer, dataPacketHeaderBuffer.Length, SocketFlags.None);
                if (receiveBytes == 0)
                {
                    // TOOO: 연결 해제 작업
                }
                else
                {
                    bool isReceive = true;
                    Exception exception = null;
                    Header header = DataPacket.ToHeader(dataPacketHeaderBuffer);

                    int size = header.DataLength;
                    byte[] dataBuffer = new byte[size];
                    int offset = 0;

                    try
                    {
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
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }


                    if (isReceive)
                    {
                        DataReceivedEventArgs e = new DataReceivedEventArgs();
                        e.SetData(header, dataBuffer);
                        ServerDataReceived?.Invoke(this, e);
                    }
                    else
                    {
                        DataReceivedEventArgs e = new DataReceivedEventArgs();
                        e.Exception = exception;
                        ServerDataReceived?.Invoke(this, e);
                    }
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
        public int Send(DataPacket packet, string data)
        {
            if (CanUseSocket)
            {
                try
                {
                    int sendBytes = Socket_Send(packet, data);
                    return sendBytes;
                }
                catch (Exception ex)
                {
                    Error(ex);
                    throw;
                }
            }
            return -1;
        }

        public int Send(DataPacket packet, byte[] data)
        {
            if (CanUseSocket)
            {
                int sendBytes = Socket_Send(packet, data);
                return sendBytes;
            }
            return -1;
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
