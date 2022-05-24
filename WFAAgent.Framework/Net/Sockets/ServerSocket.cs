using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WFAAgent.Framework.Net.Sockets
{
    public class ServerSocket : DefaultSocket
    {
        private volatile ConcurrentDictionary<IntPtr, AppClientSocket> _ClientSockets = 
            new ConcurrentDictionary<IntPtr, AppClientSocket>();

        public event DataReceivedEventhandler ClientDataReceived;
        public event AcceptClientEventHandler AcceptClient;
        public event DisconnectedEventHandler DisconnectedClient;

        private Thread _Thread;
        public int BackLog
        {
            get { return _BackLog; }
            set
            {
                if (value > 0 && value < 1000)
                {
                    _BackLog = value;
                }
            }
        }
        private int _BackLog = 999;

        private volatile bool _IsBeginAccept;
        public ServerSocket(string ipString, int port)
            : base(ipString, port)
        {
            
        }

        public bool Bind()
        {
            try
            {
                Socket.Bind(IPEndPoint);
                return true;
            }
            catch (Exception ex)
            {
                Error(ex);
            }
            return false;
        }

        public void Listen()
        {
            Listen(_BackLog);
        }

        public void Listen(int backlog)
        {
            if (_BackLog != backlog)
            {
                _BackLog = backlog;
            }
            Socket.Listen(backlog);
        }

        private void DoAcceptClient()
        {
            _IsBeginAccept = true;
            while (_IsBeginAccept)
            {
                IAsyncResult asyncResult = Socket.BeginAccept(StartAcceptClient, Socket);
            }
        }

        private void StartAcceptClient(IAsyncResult asyncResult)
        {
            Socket socket = asyncResult.AsyncState as Socket;

            Socket clientSocket = socket.EndAccept(asyncResult);

            byte[] dataPacketHeaderBuffer = DataContext.NewDefaultDataPacketHeaderBuffer();
            int receiveBytes = 0;

            try
            {
                while (true)
                {
                    Array.Clear(dataPacketHeaderBuffer, 0, dataPacketHeaderBuffer.Length);
                    receiveBytes = clientSocket.Receive(dataPacketHeaderBuffer, dataPacketHeaderBuffer.Length, SocketFlags.None);
                    if (receiveBytes == 0)
                    {
                        // 최초 AcceptClient 패킷 왔을 경우 등록
                        AppClientSocket obj = null;
                        if (_ClientSockets.TryRemove(clientSocket.Handle, out obj))
                        {
                            DisconnectEventArgs e = new DisconnectEventArgs(clientSocket) { AppId = obj.AppId };
                            DisconnectedClient?.Invoke(this, e);
                        }
                        
                        break;
                    }
                    else
                    {
                        Exception exception = null;
                        Header header = DataPacket.ToHeader(dataPacketHeaderBuffer);

                        // 실제 어플리케이션 단에서의 

                        byte[] dataBuffer = null;
                        SocketDataReceiver receiver = new SocketDataReceiver(clientSocket);
                        if (receiver.TryRead(header, out dataBuffer, out exception))
                        {
                            DataReceivedEventArgs e = new DataReceivedEventArgs();
                            e.SetData(header, dataBuffer);

                            if (header.Type == DataContext.AcceptClient)
                            {
                                JObject data = JObject.Parse(e.Data);
                                string appId = data["appId"].ToObject<string>();

                                AppClientSocket obj = new AppClientSocket(clientSocket, appId);
                                if (_ClientSockets.TryAdd(clientSocket.Handle, obj))
                                {
                                    
                                    AcceptClient?.Invoke(
                                        this,
                                        new AcceptClientEventArgs(socket)
                                        {
                                            ClientSocket = clientSocket,
                                            AppId = appId
                                        }
                                    );
                                }
                            }
                            else
                            {
                                ClientDataReceived?.Invoke(this, e);
                            }
                        }
                        else
                        {
                            DataReceivedEventArgs e = new DataReceivedEventArgs();
                            e.Exception = exception;
                            ClientDataReceived?.Invoke(this, e);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }
        }

        private void StopAcceptClient()
        {
            _IsBeginAccept = false;

            Socket.Close();
            Socket = null;
        }

        public void Start()
        {
            _Thread = new Thread(DoAcceptClient);
            _Thread.IsBackground = true;
            _Thread.Start();
        }

        public void Stop()
        {
            StopAcceptClient();
        }

        public int Send(IntPtr handle, string strData)
        {
            try
            {
                Socket clientSocket = _ClientSockets[handle].Socket;
                byte[] bytes = Encoding.UTF8.GetBytes(strData);
                return clientSocket.Send(bytes, bytes.Length, SocketFlags.None);
            }
            catch (KeyNotFoundException)
            {
                return -1;
            }
        }

        public int Send(IntPtr handle, byte[] binaryData)
        {
            try
            {
                Socket clientSocket = _ClientSockets[handle].Socket;
                return clientSocket.Send(binaryData, binaryData.Length, SocketFlags.None);
            }
            catch (KeyNotFoundException)
            {
                return -1;
            }
        }

        internal class AppClientSocket
        {
            internal Socket Socket { get; set; }
            internal string AppId { get; set; }
            internal AppClientSocket(Socket socket, string appId)
            {
                Socket = socket;
                AppId = appId;
            }
        }
    }
}
