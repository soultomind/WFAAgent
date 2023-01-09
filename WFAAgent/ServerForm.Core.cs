using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent
{
    partial class ServerForm
    {
        internal void ServerForm_SendMonitoringOutputData(string data)
        {
            if (Console.IsOutputRedirected)
            {
                Console.Out.WriteLine(data);
            }
        }

        internal void ServerForm_SendMonitoringErrorData(string data)
        {
            if (Console.IsErrorRedirected)
            {
                Console.Error.WriteLine(data);
            }
        }
    }
}
