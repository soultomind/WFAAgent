using System;
using System.Collections.Generic;
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

        private bool _IsBeginAccept;
        public ServerSocket(string ipString, int port)
            : base(ipString, port)
        {
            
        }

        public bool Bind()
        {
            try
            {
                Initialize();

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

        public void Start()
        {
            while (_IsBeginAccept)
            {

            }
            Socket.BeginAccept(StartAcceptClientWorker, Socket);
        }

        

        private void StartAcceptClientWorker(IAsyncResult asyncResult)
        {
            Socket socket = asyncResult.AsyncState as Socket;

            Socket clientSocket = socket.EndAccept(asyncResult);
        }

        public void Stop()
        {
            StopAcceptClientWorker();
        }

        private void StopAcceptClientWorker()
        {
            Socket.Disconnect(false);
        }
    }
}
