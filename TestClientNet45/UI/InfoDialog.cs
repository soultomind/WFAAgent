﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestClient.UI
{
    public partial class InfoDialog : Form
    {
        public string AppId { get; set; }
        public InfoDialog(string appId)
        {
            InitializeComponent();

            AppId = appId;
        }

        private void InfoDialog_Load(object sender, EventArgs e)
        {
            _LabelValueVersion.Text = Main.EntryAssemblyVersion;
            _LabelValueAppId.Text = AppId;

            Screen screen = Screen.PrimaryScreen;
            int x = (screen.WorkingArea.Width - Width) / 2;
            int y = (screen.WorkingArea.Height - Height) / 2;
            Location = new Point(x, y);
        }

        private void InfoDialog_Shown(object sender, EventArgs e)
        {

        }

        private void InfoDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Visible = false;
                e.Cancel = true;
            }
        }

        public void ReShow()
        {
            Visible = true;
            Activate();
        }
    }
}
