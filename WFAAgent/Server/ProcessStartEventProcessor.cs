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
                            if (useCallbackData)
                            {
                                string sessionID = eventData.AppId;
                                int agentTcpServerPort = AgentTcpServerPort;

                                JObject argObj = new JObject();
                                argObj.Add(Constant.AppID, sessionID);
                                argObj.Add(Constant.AgentTcpServerPort, agentTcpServerPort);
                                string arguments = ConvertUtility.Base64Encode(argObj.ToString());

                                ProcessStartInfo psi = new ProcessStartInfo(fileName, arguments);
                                process = Process.Start(fileName, arguments);

                                /*
                                process.StartInfo.FileName = fileName;
                                process.StartInfo.Arguments = arguments;
                                process.StartInfo.UseShellExecute = true;
                                process.StartInfo.Verb = "runas";

                                process.StartInfo.RedirectStandardOutput = true;
                                process.OutputDataReceived += Process_OutputDataReceived;

                                process.StartInfo.RedirectStandardError = true;
                                process.ErrorDataReceived += Process_ErrorDataReceived;

                                process.EnableRaisingEvents = true;

                                process.BeginErrorReadLine();
                                process.BeginOutputReadLine();
                                */
                            }
                            else
                            {
                                process = Process.Start(fileName);
                            }

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
