using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WFAAgent.Core;
using WFAAgent.Framework.Application;
using WFAAgent.Message;

namespace WFAAgent.Monitoring
{
    /// <summary>
    /// 서버 프로세스 감시자
    /// </summary>
    internal class ServerProcessWatcher
    {
        public event EventHandler<MessageItemEventArgs> MessageItem;

        public Process ServerProcess { get; internal set; }

        public int ServerProcessWatcherInterval
        {
            get { return _serverProcessWatcherInterval; }
            set
            {
                if (value > 0)
                {
                    _serverProcessWatcherInterval = value;
                }
            }
        }
        private int _serverProcessWatcherInterval = 3000;
        public Thread ServerProcessStarter { get; private set; }

        public bool IsServerProcessStarterAlive
        {
            get { return _serverProcessStarterAlive; }
            private set { _serverProcessStarterAlive = value; }
        }
        private volatile bool _serverProcessStarterAlive;

        public bool IsServerProcessExited
        {
            get { return _serverProcessExited; }
            private set { _serverProcessExited = value; }
        }
        private volatile bool _serverProcessExited;

        private void OnMessageItem(MessageItemEventArgs e)
        {
            MessageItem?.Invoke(this, e);
        }

        private void OnDebugMessageItem(string message)
        {
            MessageItemEventArgs e = new MessageItemEventArgs();
            e.MessageItem = new MessageItem() { LogLevel = LogLevel.Debug, Message = message };
            OnMessageItem(e);
        }

        private void OnInfoMessageItem(string message)
        {
            MessageItemEventArgs e = new MessageItemEventArgs();
            e.MessageItem = new MessageItem() { LogLevel = LogLevel.Info, Message = message };
            OnMessageItem(e);
        }

        private void OnWarnMessageItem(string message)
        {
            MessageItemEventArgs e = new MessageItemEventArgs();
            e.MessageItem = new MessageItem() { LogLevel = LogLevel.Warn, Message = message };
            OnMessageItem(e);
        }

        private void OnWarnMessageItem(string message, Exception exception)
        {
            MessageItemEventArgs e = new MessageItemEventArgs();
            e.MessageItem = new MessageItem() { LogLevel = LogLevel.Warn, Message = message, Exception = exception };
            OnMessageItem(e);
        }

        private void OnErrorMessageItem(string message)
        {
            MessageItemEventArgs e = new MessageItemEventArgs();
            e.MessageItem = new MessageItem() { LogLevel = LogLevel.Error, Message = message };
            OnMessageItem(e);
        }

        private void OnErrorMessageItem(string message, Exception exception)
        {
            MessageItemEventArgs e = new MessageItemEventArgs();
            e.MessageItem = new MessageItem() { LogLevel = LogLevel.Error, Message = message, Exception = exception };
            OnMessageItem(e);
        }

        internal void FormLoadEventProcess(bool isCurrentProcessExecuteAdministrator)
        {
            if (isCurrentProcessExecuteAdministrator == false)
            {
                Process currentProcess = Process.GetCurrentProcess();
                bool useShellExecute = currentProcess.StartInfo.UseShellExecute;

#if MONITORING_DEBUG
            
                MessageBox.Show(String.Format("Monitoring UseShellExecute={0}", useShellExecute), "MONITORING_DEBUG CONDITION COMPILE");
#else

                OnDebugMessageItem(String.Format("Monitoring UseShellExecute={0}", useShellExecute));
#endif

                string[] commandLineArgs = null;
                if (ExecuteContext.ExecCommandLineArgs.Length == 0)
                {
                    // 최초 처음 실행 사용자 인자
                    commandLineArgs = Main.ExecuteContext.UserCommandLineArgs;
                }
                else
                {
                    // 그 이후에 실행 인자
                    commandLineArgs = ExecuteContext.ExecCommandLineArgs;
                }

                if (commandLineArgs != null && commandLineArgs.Length == 1)
                {
                    if (Main.ExecuteContext.Execute == Execute.Monitoring)
                    {
                        // 최초 관리자 권한 모니터링 실행
                        // 처음 실행이기 때문에 Redirect Output, Redirect Error 이벤트를 등록 하지 않음

                        Process process = CreateExecuteAsAdministrator(
                            Application.ExecutablePath,
                            ExecuteContext.ExecuteMonitoringArgs.ToString(),
                            true
                        );

                        if (process != null)
                        {
                            process.Start();
                        }
                    }
                }
            }
        }

