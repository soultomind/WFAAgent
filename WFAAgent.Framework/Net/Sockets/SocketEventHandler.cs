using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework.Net.Sockets
{
    public delegate void ListenEventHandler(object sender, ListenEventArgs e);
    public delegate void AcceptClientEventHandler(object sender, AcceptClientEventArgs e);

    public delegate void DisconnectedEventHandler(object sender, DisconnectEventArgs e);
    public delegate void ConnectedEventhandler(object sender, ConnectedEventArgs e);
    public delegate void AsyncSendCompletedEventHandler(object sender, AsyncSendSocketEventArgs e);

    public delegate void DataReceivedEventhandler(object sender, DataReceivedEventArgs e);
}
