using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
