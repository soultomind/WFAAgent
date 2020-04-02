using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent
{
    public class User32
    {
        private const string dllName = "User32.dll";

        [DllImport(dllName)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport(dllName)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport(dllName)]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport(dllName)]
        public static extern int SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);
    }
}
