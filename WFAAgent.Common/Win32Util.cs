using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent
{
    public class Win32Util
    {
        public static void RefreshTrayArea()
        {
            IntPtr shellTrayHwnd = User32.FindWindow("Shell_TrayWnd", null);
            RefreshTrayArea(shellTrayHwnd);
            IntPtr trayNotifyHwnd = User32.FindWindowEx(shellTrayHwnd, IntPtr.Zero, "TrayNotifyWnd", null);
            RefreshTrayArea(trayNotifyHwnd);
            IntPtr sysPaperHwnd = User32.FindWindowEx(trayNotifyHwnd, IntPtr.Zero, "SysPager", null);
            RefreshTrayArea(sysPaperHwnd);
            RefreshTrayArea(User32.FindWindowEx(sysPaperHwnd, IntPtr.Zero, "ToolbarWindow32", null));
            IntPtr notifyIconOverflowWindowHwnd = User32.FindWindow("NotifyIconOverflowWindow", null);
            RefreshTrayArea(notifyIconOverflowWindowHwnd);
            RefreshTrayArea(User32.FindWindowEx(notifyIconOverflowWindowHwnd, IntPtr.Zero, "ToolbarWindow32", null));
        }

        private static void RefreshTrayArea(IntPtr windowHandle)
        {
            RECT lpRect;
            User32.GetClientRect(windowHandle, out lpRect);

            int x = 0;
            while (x < lpRect.right)
            {
                int y = 0;
                while (y < lpRect.bottom)
                {
                    User32.SendMessage(windowHandle, 512U, 0, (y << 16) + x);
                    y += 5;
                }
                x += 5;
            }
        }
    }
}
