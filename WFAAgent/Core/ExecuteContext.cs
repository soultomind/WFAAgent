using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFAAgent.Framework.Application;

namespace WFAAgent.Core
{
    public class ExecuteContext
    {
        public const char NameValueDelimiter = '=';
        public const char ParamDelimiter = ';';

        public static readonly string ExecuteType = "ExecuteType";

        public static readonly string ExecuteType_Monitoring;
        public static readonly ExecuteContext Monitoring;

        public static readonly string ExecuteType_Server;
        public static readonly ExecuteContext Server;

        static ExecuteContext()
        {
            ExecuteType_Monitoring = Execute.Monitoring.ToString();
            ExecuteType_Server = Execute.Server.ToString();

            Monitoring = Create(Execute.Monitoring);
            Server = Create(Execute.Server);
        }

        public ExecuteContext()
        {
            ArgsDictionary = new Dictionary<string, string>();
        }

        internal Execute Execute
        {
            get; set;
        }

        internal Dictionary<string, string> ArgsDictionary
        {
            get; set;
        }

        internal Dictionary<string, string> CopyArgsDirectory
        {
            get
            {
                Dictionary<string, string> copyArgsDictionary = new Dictionary<string, string>(ArgsDictionary);
                return copyArgsDictionary;
            }
        }

        private static ExecuteContext Create(Execute execute)
        {
            ExecuteContext context = new ExecuteContext();
            context.Execute = execute;

            Dictionary<string, string> argsDictionary = new Dictionary<string, string>();
            argsDictionary[ExecuteType] = execute.ToString();
            switch (execute)
            {
                case Execute.Server:
                    break;
                case Execute.Monitoring:
                    break;
            }
            context.ArgsDictionary = argsDictionary;
            return context;
        }

        internal static string[] ExecCommandLineArgs
        {
            get; set;
        }

        internal static string ExecuteMonitoringArgs
        {
            get
            {
                return ExecuteType + NameValueDelimiter + Execute.Monitoring.ToString();
            }
        }

        internal static string ExecuteServerArgs
        {
            get
            {
                return ExecuteType + NameValueDelimiter + Execute.Server.ToString();
            }
        }

        internal string[] UserCommandLineArgs
        {
            get; set;
        }

        internal static ExecuteContext Parse(string[] args)
        {
            ExecuteContext o = new ExecuteContext();
            o.UserCommandLineArgs = args;
            if (args.Length == 1)
            {
                string[] pairs = args[0].Split(new char[] { ParamDelimiter });
                foreach (string pair in pairs)
                {
                    string[] item = pair.Split(new char[] { NameValueDelimiter });

                    string key = item[0];
                    string value = item[1];

                    o.Execute = (Execute)Enum.Parse(typeof(Execute), value);
                    o.ArgsDictionary.Add(key, value);
                }
            }
            return o;
        }

        internal static string MakeStringArgs(Dictionary<string, string> args)
        {
            StringBuilder builder = new StringBuilder();
            for (int index = 0; index < args.Count; index++)
            {
                var item = args.ElementAt(index);
                var key = item.Key;
                var value = item.Value;
                builder.Append(String.Format("{0}{1}{2}", key, NameValueDelimiter, value));
                if (index + 1 != args.Count)
                {
                    builder.Append(ParamDelimiter);
                }
            }
            return builder.ToString();
        }

        internal static Dictionary<string, string> MakeServerDictionaryArgs()
        {
            // 모니터링 Pid 같이 실행인자로 추가하여 넘겨주기
            Dictionary<string, string> dictionary = ExecuteContext.Server.CopyArgsDirectory;
            dictionary.Add(Constant.ParentProcessId, Process.GetCurrentProcess().Id.ToString());
            return dictionary;
        }
    }
    

    public enum Execute
    {
        None, Monitoring, Server
    }
}
