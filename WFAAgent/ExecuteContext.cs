using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent
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

        public Execute Execute
        {
            get; set;
        }

        public Dictionary<string, string> ArgsDictionary
        {
            get; set;
        }

        public Dictionary<string, string> CopyArgsDirectory
        {
            get
            {
                Dictionary<string, string> copyArgsDictionary = new Dictionary<string, string>(ArgsDictionary);
                return copyArgsDictionary;
            }
        }

        public string[] UserCommandLineArgs
        {
            get; set;
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

        public static string ExecuteMonitoringArgs
        {
            get
            {
                return ExecuteType + NameValueDelimiter + Execute.Monitoring.ToString();
            }
        }

        public static string ExecuteServerArgs
        {
            get
            {
                return ExecuteType + NameValueDelimiter + Execute.Server.ToString();
            }
        }

        public static string[] ExecCommandLineArgs
        {
            get; set;
        }

        public static ExecuteContext Parse(string[] args)
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

        public static string MakeStringArgs(Dictionary<string, string> args)
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
    }
    

    public enum Execute
    {
        None, Monitoring, Server
    }
}
