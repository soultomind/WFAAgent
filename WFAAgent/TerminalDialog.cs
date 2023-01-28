using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WFAAgent.Core;

namespace WFAAgent
{
    internal partial class TerminalDialog : Form
    {
        
        public TerminalDialog()
        {
            InitializeComponent();
        }

        private void TerminalDialog_Load(object sender, EventArgs e)
        {

        }

        private void TerminalDialog_Shown(object sender, EventArgs e)
        {

        }

        internal void InitializeMinimized()
        {
            this.WindowState = FormWindowState.Minimized;
            this.Visible = false;
        }

        internal void InitializeNormal(Point location)
        {
            this.Location = location;
            this.WindowState = FormWindowState.Normal;
            this.Visible = true;
        }

        internal void AppendText(string message)
        {
            if (_RichTextBox.InvokeRequired)
            {
                _RichTextBox.Invoke(new Action<string>(AppendText), new object[] { message });
                return;
            }

            _RichTextBox.AppendText(message);
            _RichTextBox.AppendText(Environment.NewLine);
            _RichTextBox.ScrollToCaret();
        }

        private void TerminalDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Visible = false;
                e.Cancel = true;
            }
        }
    }
}
