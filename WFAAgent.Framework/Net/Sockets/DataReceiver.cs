using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework.Net.Sockets
{
    public interface DataReceiver
    {
        bool TryReadData(out byte[] data, out Exception exception);
    }
}
