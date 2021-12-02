using SuperSocket.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.WebSocket
{
    public static class WebSocketServerExtension
    {
        public static string ToInfoString(this WebSocketServer server)
        {
            return new StringBuilder()
                .AppendFormat("Name={0},Port={1}",server.Config.Name, server.Config.Port)
                .ToString();
        }
    }
}
