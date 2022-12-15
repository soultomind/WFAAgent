using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WFAAgent.Framework.Application;

namespace TestClient
{
    public class Main
    {
        private static string _sEntryAssemblyVersion;
        public static string[] _sArgs;
        private static MainForm _sMainForm;

        public Main()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        }

        public void Run(string[] args)
        {
            Args = args;

            Application.Run(_sMainForm = new MainForm());
        }

        #region Properties
        public static string EntryAssemblyVersion
        {
            get
            {
                if (String.IsNullOrEmpty(_sEntryAssemblyVersion))
                {
                    Assembly assembly = Assembly.GetEntryAssembly();
                    Version version = assembly.GetName().Version;
                    _sEntryAssemblyVersion = version.ToString();
                }

                return _sEntryAssemblyVersion;
            }
        }

        public static string[] Args
        {
            get; private set;
        }

        public static MainForm Form
        {
            get; private set;
        }

        #endregion

        public static int ProcessId
        {
            get { return Process.GetCurrentProcess().Id; }
        }

        public static void AgentErrorDataSend(string data)
        {
            if (Console.IsErrorRedirected)
            {
                string text = JsonConvert.SerializeObject(new AgentErrorData() { ProcessId = ProcessId, Data = data });
                Console.Error.WriteLine(text);
            }
        }

        public static void AgentOutputDataSend(string data)
        {
            if (Console.IsOutputRedirected)
            {
                string text = JsonConvert.SerializeObject(new AgentOutputData() { ProcessId = ProcessId, Data = data });
                Console.Out.WriteLine(text);
            }
        }

        public static bool OpenForm(string text)
        {
            if (Application.OpenForms.Count == 1)
            {
                return false;
            }

            foreach (Form openForm in Application.OpenForms)
            {
                if (!object.ReferenceEquals(Form, openForm) && openForm.Text == text)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
