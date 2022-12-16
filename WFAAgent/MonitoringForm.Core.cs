using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WFAAgent.Framework.Application;

namespace WFAAgent
{
    partial class MonitoringForm
    {
        private bool _isCurrentProcessExecuteAdministrator;
        private Process _serverProcess;

        private volatile bool _isProcessAgentServerExited;
        private int _interval = 3000;
        private volatile bool _isServerProcessStartWorker;
        private Thread _serverProcessStartWorker;

        private Dictionary<string, string> MakeServerDictionaryArgs()
        {
            // 모니터링 Pid 같이 실행인자로 추가하여 넘겨주기
            Dictionary<string, string> dictionary = ExecuteContext.Server.CopyArgsDirectory;
            dictionary.Add(Constant.ParentProcessId, Process.GetCurrentProcess().Id.ToString());
            return dictionary;
        }

        private void LoadProcess()
        {
            // 윈도우즈 서비스를 활용하여 프로세스가 죽었을때 다시 서비스에서 프로그램을 실행하는 구조 대신에 아래와 같이 
            // 관리자권한(모니터링), 관리자권한,일반(서버) 로 구성하여 개발해보고자 한다.

            // 실제 테스트를 하려면 샐힝 파일 더블클릭 실행 (VS에서 Ctrl + F5 ) X
            bool isCurrentProcessExecuteAdministrator = Toolkit.IsCurrentProcessAdministrator();
            _isCurrentProcessExecuteAdministrator = isCurrentProcessExecuteAdministrator;

            string[] cmdLineArgs = null;
            if (Debugger.IsAttached && isCurrentProcessExecuteAdministrator && ExecuteContext.ExecCommandLineArgs.Length == 0)
            {
                isCurrentProcessExecuteAdministrator = false;
            }

            if (ExecuteContext.ExecCommandLineArgs.Length == 0)
            {
                // 최초 처음 실행 사용자 인자
                cmdLineArgs = Main.ExecuteContext.UserCommandLineArgs;
            }
            else
            {
                // 그 후에 실행인자
                cmdLineArgs = ExecuteContext.ExecCommandLineArgs;
            }

            if (isCurrentProcessExecuteAdministrator)
            {
                // Do nothing
            }
            else
            {
                // 최초 처음 실행 사용자 인자
                if ((cmdLineArgs != null && cmdLineArgs.Length == 1))
                {
                    if (Main.ExecuteContext.Execute == Execute.Monitoring)
                    {
                        // 최초 관리자 권한 모니터링 실행
                        // 처음 실행이기때문에 Redirect Output, Error 이벤트 등록 하지 않음
                        Process process = CreateExecuteAsAdmin(Application.ExecutablePath, ExecuteContext.ExecuteMonitoringArgs.ToString(), true);
                        if (process != null)
                        {
                            process.Start();
                        }

                        _isCurrentProcessExecuteAdministrator = false;
                    }
                }
            }
        }

        private void ShownProcess()
        {
            if (_isCurrentProcessExecuteAdministrator)
            {
                Dictionary<string, string> dictionary = MakeServerDictionaryArgs();

                Process process = CreateExecuteAsAdmin(Application.ExecutablePath, ExecuteContext.MakeStringArgs(dictionary));
                if (process != null)
                {
                    process.Exited += ServerProcess_Exited;
                    try
                    {
                        StartAndIfFalseUseShellExecuteOutputErrorReadLine(process);

                        _serverProcess = process;

                        string text = String.Format("({0}={1}) 프로세스가 시작 되었습니다.", process.ProcessName, process.Id);
                        Toolkit.TraceWrite(text);

                        _isProcessAgentServerExited = false;

                        _serverProcessStartWorker = new Thread(ServerProcess_Started);
                        _serverProcessStartWorker.IsBackground = true;
                        _serverProcessStartWorker.Start(_interval);
                    }
                    catch (Exception ex)
                    {
                        Toolkit.TraceWriteLine(ex.Message);
                        Toolkit.TraceWriteLine(ex.StackTrace);

                        // TODO: Process.Start() 계속적으로 예외 발생시 예외처리 필요
                        _isProcessAgentServerExited = true;
                    }
                }
            }
            else
            {
                // 일반 실행일때 해당 프로그램은 종료.
                Close();
            }
        }

        private void ServerProcess_Started(object argument)
        {
            _isServerProcessStartWorker = true;

            int interval = (int)argument;

            Dictionary<string, string> dictionary = MakeServerDictionaryArgs();

            while (_isServerProcessStartWorker)
            {
                Thread.Sleep(interval);
                if (_isProcessAgentServerExited)
                {
                    StartServerExecuteAsAdmin(Application.ExecutablePath, ExecuteContext.MakeStringArgs(dictionary));
                }
            }
        }

        private void StartServerExecuteAsAdmin(string fileName, string arguments, bool useShellExecute = false)
        {
            Process process = CreateExecuteAsAdmin(fileName, arguments, useShellExecute);
            if (process != null)
            {
                process.Exited += ServerProcess_Exited;
                try
                {
                    StartAndIfFalseUseShellExecuteOutputErrorReadLine(process);

                    _serverProcess = process;

                    string text = String.Format("({0}={1}) 프로세스가 시작 되었습니다.", process.ProcessName, process.Id);
                    Toolkit.TraceWrite(text);

                    _isProcessAgentServerExited = false;
                }
                catch (Exception ex)
                {
                    Toolkit.TraceWriteLine(ex.Message);
                    Toolkit.TraceWriteLine(ex.StackTrace);

                    // TODO: Process.Start() 계속적으로 예외 발생시 예외처리 필요
                    _isProcessAgentServerExited = true;
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
            Toolkit.TraceWrite(text);

            _isProcessAgentServerExited = true;
        }

        private void StartAndIfFalseUseShellExecuteOutputErrorReadLine(Process process)
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

        private Process CreateExecuteAsAdmin(string fileName, string arguments, bool useShellExecute = false)
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

                    if (process.StartInfo.RedirectStandardError)
                    {
                        process.ErrorDataReceived += Process_ErrorDataReceived;
                    }

                    if (process.StartInfo.RedirectStandardOutput)
                    {
                        process.OutputDataReceived += Process_OutputDataReceived;
                    }
                }

                process.EnableRaisingEvents = true;
                return process;
            }
            catch (Exception ex)
            {
                Toolkit.TraceWriteLine(ex.Message);
                Toolkit.TraceWriteLine(ex.StackTrace);
                return null;
            }
        }
        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Toolkit.TraceWriteLine("Process_OutputDataReceived=" + e.Data);
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Toolkit.TraceWriteLine("Process_ErrorDataReceived=" + e.Data);
        }
    }
}
