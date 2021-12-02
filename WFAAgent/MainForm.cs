using CommonLibrary;
using System;
using System.ComponentModel;
using System.Windows.Forms;
using WFAAgent.Core;
using WFAAgent.Dialogs;
using WFAAgent.Message;

namespace WFAAgent
{
    public partial class MainForm : Form
    {
        private bool _IsAgentStart;
        private Exception _AgentStartException;
        private IAgentManager _AgentManager;

        private MessageItemQueueAppendWorker<MessageItem> _MessageItemQueueAppendWorker;
        public MainForm()
        {
            InitializeComponent();

            InitializeTaskbarTray();

            _AgentManager = new AgentManager();
            _AgentManager.MessageObjectReceived += AgentManager_MessageObjectReceived;

            _MessageItemQueueAppendWorker = new MessageItemQueueAppendWorker<MessageItem>();
            _MessageItemQueueAppendWorker.MessageItemReceived += MessageItemQueueAppendWorker_MessageItemReceived;

        }

        #region Properties
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        internal InfoDialog InfoDialog { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        internal MonitoringDialog MonitoringDialog { get; set; }
        #endregion

        private void InitializeTaskbarTray()
        {
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                _AgentManager.StartServer();
                _IsAgentStart = true;
            }
            catch (Exception ex)
            {
                _IsAgentStart = false;
                _AgentStartException = ex;
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            Taskbar.RefreshTrayArea();

            if (!_IsAgentStart)
            {
                // TODO: 에외 출력
            }
        }

        private void AgentManager_MessageObjectReceived(object messageObject)
        {
#if DEBUG
            Toolkit.DebugWriteLine(messageObject.ToString());
#else
            Toolkit.TraceWriteLine(messageObject.ToString());
#endif
            if (messageObject is string)
            {
                string message = messageObject.ToString();
                _MessageItemQueueAppendWorker.Enqueue(new MessageItem() { Message = message });
            }
            else
            {
                MessageItem messageItem = messageObject as MessageItem;
                _MessageItemQueueAppendWorker.Enqueue(messageItem);
            }
        }

        private void MessageItemQueueAppendWorker_MessageItemReceived(MessageItem item)
        {
            if (MonitoringDialog != null && !MonitoringDialog.IsDisposed)
            {
                if (item is DetailMessageItem)
                {

                }
                else
                {
                    MonitoringDialog.AppendMessageLine(item.Message);
                }
            }
        }

        private void TrayNotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (InfoDialog == null)
            {
                InfoDialog = new InfoDialog();
            }

            if (Main.OpenForm(InfoDialog.Text))
            {
                InfoDialog.ReShow();
            }
            else
            {
                InfoDialog.Show();
            }
        }

        private void ShowMonitoringDlgToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MonitoringDialog == null)
            {
                MonitoringDialog = new MonitoringDialog();
                MonitoringDialog.Shown += MonitoringDialog_Shown;
                MonitoringDialog.FormClosed += MonitoringDlg_FormClosed;
            }

            MonitoringDialog.Show();
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MonitoringDialog != null && !MonitoringDialog.IsDisposed)
            {
                MonitoringDialog.Close();
            }

            if (_MessageItemQueueAppendWorker.IsStart)
            {
                _MessageItemQueueAppendWorker.Stop();
            }

            Application.Exit();
        }

        #region MonitoringDialog
        private void MonitoringDialog_Shown(object sender, EventArgs e)
        {
            _MessageItemQueueAppendWorker.Start();
        }

        private void MonitoringDlg_FormClosed(object sender, FormClosedEventArgs e)
        {
            MonitoringDialog = null;
        }
        #endregion
    }
}