        internal void FormShownEventProcess()
        {
            Dictionary<string, string> dictionary = ExecuteContext.MakeServerDictionaryArgs();

            Process process = CreateExecuteAsAdministrator(Application.ExecutablePath, ExecuteContext.MakeStringArgs(dictionary), false);
            if (process != null)
            {
                process.Exited += ServerProcess_Exited;
                try
                {
                    StartAndIfFalseUseShellExecuteOutputErrorReadLine(process);

                    ServerProcess = process;

                    string text = String.Format("({0}={1}) 프로세스가 시작 되었습니다.", process.ProcessName, process.Id);
                    OnDebugMessageItem(text);

                    IsServerProcessExited = false;

                    StartServerProcessStarter();
                }
                catch (Exception ex)
                {
                    OnErrorMessageItem("프로세스 실행에 실패하였습니다.", ex);

                    IsServerProcessExited = true;
                }
            }
        }

        internal void StartServerProcessStarter()
        {
            IsServerProcessStarterAlive = true;

            ServerProcessStarter = new Thread(ServerProcess_Started);
            ServerProcessStarter.IsBackground = true;
            ServerProcessStarter.Start(ServerProcessWatcherInterval);
        }

        internal void StopServerProcessStarter()
        {
            IsServerProcessStarterAlive = false;
            if (ServerProcessStarter != null)
            {
                ServerProcessStarter.Join();
            }

            ServerProcessStarter = null;
        }

        private Process CreateExecuteAsAdministrator(string fileName, string arguments, bool useShellExecute)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = fileName;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.UseShellExecute = useShellExecute;
                process.StartInfo.Verb = "runas";

                if (!process.StartInfo.UseShellExecute)
                {
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.RedirectStandardOutput = true;

                    process.ErrorDataReceived += Process_ErrorDataReceived;
                    process.OutputDataReceived += Process_OutputDataReceived;
                }

                process.EnableRaisingEvents = true;
                return process;
            }
            catch (Exception ex)
            {
                OnErrorMessageItem("프로세스 생성중 오류가 발생하였습니다.", ex);
                return null;
            }
        }


        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            OnDebugMessageItem("Process_OutputDataReceived=" + e.Data);
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            OnDebugMessageItem("Process_ErrorDataReceived=" + e.Data);
        }

        private void ServerProcess_Started(object argument)
        {
            int watcherInterval = (int)argument;

            Dictionary<string, string> dictionary = ExecuteContext.MakeServerDictionaryArgs();

            while (IsServerProcessStarterAlive)
            {
                Thread.Sleep(watcherInterval);
                if (IsServerProcessExited)
                {
                    OnDebugMessageItem("서버 프로세스가 종료되어 다시 시작합니다.");
                    StartServerExecuteAsAdministrator(Application.ExecutablePath, ExecuteContext.MakeStringArgs(dictionary));
                    OnDebugMessageItem("성공적으로 서버 프로세스가 시작 되었습니다.");
                }
            }
        }

        private void StartServerExecuteAsAdministrator(string fileName, string arguments, bool useShellExecute = false)
        {
            Process process = CreateExecuteAsAdministrator(fileName, arguments, useShellExecute);
            if (process != null)
            {
                process.Exited += ServerProcess_Exited;
                try
                {
                    StartAndIfFalseUseShellExecuteOutputErrorReadLine(process);

                    ServerProcess = process;

                    string text = String.Format("({0}={1}) 서버프로세스가 시작 되었습니다.", process.ProcessName, process.Id);
                    OnDebugMessageItem(text);

                    IsServerProcessExited = false;
                }
                catch (Exception ex)
                {
                    OnErrorMessageItem(ex.Message, ex);
                }
            }
        }

        private void ServerProcess_Exited(object sender, EventArgs e)
        {
            // TODO: 해당 이벤트 발생시에
            //       약 2-3초 후에 다시 실행 
            //       스레드로 계속 감지 하여 플래그값 변경시 이벤트 발생시키기

            Process process = sender as Process;
            string text = String.Format("({0}={1}) 프로세스가 종료 되었습니다.", process.ProcessName, process.Id);
            OnDebugMessageItem(text);

            IsServerProcessExited = true;
        }

        private  void StartAndIfFalseUseShellExecuteOutputErrorReadLine(Process process)
        {
            process.Start();
            if (!process.StartInfo.UseShellExecute)
            {
                if (process.StartInfo.RedirectStandardError)
                {
                    process.BeginErrorReadLine();
                }

                if (process.StartInfo.RedirectStandardOutput)
                {
                    process.BeginOutputReadLine();
                }
            }
        }
    }
}
