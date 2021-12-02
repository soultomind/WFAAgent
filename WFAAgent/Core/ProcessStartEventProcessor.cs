using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFAAgent.WebSocket;

namespace WFAAgent.Core
{
    public delegate void StartedEventHandler(object sender, EventArgs e);
    public delegate void ExitedEventHandler(object sender, EventArgs e);
    public class ProcessStartEventProcessor : EventProcessor
    {
        public readonly string DataFileName = "fileName";

        public bool IsMultiProcess { get; set; }
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
                    try
                    {
                        fileName = eventData.Data[DataFileName].ToObject<string>();
                        
                        // TODO: Arguments 정보에 SessionID 정보 보내야함
                        // 서버로 다시 데이터를 전달받을때 세션정보 수신
                        Process process = Process.Start(fileName);

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
