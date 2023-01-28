using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WFAAgent.Core;
using WFAAgent.Framework.Application;

namespace WFAAgent.Monitoring
{
    /// <summary>
    /// 서버 프로세스 감시자
    /// </summary>
    internal static class ServerProcessWatcher
    {
        public static bool IsCurrentServerProcessExecuteAdministrator { get; internal set; }
        public static Process ServerProcess { get; internal set; }

        public static bool IsServerProcessExited
        {
            get { return _serverProcessExited; }
            private set { _serverProcessExited = value; }
        }
        private static volatile bool _serverProcessExited;

        internal static void FormLoadEventProcess()
        {
            bool isCurrentServerProcessExecuteAdministrator = Toolkit.IsCurrentProcessExecuteAdministrator();
            IsCurrentServerProcessExecuteAdministrator = isCurrentServerProcessExecuteAdministrator;

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

            if (isCurrentServerProcessExecuteAdministrator)
            {
                
            }
            else
            {
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

                        IsCurrentServerProcessExecuteAdministrator = false;
                    }
                }
            }
        }

        internal static Process CreateExecuteAsAdministrator(string fileName, string arguments, bool useShellExecute)
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
                Toolkit.TraceWriteLine(ex);
                return null;
            }
        }

        internal static void FormShownEventProcess()
        {
            if (IsCurrentServerProcessExecuteAdministrator)
            {

            }
        }

        private static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Toolkit.TraceWriteLine("Process_OutputDataReceived=" + e.Data);
        }

        private static void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Toolkit.TraceWriteLine("Process_ErrorDataReceived=" + e.Data);
        }
    }
}
