using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent
{
    public class ExecuteManager
    {
        public const string ExecuteType = "ExecuteType";
        public const string ExecuteType_Monitoring = "Monitoring";
        public const string ExecuteType_Server = "Server";

        public ExecuteManager()
        {
            ArgsDictionary = new Dictionary<string, string>();
        }
        public Dictionary<string, string> ArgsDictionary
        {
            get; set;
        }

        public string[] UserCommandLineArgs
        {
            get; set;
        }

        public Execute Execute
        {
            get; set;
        }

        public static string ExecuteMonitoring
        {
            get
            {
                return ExecuteType + "=" + Execute.Monitoring.ToString();
            }
        }

        public static string ExecuteServer
        {
            get
            {
                return ExecuteType + "=" + Execute.Server.ToString();
            }
        }

        public static string[] ExecCommandLineArgs
        {
            get; set;
        }

        public static ExecuteManager Parse(string[] args)
        {
            ExecuteManager o = new ExecuteManager();
            o.UserCommandLineArgs = args;
            if (args.Length == 1)
            {
                string[] pairs = args[0].Split(new char[] { ';' });
                foreach (string pair in pairs)
                {
                    string[] item = pair.Split(new char[] { '=' });

                    string key = item[0];
                    string value = item[1];

                    o.Execute = (Execute)Enum.Parse(typeof(Execute), value);
                    o.ArgsDictionary.Add(key, value);
                }
            }
            return o;
        }
    }

    public enum Execute
    {
        None, Monitoring, Server
    }
}
