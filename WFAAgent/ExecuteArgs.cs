using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent
{
    public class ExecuteArgs
    {
        public const string ExecuteType = "ExecuteType";
        public const string Monitoring = "Monitoring";
        
        public ExecuteArgs()
        {
            Args = new Dictionary<string, string>();
        }
        public Dictionary<string, string> Args
        {
            get; set;
        }

        public static string[] MonitoringArgs
        {
            get
            {
                return new string[] { ExecuteType + "=" + Monitoring };
            }
        }
        public static ExecuteArgs Parse(string[] args)
        {
            ExecuteArgs o = new ExecuteArgs();
            if (args.Length == 1)
            {
                string[] pairs = args[0].Split(new char[] { ';' });
                foreach (string pair in pairs)
                {
                    string[] item = pair.Split(new char[] { '=' });
                    string key = item[0];
                    string value = item[1];
                    o.Args.Add(key, value);
                }
            }
            return o;
        }
    }
}
