using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WFAAgent.Framework.Application;
using WFAAgent.Framework.Win32;

namespace WFAAgent
{
    public partial class MonitoringForm : Form
    {
        private bool _isCurrentProcessExecuteAdministrator;
        private Process _serverProcess;

        private volatile bool _isProcessAgentServerExited;
        private int _interval = 3000;
        private volatile bool _isServerProcessStartWorker;
        private Thread _serverProcessStartWorker;
        public MonitoringForm(string[] args)
        {
            InitializeComponent();

            InitializeTaskbarTray();

            Text = String.Format("WFAAgent.MonitoringForm Administrator={0}", Toolkit.IsCurrentProcessAdministrator());
        }

        #region Properties
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        internal InfoDialog InfoDialog { get; set; }

        #endregion

        private void InitializeTaskbarTray()
        {
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;
        }

        private void MonitoringForm_Load(object sender, EventArgs e)
        {
            // 윈도우즈 서비스를 활용하여 프로세스가 죽었을때 다시 서비스에서 프로그램을 실행하는 구조 대신에 아래와 같이 
            // 관리자권한(모니터링), 관리자권한,일반(서버) 로 구성하여 개발해보고자 한다.

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
                cmdLineArgs = Main.ExecuteArgs.UserCommandLineArgs;
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
                    if (Main.ExecuteArgs.Execute == Execute.Monitoring)
                    {
                        Process process = CreateExecuteAsAdmin(Application.ExecutablePath, ExecuteContext.ExecuteMonitoringArgs.ToString());
                        if (process != null)
                        {
                            process.Start();
                        }
                        _isCurrentProcessExecuteAdministrator = false;
                    }
                }
            }

            Text = String.Format("WFAAgent.MonitoringForm Administrator={0}", Toolkit.IsCurrentProcessAdministrator());
        }

        private void MonitoringForm_Shown(object sender, EventArgs e)
        {
            Taskbar.RefreshTrayArea();

            if (_isCurrentProcessExecuteAdministrator)
            {
                Dictionary<string, string> dictionary = MakeServerDictionaryArgs();

                Process process = CreateExecuteAsAdmin(Application.ExecutablePath, ExecuteContext.MakeStringArgs(dictionary));
                if (process != null)
                {
                    process.Exited += ServerProcess_Exited;
                    try
                    {
                        process.Start();
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

        private Dictionary<string, string> MakeServerDictionaryArgs()
        {
            // 모니터링 Pid 같이 실행인자로 추가하여 넘겨주기
            Dictionary<string, string> dictionary = ExecuteContext.Server.CopyArgsDirectory;
            dictionary.Add(Constant.ParentProcessId, Process.GetCurrentProcess().Id.ToString());
            return dictionary;
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

        private void StartServerExecuteAsAdmin(string fileName, string arguments)
        {
            Process process = CreateExecuteAsAdmin(fileName, arguments);
            if (process != null)
            {
                process.Exited += ServerProcess_Exited;
                try
                {
                    process.Start();
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
    

        public Process CreateExecuteAsAdmin(string fileName, string arguments)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = fileName;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.Verb = "runas";

                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.OutputDataReceived += Process_OutputDataReceived;
                process.ErrorDataReceived += Process_ErrorDataReceived;
                process.EnableRaisingEvents = true;

                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
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
               
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            
        }

        private void TrayNotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (InfoDialog == null)
            {
                InfoDialog = new InfoDialog();
                InfoDialog.Width += 20;
                InfoDialog.Text = String.Format("{0}.{1}", Application.ProductName, Execute.Monitoring.ToString());
            }

            if (Main.HasOpenForm(InfoDialog.Text))
            {
                InfoDialog.Visible = true;
            }
            else
            {
                InfoDialog.Show();
            }
            InfoDialog.Activate();
        }

        private void ShowConfigDlgToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
