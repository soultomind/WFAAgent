using System;
using System.ComponentModel;
using System.Windows.Forms;
using WFAAgent.Core;
using WFAAgent.Framework.Win32;
using WFAAgent.Message;
using WFAAgent.Server;

namespace WFAAgent
{
    public partial class ServerForm : Form
    {
        private bool _IsAgentStart;
        private Exception _AgentStartException;
        private IAgentManager _AgentManager;

        private MessageItemQueueAppendWorker<MessageItem> _MessageItemQueueAppendWorker;
        public ServerForm(string[] args)
        {
            InitializeComponent();

            InitializeTaskbarTray();

            _AgentManager = new AgentManager();
            _AgentManager.MessageObjectReceived += AgentManager_MessageObjectReceived;

            _MessageItemQueueAppendWorker = new MessageItemQueueAppendWorker<MessageItem>();
            _MessageItemQueueAppendWorker.MessageItemReceived += MessageItemQueueAppendWorker_MessageItemReceived;

            Text = String.Format("WFAAgent.ServerForm Administrator={0}", Toolkit.IsCurrentProcessExecuteAdministrator());
        }

        #region Properties
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        internal InfoDialog InfoDialog { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        internal TerminalDialog TerminalDialog { get; set; }
        #endregion

        private void InitializeTaskbarTray()
        {
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;
        }

        private void ServerForm_Load(object sender, EventArgs e)
        {
            
        }

        private void ServerForm_Shown(object sender, EventArgs e)
        {
            Taskbar.RefreshTrayArea();

            try
            {
                _AgentManager.StartServer();
                ServerForm_SendMonitoringOutputData("ServerForm 서버 시작");
                _IsAgentStart = true;
            }
            catch (Exception ex)
            {
                _IsAgentStart = false;
                _AgentStartException = ex;
                ServerForm_SendMonitoringErrorData("ServerForm 서버 시작 실패\n" + ex.ToString());
            }

            if (!_IsAgentStart)
            {
                // TODO: 에외 출력
                MessageBox.Show(this, _AgentStartException.StackTrace, "서버 시작에 실패하였습니다.");
                return;
            }
        }

        private void TrayNotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (InfoDialog == null)
            {
                InfoDialog = new InfoDialog();
                InfoDialog.Text = String.Format("{0}.{1}", Application.ProductName, Execute.Server.ToString());
            }

            if (Main.HasOpenForm(InfoDialog.Text))
            {
                InfoDialog.Visible = true;
            }
            else
            {
                InfoDialog.Show();
            }
            InfoDialog.Activate();
        }

        private void ShowTerminalDialogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TerminalDialog == null)
            {
                // TODO: 미리 켜놓고 숨긴상태로 미리 메시지 추가해놓기
                TerminalDialog = new TerminalDialog();
                TerminalDialog.Text = "TermialDialog." + Execute.Server.ToString();
                TerminalDialog.Shown += TerminalDialog_Shown;
                TerminalDialog.FormClosed += TerminalDlg_FormClosed;
            }

            TerminalDialog.Show();
        }


        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TerminalDialog != null && !TerminalDialog.IsDisposed)
            {
                TerminalDialog.Close();
            }

            if (_MessageItemQueueAppendWorker.IsStart)
            {
                _MessageItemQueueAppendWorker.Stop();
            }

            Application.Exit();
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
            if (TerminalDialog != null && !TerminalDialog.IsDisposed)
            {
                if (item is DetailMessageItem)
                {

                }
                else
                {
                    TerminalDialog.AppendText(item.MakeMessage());
                }
            }
        }

        #region TerminalDialog
        private void TerminalDialog_Shown(object sender, EventArgs e)
        {
            _MessageItemQueueAppendWorker.Start();
        }

        private void TerminalDlg_FormClosed(object sender, FormClosedEventArgs e)
        {
            TerminalDialog = null;
        }
        #endregion
    }
}
