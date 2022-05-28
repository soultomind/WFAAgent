using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework.Net.Sockets
{
    public class Header
    {
        /// <summary>
        /// TcpClient 쪽 클라이언트 프로그램 AppId
        /// <para>WebSocket 에서는 SessionId정보</para>
        /// <para>TcpSocket 에서는 Guid정보</para>
        /// </summary>
        public string AppId
        {
            get { return _AppId; }
            set { _AppId = value; }
        }
        private string _AppId = String.Empty;
        public ushort Type
        {
            get { return _Type; }
            internal set { _Type = value; }
        }
        private ushort _Type = DataContext.AgentStringData;

        public TransmissionData TransmissionData
        {
            get { return _TransmissionData; }
            internal set { _TransmissionData = value; }
        }
        private TransmissionData _TransmissionData = TransmissionData.Text;

        public int DataLength
        {
            get { return _DataLength; }
            internal set { _DataLength = value; }
        }
        private int _DataLength;
    }
}
