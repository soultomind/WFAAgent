﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WFAAgent.Framework;

namespace TestClient
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string guid = Guid.NewGuid().ToString();

#if DEBUG
            MessageBox.Show("DEBUG");
#endif
            new Main().Run(args);
        }
    }
}
