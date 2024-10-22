using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using WFAAgent.Core;

namespace WFAAgent
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // 운영

            // Environment.GetCommandLineArgs() 메서드는 실행파일 위치까지 포함하여 넘어오므로 아래 코드 추가
            ExecuteContext.ExecCommandLineArgs = args;

            Toolkit.TraceWriteLine("============= Start Main ");
            if (args != null && args.Length > 0)
            {
                for (int index = 0; index < args.Length; index++)
                {
                    Toolkit.TraceWrite(String.Format("{0}:{1}", (index + 1), args[index]));
                }
            }
            Toolkit.TraceWriteLine(" Start Main =============");

            if (args.Length == 0)
            {
                if (Debugger.IsAttached)
                {
                    args = new string[] { ExecuteContext.ExecuteServerArgs };
                }
                else
                {
                    args = new string[] { ExecuteContext.ExecuteMonitoringArgs };
                }
            }
            else
            {
#if DEBUG
                bool isCurrentProcessExecuteAdministrator = Toolkit.IsCurrentProcessExecuteAdministrator();
                MessageBox.Show(String.Format("IsCurrentProcessExecuteAdministrator={0}, Args.Length={1}, Args[0]={2}",
                    isCurrentProcessExecuteAdministrator, args.Length, args[0]), "DEBUG CONDITION COMPILE");
#endif
            }
            new Main().Run(args);
        }
    }
}
