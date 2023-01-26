using log4net;
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
    public class ProcessStartDataEventProcessor : EventProcessor
    {
        public readonly string StartInfoObj = "startInfo";
        public readonly string StartInfoObj_FileNameProp = "fileName";
        public readonly string StartInfoObj_UseCallbackData = "useCallbackData";
        
        public readonly string ExecuteDataObj = "executeData";
        public readonly string ExecuteDataObj_Port = "port";

        private static ILog Log = LogManager.GetLogger(typeof(AgentManager));

        public bool IsMultiProcess { get; set; }
        public List<ProcessInfo> ProcessList { get; set; }
        public ProcessInfo SingleProcessInfo { get; private set; }


        public event EventHandler<StartAgentTcpServerEventArgs> StartAgentTcpServer;
        public event EventHandler<ProcessStartedEventArgs> ProcessStarted;
        public event EventHandler<ProcessExitedEventArgs> ProcessExited;
        public event EventHandler<ProcessStartedSendDataEventArgs> ProcessStartedSendData;

        private static object _Lock = new object();

        public override void DoProcess(ClientEventData clientEventData)
        {
            if (IsMultiProcess)
            {
                // TODO: 추후에는 다중 프로세스 처리 필요함!
                
                // AppId 가 키가 되어 프로세스정보저장
            }
            else
            {
                lock (_Lock)
                {
                    if (SingleProcessInfo == null)
                    {
                        try
                        {
                            JObject startInfoObj = null, executeDataObj = null;
                            
                            startInfoObj = clientEventData.Data[StartInfoObj] as JObject;

                            bool useCallbackData = (startInfoObj[StartInfoObj_UseCallbackData] != null) ?
                                startInfoObj[StartInfoObj_UseCallbackData].ToObject<bool>() : false;

                            Process process = null;
                            string fileName = startInfoObj[StartInfoObj_FileNameProp].ToObject<string>();
                            if (useCallbackData)
                            {
                                string sessionID = clientEventData.AppId;

                                executeDataObj = clientEventData.Data[ExecuteDataObj] as JObject;
                                int agentTcpServerPort = executeDataObj[ExecuteDataObj_Port].ToObject<int>();

                                // TODO: 클라이언트와 통신하는 서버소켓 시작 AgentTcpServerPort 정보로 (웹 클라이언트)
                                StartAgentTcpServer?.Invoke(this, new StartAgentTcpServerEventArgs(agentTcpServerPort));

                                Process newProcess = new Process();

                                JObject argObj = new JObject();
                                argObj.Add(Constant.AppID, sessionID);
                                argObj.Add(Constant.AgentTcpServerPort, agentTcpServerPort);
                                argObj.Add(Constant.UseCallBackData, true);
                                //argObj.Add(Constant.ProcessId, newProcess.Id);

                                string arguments = ConvertUtility.Base64Encode(argObj.ToString());
                                newProcess.StartInfo = new ProcessStartInfo(fileName, arguments);
                                newProcess.StartInfo.UseShellExecute = false;
                                newProcess.StartInfo.Verb = "runas";

                                newProcess.StartInfo.RedirectStandardError = true;
                                newProcess.ErrorDataReceived += Process_ErrorDataReceived;
                                newProcess.StartInfo.RedirectStandardOutput = true;
                                newProcess.OutputDataReceived += Process_OutputDataReceived;

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
                                SetProcessEvent(process);

                                SingleProcessInfo = new ProcessInfo()
                                {
                                    FileName = fileName,
                                    Process = process,
                                    AppId = clientEventData.AppId
                                };

                                ProcessStarted?.Invoke(this, new ProcessStartedEventArgs(SingleProcessInfo));

                                if (useCallbackData && clientEventData.Data[ExecuteDataObj] != null)
                                {
                                    string data = clientEventData.Data[ExecuteDataObj].ToString();
                                    ProcessStartedSendData?.Invoke(this, new ProcessStartedSendDataEventArgs(data)
                                    {
                                        ProcessInfo = SingleProcessInfo
                                    });
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex);
                        }
                    }
                    else
                    {
                        // TODO: 창만 User32.ShowWindow 처리
                    }
                }
            }
        }

        private void SetProcessEvent(Process process)
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
            process.Exited += Process_Exited;
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e != null && !String.IsNullOrEmpty(e.Data))
            {
                AgentErrorData o = JsonConvert.DeserializeObject<AgentErrorData>(e.Data);
                Toolkit.TraceWriteLine(String.Format("Pid={0}, Data={1}", o.ProcessId, o.Data));
            }
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e != null && !String.IsNullOrEmpty(e.Data))
            {
                AgentOutputData o = JsonConvert.DeserializeObject<AgentOutputData>(e.Data);
                Toolkit.TraceWriteLine(String.Format("Pid={0}, Data={1}", o.ProcessId, o.Data));
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
                        ProcessExited?.Invoke(this, new ProcessExitedEventArgs(item));

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
                    ProcessExited?.Invoke(this, new ProcessExitedEventArgs(SingleProcessInfo));
                    SingleProcessInfo.Close();
                    SingleProcessInfo = null;
                }
            }
        }
    }
}
