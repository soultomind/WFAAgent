using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using WFAAgent.Server;

namespace WFAAgent.Server
{
    public class ProcessInfo : IDisposable
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


        private bool _disposed;

        public ProcessInfo()
        {
            Process = null;
            AppId = String.Empty;
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!_disposed)
                {
                    if (Process != null)
                    {
                        Process.Dispose();
                        Process = null;
                    }
                    _disposed = true;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
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
