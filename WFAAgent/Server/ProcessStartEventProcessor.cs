using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using WFAAgent.Core;
using WFAAgent.Framework.Application;
using WFAAgent.Framework.Utilities;
using WFAAgent.Server;

namespace WFAAgent.Server
{
    public delegate void StartedEventHandler(object sender, EventArgs e);
    public delegate void ExitedEventHandler(object sender, EventArgs e);
    public class ProcessStartEventProcessor : EventProcessor
    {
        public readonly string DataFileName = "fileName";
        public readonly string DataUseCallbackData = "useCallbackData";

        public bool IsMultiProcess { get; set; }
        public int AgentTcpServerPort { get; set; }
        public List<ProcessInfo> ProcessList { get; set; }
        public ProcessInfo ProcessInfo { get; private set; }


        public event StartedEventHandler Started;
        public event ExitedEventHandler Exited;

        private static object _Lock = new object();

        public override void DoProcess(EventData eventData)
        {
            if (IsMultiProcess)
            {
                // TODO: 추후에는 다중 프로세스 처리 필요함!
            }
            else
            {
                lock (_Lock)
                {
                    if (ProcessInfo == null)
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
                            string arguments = null;
                            if (useCallbackData)
                            {
                                string sessionID = eventData.AppId;
                                int agentTcpServerPort = AgentTcpServerPort;

                                Process newProcess = new Process();

                                JObject argObj = new JObject();
                                argObj.Add(Constant.AppID, sessionID);
                                argObj.Add(Constant.AgentTcpServerPort, agentTcpServerPort);
                                //argObj.Add(Constant.ProcessId, newProcess.Id);

                                arguments = ConvertUtility.Base64Encode(argObj.ToString());
                                newProcess.StartInfo = new ProcessStartInfo(fileName, arguments);
                                newProcess.StartInfo.UseShellExecute = false;
                                newProcess.StartInfo.Verb = "runas";

                                newProcess.StartInfo.RedirectStandardError = true;
                                if (newProcess.StartInfo.RedirectStandardError)
                                {
                                    newProcess.ErrorDataReceived += Process_ErrorDataReceived;
                                }

                                newProcess.StartInfo.RedirectStandardOutput = true;
                                if (newProcess.StartInfo.RedirectStandardOutput)
                                {
                                    newProcess.OutputDataReceived += Process_OutputDataReceived;
                                }

                                if (newProcess.Start())
                                {
                                    process = newProcess;
                                }
                            }
                            else
                            {
                                process = Process.Start(fileName);
                            }

                            if (process != null)
                            {
                                if (process.StartInfo.RedirectStandardError)
                                {
                                    process.BeginErrorReadLine();
                                }
                                
                                if (process.StartInfo.RedirectStandardOutput)
                                {
                                    process.BeginOutputReadLine();
                                }

                                process.EnableRaisingEvents = true;
                                ProcessInfo = new ProcessInfo()
                                {
                                    FileName = fileName,
                                    Process = process,
                                    SessionId = eventData.AppId
                                };

                                // Exited Event Enabled
                                ProcessInfo.Process.EnableRaisingEvents = true;
                                ProcessInfo.Process.Exited += Process_Exited;

                                Started?.Invoke(ProcessInfo, EventArgs.Empty);
                            }
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
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            AgentErrorData o = JsonConvert.DeserializeObject<AgentErrorData>(e.Data);
            Toolkit.TraceWriteLine(String.Format("Pid={0}, Data={1}", o.ProcessId, o.Data));
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            AgentOutputData o = JsonConvert.DeserializeObject<AgentOutputData>(e.Data);
            Toolkit.TraceWriteLine(String.Format("Pid={0}, Data={1}", o.ProcessId, o.Data));
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            if (IsMultiProcess)
            {
                int id = (sender as Process).Id;
                foreach (ProcessInfo item in ProcessList)
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
                lock (_Lock)
                {
                    Exited?.Invoke(ProcessInfo, e);
                    ProcessInfo.Close();
                    ProcessInfo = null;
                }
            }
        }
    }
}
