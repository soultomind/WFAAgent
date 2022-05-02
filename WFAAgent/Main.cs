using log4net;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace WFAAgent
{
    public class Main
    {
        private static ILog _sLog = LogManager.GetLogger(typeof(Main));

        private static string _sEntryAssemblyVersion;
        private static string[] _sArgs;
        private static ExecuteContext _sExecuteArgs;
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
            
            Run(ExecuteArgs = ExecuteContext.Parse(args));
            
        }

        private void Run(ExecuteContext args)
        {
            string executeType = args.ArgsDictionary[ExecuteContext.ExecuteType];
            switch (executeType)
            {
                case ExecuteContext.ExecuteType_Server:
                    CurrentForm = new ServerForm(args.UserCommandLineArgs);
                    break;
                case ExecuteContext.ExecuteType_Monitoring:
                    CurrentForm = new MonitoringForm(args.UserCommandLineArgs);
                    break;
            }
            Application.Run(CurrentForm);
        }


        public static ILog Log
        {
            get { return _sLog; }
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

        public static ExecuteContext ExecuteArgs
        {
            get { return _sExecuteArgs; }
            set { _sExecuteArgs = value; }
        }

        public static Form CurrentForm
        {
            get { return _sCurrentForm; }
            private set { _sCurrentForm = value; }
        }

        public static string[] Args
        {
            get { return _sArgs; }
            private set { _sArgs = value; }
        }

        public static bool HasOpenForm(string text)
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
