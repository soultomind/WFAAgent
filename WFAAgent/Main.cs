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
        private static ExecuteArgs _sExecuteArgs;
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
            
            Run(ExecuteArgs = ExecuteArgs.Parse(args));
            
        }

        private void Run(ExecuteArgs args)
        {
            Application.Run(Form = new MainForm());
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

        public static ExecuteArgs ExecuteArgs
        {
            get { return _sExecuteArgs; }
            set { _sExecuteArgs = value; }
        }

        public static string[] Args
        {
            get { return _sArgs; }
            private set { _sArgs = value; }
        }

        public static MainForm Form
        {
            get { return _sMainForm; }
            private set { _sMainForm = value; }
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
