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
            ExecuteManager.ExecCommandLineArgs = args;

            if (args.Length == 0)
            {
                if (Debugger.IsAttached)
                {
                    args = new string[] { ExecuteManager.ExecuteServer };
                }
                else
                {
                    args = new string[] { ExecuteManager.ExecuteMonitoring };
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
