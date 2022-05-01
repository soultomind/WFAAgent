using log4net;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace WFAAgent
{
    public class Main
    {
        private static string _sEntryAssemblyVersion;
        private static ILog _sLog = LogManager.GetLogger(typeof(Main));
        private static string[] _sArgs;
        private static ExecuteManager _sExecuteArgs;
        private static Form _sCurrentForm;
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
            
            Run(ExecuteArgs = ExecuteManager.Parse(args));
            
        }

        private void Run(ExecuteManager args)
        {
            string executeType = args.ArgsDictionary[ExecuteManager.ExecuteType];
            switch (executeType)
            {
                case ExecuteManager.ExecuteType_Server:
                    CurrentForm = new ServerForm(args.UserCommandLineArgs);
                    break;
                case ExecuteManager.ExecuteType_Monitoring:
                    CurrentForm = new MonitoringForm(args.UserCommandLineArgs);
                    break;
            }
            Application.Run(CurrentForm);
        }

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

        public static ILog Log
        {
            get { return _sLog; }
        }

        public static ExecuteManager ExecuteArgs
        {
            get { return _sExecuteArgs; }
            set { _sExecuteArgs = value; }
        }

        public static string[] Args
        {
            get { return _sArgs; }
            private set { _sArgs = value; }
        }

        public static Form CurrentForm
        {
            get { return _sCurrentForm; }
            private set { _sCurrentForm = value; }
        }
        public static bool OpenForm(string text)
        {
            if (Application.OpenForms.Count == 1)
            {
                return false;
            }

            foreach (Form openForm in Application.OpenForms)
            {
                if (!object.ReferenceEquals(CurrentForm, openForm) && openForm.Text == text)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
