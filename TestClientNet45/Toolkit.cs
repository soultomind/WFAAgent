using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace TestClientNet45
{
    internal static class Toolkit
    {
        /// <summary>
        /// DebugView Filter 이름
        /// <para>Filter/Hightlight 메뉴 Include 항목에 사용될 값</para>
        /// </summary>
        public static string IncludeFilterName
        {
            get { return _sIncludeFilterName; }
            set
            {
                if (!String.IsNullOrEmpty(value) && value.Length > 1)
                {
                    _sIncludeFilterName = value;
                }
            }
        }
        private static string _sIncludeFilterName;

        /// <summary>
        /// <see cref="Debug.WriteLine(object)"/>,<see cref="Debug.Write(object)"/>
        /// <para>메서드 출력 여부</para>
        /// </summary>
        public static bool IsDebugEnabled;

        /// <summary>
        /// <see cref="Trace.WriteLine(object)"/>,<see cref="Trace.Write(object)"/>
        /// <para>메서드 출력 여부</para>
        /// </summary>
        public static bool IsTraceEnabled;

        /// <summary>
        /// 메시지 출력시 현재시간 출력 여부
        /// </summary>
        public static bool UseNowToString;
        static Toolkit()
        {

#if DEBUG
            _sIncludeFilterName = CreateNamespace();
            IsDebugEnabled = true;
#else
            _sIncludeFilterName = "TestClient";
            IsDebugEnabled = false;
#endif
            IsTraceEnabled = true;
            UseNowToString = false;
        }

        private static string CreateNamespace()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(Toolkit));
            return assembly.GetName().Name;
        }

        private static string NowToString(string format = "yyyy/MM/dd HH:mm:ss")
        {
            return DateTime.Now.ToString(format);
        }

        private static string MakeMessage(string message)
        {
            string className = new StackFrame(1).GetMethod().ReflectedType.Name;
            string methodName = new StackFrame(1, true).GetMethod().Name;
            string header = String.Format("{0} :: {1}", className, methodName);
#if DEBUG
            message = String.Format("[{0}] DEBUG - {1}", header, message);
#else
            message = String.Format("[{0}] TRACE - {1}", header, message);
#endif
            return message;
        }

        /// <summary>
        /// <see cref="System.Diagnostics.Debug.WriteLine(object)"/>을 활용하여 메시지를 출력합니다.
        /// </summary>
        /// <param name="message"></param>
        internal static void DebugWriteLine(string message)
        {
            if (IsDebugEnabled)
            {
                message = MakeMessage(message);
                Debug.WriteLine(message);
            }
        }

        /// <summary>
        /// <see cref="System.Diagnostics.Debug.Write(object)"/>을 활용하여 메시지를 출력합니다.
        /// </summary>
        /// <param name="message"></param>
        internal static void DebugWrite(string message)
        {
            if (IsDebugEnabled)
            {
                message = MakeMessage(message);
                Debug.Write(message);
            }
        }

        /// <summary>
        /// <see cref="System.Diagnostics.Trace.WriteLine(object)"/>을 활용하여 메시지를 출력합니다.
        /// </summary>
        /// <param name="message"></param>
        internal static void TraceWriteLine(string message)
        {
            if (IsTraceEnabled)
            {
                message = MakeMessage(message);
                Trace.WriteLine(message);
            }
        }

        /// <summary>
        /// <see cref="System.Diagnostics.Trace.Write(object)"/>을 활용하여 메시지를 출력합니다.
        /// </summary>
        /// <param name="message"></param>
        internal static void TraceWrite(string message)
        {
            if (IsTraceEnabled)
            {
                message = MakeMessage(message);
                Trace.Write(message);
            }
        }

        internal static bool IsCurrentAdministratorProcess()
        {
            bool flag;

            WindowsIdentity identity = null;
            try
            {
                identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                flag = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (Exception ex)
            {
                TraceWriteLine(ex.Message);
                TraceWriteLine(ex.StackTrace);

                flag = false;
            }
            finally
            {
                if (identity != null)
                {
                    identity.Dispose();
                }
            }

            return flag;
        }
    }
}
