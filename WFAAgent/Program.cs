using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

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
            // Environment.GetCommandLineArgs() 메서드는 실행파일 위치까지 포함하여 넘어오므로 아래 코드 추가
            ExecuteContext.ExecCommandLineArgs = args;

            Toolkit.TraceWrite("============= Start Main ");
            if (args != null && args.Length > 0)
            {
                for (int index = 0; index < args.Length; index++)
                {
                    Toolkit.TraceWrite(String.Format("{0}:{1}", (index + 1), args[index]));
                }
            }
            Toolkit.TraceWrite(" Start Main =============");

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
                if (Debugger.IsAttached)
                {
                    MessageBox.Show("DEBUG");
                }
#endif
            }
            new Main().Run(args);
        }
    }
}
