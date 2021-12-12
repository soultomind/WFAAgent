using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFAAgent.WebSocket;

namespace WFAAgent.Core
{
    public class ProcessInfo
    {
        private readonly string PropProcessId = "processId";
        private readonly string PropProcessName = "processName";
        private readonly string PropFileName = "fileName";
        private readonly string PropStartTime = "startTime";
        private readonly string PropExitTime = "exitTime";
        
        public string FileName { get; set; }
        public Process Process { get; set; }
        public string SessionID { get; set; }

        public ProcessInfo()
        {
            Process = null;
            SessionID = String.Empty;
        }

        public void Close()
        {
            if (Process != null)
            {
                Process.Close();
                Process = null;

                SessionID = null;
            }
        }

        private JObject ToCommonJson()
        {
            JObject o = new JObject();
            o.Add(PropFileName, FileName);
            o.Add(PropProcessName, Process.ProcessName);
            o.Add(PropProcessId, Process.Id);
            return o;
        }
        public JObject ToStartedJson()
        {
            JObject o = ToCommonJson();
            o.Add(PropStartTime, Process.StartTime.ToString());
            o.Add(EventConstant.EventName, EventConstant.ProcessStartedEvent);
            return o;
        }

        public JObject ToExitedJson()
        {
            JObject o = ToCommonJson();
            o.Add(PropExitTime, Process.ExitTime.ToString());
            o.Add(EventConstant.EventName, EventConstant.ProcessExitedEvent);
            return o;
        }
    }
}
