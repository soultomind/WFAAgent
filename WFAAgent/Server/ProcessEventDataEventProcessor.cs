using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Server
{
    
    public class ProcessEventDataEventProcessor : EventProcessor
    {
        private static ILog Log = LogManager.GetLogger(typeof(AgentManager));

        public event EventHandler<ProcessEventSendDataEventArgs> ProcessEventSendData;

        public override void DoProcess(ClientEventData clientEventData)
        {
            JObject data = clientEventData.Data;

            ProcessEventSendData?.Invoke(this, new ProcessEventSendDataEventArgs()
            {
                AppId = clientEventData.AppId,
                Data = data.ToString()
            });
        }
    }
}
