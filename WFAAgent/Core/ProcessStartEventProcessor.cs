using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFAAgent.Framework.Application;
using WFAAgent.Framework.Utilities;
using WFAAgent.WebSocket;

namespace WFAAgent.Core
{
    public delegate void StartedEventHandler(object sender, EventArgs e);
    public delegate void ExitedEventHandler(object sender, EventArgs e);
    public class ProcessStartEventProcessor : EventProcessor
    {
        public readonly string DataFileName = "fileName";
        public readonly string DataUseCallbackData = "useCallbackData";

        public bool IsMultiProcess { get; set; }
        public int AgentTcpServerPort { get; set; }
        public List<ProcessStartInfo> ProcessList { get; set; }
        public ProcessStartInfo ProcessStartInfo { get; private set; }


        public event StartedEventHandler Started;
        public event ExitedEventHandler Exited;

        public override void DoProcess(EventData eventData)
        {
            if (IsMultiProcess)
            {
                // TODO: 추후에는 다중 프로세스 처리 필요함!
            }
            else
            {
                if (ProcessStartInfo == null)
                {
                    string fileName = String.Empty;
                    bool useCallbackData = false;
                    try
                    {
                        fileName = eventData.Data[DataFileName].ToObject<string>();
                        if (eventData.Data.ContainsKey(DataUseCallbackData))
                        {
                            useCallbackData = eventData.Data[DataUseCallbackData].ToObject<bool>();
                        }
                        
                        
                        Process process = null;
                        if (useCallbackData)
                        {
                            string sessionID = eventData.SessionID;
                            int agentTcpServerPort = AgentTcpServerPort;

                            JObject argObj = new JObject();
                            argObj.Add(Context.ArgSessionID, sessionID);
                            argObj.Add(Context.ArgAgentTcpServerPort, agentTcpServerPort);
                            string arguments = ConvertUtility.Base64Encode(argObj.ToString());
                            process = Process.Start(fileName, arguments);
                        }
                        else
                        {
                            process = Process.Start(fileName);
                        }

                        ProcessStartInfo = new ProcessStartInfo() {
                            FileName = fileName,
                            Process = process,
                            SessionID = eventData.SessionID
                        };

                        // Exited Event Enabled
                        ProcessStartInfo.Process.EnableRaisingEvents = true;
                        ProcessStartInfo.Process.Exited += Process_Exited;

                        Started?.Invoke(ProcessStartInfo, EventArgs.Empty);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else
                {
                    // TODO: 창만 User32.ShowWindow 처리
                }
            }
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            if (IsMultiProcess)
            {
                int id = (sender as Process).Id;
                foreach (ProcessStartInfo item in ProcessList)
                {
                    if (item.Process.Id == id)
                    {
                        Exited?.Invoke(item, e);

                        item.Close();
                        ProcessList.Remove(item);
                        break;
                    }
                }
            }
            else
            {
                Exited?.Invoke(ProcessStartInfo, e);
                ProcessStartInfo.Close();
            }
        }
    }
}
