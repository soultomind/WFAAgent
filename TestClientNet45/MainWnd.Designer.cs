namespace TestClientNet45
{
    partial class MainWnd
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWnd));
            this._ButtonConnectAgentTcpServer = new System.Windows.Forms.Button();
            this._RichTextBoxReceiveDataAgentTcpServer = new System.Windows.Forms.RichTextBox();
            this._LabelReceiveDataAgentTcpServer = new System.Windows.Forms.Label();
            this._LabelSendDataAgentTcpServer = new System.Windows.Forms.Label();
            this._RichTextBoxSendDataAgentTcpServer = new System.Windows.Forms.RichTextBox();
            this._ButtonSendDataAgentTcpServer = new System.Windows.Forms.Button();
            this._ButtonImageFileBinary = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _ButtonConnectAgentTcpServer
            // 
            this._ButtonConnectAgentTcpServer.Location = new System.Drawing.Point(12, 15);
            this._ButtonConnectAgentTcpServer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this._ButtonConnectAgentTcpServer.Name = "_ButtonConnectAgentTcpServer";
            this._ButtonConnectAgentTcpServer.Size = new System.Drawing.Size(158, 32);
            this._ButtonConnectAgentTcpServer.TabIndex = 0;
            this._ButtonConnectAgentTcpServer.Text = "AgentTcpServer 에 연결";
            this._ButtonConnectAgentTcpServer.UseVisualStyleBackColor = true;
            this._ButtonConnectAgentTcpServer.Click += new System.EventHandler(this._ButtonConnectAgentTcpServer_Click);
            // 
            // _RichTextBoxReceiveDataAgentTcpServer
            // 
            this._RichTextBoxReceiveDataAgentTcpServer.Location = new System.Drawing.Point(12, 296);
            this._RichTextBoxReceiveDataAgentTcpServer.Name = "_RichTextBoxReceiveDataAgentTcpServer";
            this._RichTextBoxReceiveDataAgentTcpServer.Size = new System.Drawing.Size(460, 134);
            this._RichTextBoxReceiveDataAgentTcpServer.TabIndex = 1;
            this._RichTextBoxReceiveDataAgentTcpServer.Text = "";
            // 
            // _LabelReceiveDataAgentTcpServer
            // 
            this._LabelReceiveDataAgentTcpServer.AutoSize = true;
            this._LabelReceiveDataAgentTcpServer.Location = new System.Drawing.Point(12, 278);
            this._LabelReceiveDataAgentTcpServer.Name = "_LabelReceiveDataAgentTcpServer";
            this._LabelReceiveDataAgentTcpServer.Size = new System.Drawing.Size(135, 15);
            this._LabelReceiveDataAgentTcpServer.TabIndex = 2;
            this._LabelReceiveDataAgentTcpServer.Text = "서버에서 전달된 데이터";
            // 
            // _LabelSendDataAgentTcpServer
            // 
            this._LabelSendDataAgentTcpServer.AutoSize = true;
            this._LabelSendDataAgentTcpServer.Location = new System.Drawing.Point(12, 94);
            this._LabelSendDataAgentTcpServer.Name = "_LabelSendDataAgentTcpServer";
            this._LabelSendDataAgentTcpServer.Size = new System.Drawing.Size(123, 15);
            this._LabelSendDataAgentTcpServer.TabIndex = 4;
            this._LabelSendDataAgentTcpServer.Text = "서버로 전달할 데이터";
            // 
            // _RichTextBoxSendDataAgentTcpServer
            // 
            this._RichTextBoxSendDataAgentTcpServer.Location = new System.Drawing.Point(15, 112);
            this._RichTextBoxSendDataAgentTcpServer.Name = "_RichTextBoxSendDataAgentTcpServer";
            this._RichTextBoxSendDataAgentTcpServer.Size = new System.Drawing.Size(460, 134);
            this._RichTextBoxSendDataAgentTcpServer.TabIndex = 3;
            this._RichTextBoxSendDataAgentTcpServer.Text = "";
            // 
            // _ButtonSendDataAgentTcpServer
            // 
            this._ButtonSendDataAgentTcpServer.Location = new System.Drawing.Point(370, 83);
            this._ButtonSendDataAgentTcpServer.Name = "_ButtonSendDataAgentTcpServer";
            this._ButtonSendDataAgentTcpServer.Size = new System.Drawing.Size(105, 23);
            this._ButtonSendDataAgentTcpServer.TabIndex = 5;
            this._ButtonSendDataAgentTcpServer.Text = "서버로 전송";
            this._ButtonSendDataAgentTcpServer.UseVisualStyleBackColor = true;
            this._ButtonSendDataAgentTcpServer.Click += new System.EventHandler(this._ButtonSendDataAgentTcpServer_Click);
            // 
            // _ButtonImageFileBinary
            // 
            this._ButtonImageFileBinary.Location = new System.Drawing.Point(149, 83);
            this._ButtonImageFileBinary.Name = "_ButtonImageFileBinary";
            this._ButtonImageFileBinary.Size = new System.Drawing.Size(215, 23);
            this._ButtonImageFileBinary.TabIndex = 6;
            this._ButtonImageFileBinary.Text = "이미지 파일 바이너리 데이터 추가";
            this._ButtonImageFileBinary.UseVisualStyleBackColor = true;
            this._ButtonImageFileBinary.Click += new System.EventHandler(this. ButtonImageFileBinary_Click);
            // 
            // MainWnd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 442);
            this.Controls.Add(this._ButtonImageFileBinary);
            this.Controls.Add(this._ButtonSendDataAgentTcpServer);
            this.Controls.Add(this._LabelSendDataAgentTcpServer);
            this.Controls.Add(this._RichTextBoxSendDataAgentTcpServer);
            this.Controls.Add(this._LabelReceiveDataAgentTcpServer);
            this.Controls.Add(this._RichTextBoxReceiveDataAgentTcpServer);
            this.Controls.Add(this._ButtonConnectAgentTcpServer);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MainWnd";
            this.Text = "MainWnd";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainWnd_FormClosed);
            this.Load += new System.EventHandler(this.MainWnd_Load);
            this.Shown += new System.EventHandler(this.MainWnd_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _ButtonConnectAgentTcpServer;
        private System.Windows.Forms.RichTextBox _RichTextBoxReceiveDataAgentTcpServer;
        private System.Windows.Forms.Label _LabelReceiveDataAgentTcpServer;
        private System.Windows.Forms.Label _LabelSendDataAgentTcpServer;
        private System.Windows.Forms.RichTextBox _RichTextBoxSendDataAgentTcpServer;
        private System.Windows.Forms.Button _ButtonSendDataAgentTcpServer;
        private System.Windows.Forms.Button _ButtonImageFileBinary;
    }
}