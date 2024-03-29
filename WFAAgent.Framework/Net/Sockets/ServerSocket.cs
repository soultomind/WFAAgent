﻿using Newtonsoft.Json.Linq;
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
using WFAAgent.Framework.Application;

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

            bool clientSocketClose = false;
            try
            {
                while (!clientSocketClose)
                {
                    Array.Clear(dataPacketHeaderBuffer, 0, dataPacketHeaderBuffer.Length);
                    receiveBytes = clientSocket.Receive(dataPacketHeaderBuffer, dataPacketHeaderBuffer.Length, SocketFlags.None);
                    if (receiveBytes == 0)
                    {
                        clientSocketClose = true;
                    }
                    else
                    {
                        Exception exception = null;
                        Header header = DataPacket.ToHeader(dataPacketHeaderBuffer);

                        byte[] dataBuffer = null;
                        SocketDataReceiver receiver = new SocketDataReceiver(clientSocket);
                        if (receiver.TryReadSocket(header, out dataBuffer, out exception))
                        {
                            DataReceivedEventArgs e = new DataReceivedEventArgs();
                            e.SetData(header, dataBuffer);

                            if (header.Type == DataContext.AcceptClient)
                            {
                                string appId = header.AppId;
                                int processId = header.ProcessId;

                                AppClientSocket acceptClientSocket = new AppClientSocket(clientSocket, appId);
                                if (_ClientSockets.TryAdd(clientSocket.Handle, acceptClientSocket))
                                {
                                    AcceptClient?.Invoke(
                                        this,
                                        new AcceptClientEventArgs(socket)
                                        {
                                            ClientSocket = clientSocket,
                                            AppId = appId,
                                            ProcessId = processId
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

                            if (e.Exception is AgentTcpClientException)
                            {
                                clientSocketClose = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            // 최초 AcceptClient 패킷 왔을 경우 등록
            AppClientSocket outValue = null;
            if (_ClientSockets.TryRemove(clientSocket.Handle, out outValue))
            {
                DisconnectEventArgs e = new DisconnectEventArgs(clientSocket) { AppId = outValue.AppId };
                DisconnectedClient?.Invoke(this, e);
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

        public int Send(IntPtr handle, DataPacket dataPacket, string data)
        {
            try
            {
                Socket clientSocket = _ClientSockets[handle].Socket;
                if (clientSocket != null && clientSocket.Connected)
                {
                    return ClientSocket.SendSyncClientSocket(clientSocket, dataPacket, data);
                }
                return -1;
            }
            catch (KeyNotFoundException)
            {
                return -1;
            }
        }

        public int Send(IntPtr handle, DataPacket dataPacket, byte[] data)
        {
            try
            {
                Socket clientSocket = _ClientSockets[handle].Socket;
                if (clientSocket != null && clientSocket.Connected)
                {
                    return ClientSocket.SendSyncClientSocket(clientSocket, dataPacket, data);
                }
                return -1;
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
