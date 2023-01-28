using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using WFAAgent.Server;

namespace WFAAgent.Server
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
        public string AppId { get; set; }
        public IntPtr SocketHandle { get; internal set; }

        public ProcessInfo()
        {
            Process = null;
            AppId = String.Empty;
        }

        public void Close()
        {
            if (Process != null)
            {
                Process.Close();
                Process = null;
            }
        }

        private JObject ToCommonJson(JObject o)
        {
            o.Add(PropFileName, FileName);
            o.Add(PropProcessName, Process.ProcessName);
            o.Add(PropProcessId, Process.Id);
            return o;
        }
        public JObject ToStartedJson()
        {
            JObject o = new JObject();
            o.Add(EventConstant.EventName, EventConstant.ProcessStartedEvent);
            o = ToCommonJson(o);
            o.Add(PropStartTime, Process.StartTime.ToString());
            return o;
        }

        public JObject ToExitedJson()
        {
            JObject o = new JObject();
            o.Add(EventConstant.EventName, EventConstant.ProcessExitedEvent);
            o = ToCommonJson(o);
            o.Add(PropExitTime, Process.ExitTime.ToString());
            return o;
        }
    }
}
