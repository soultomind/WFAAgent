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

        /// <summary>
        /// 프로세스 아이디
        /// </summary>
        public int ProcessId
        {
            get { return _ProcessId; }
            set { _ProcessId = value; }
        }
        private int _ProcessId;

        /// <summary>
        /// 데이터 타입
        /// </summary>
        public ushort Type
        {
            get { return _Type; }
            internal set { _Type = value; }
        }
        private ushort _Type = DataContext.AgentStringData;

        /// <summary>
        /// 전송 데이터 타입
        /// </summary>
        public TransmissionData TransmissionData
        {
            get { return _TransmissionData; }
            internal set { _TransmissionData = value; }
        }
        private TransmissionData _TransmissionData = TransmissionData.Text;

        /// <summary>
        /// 실제 전송 데이터 길이
        /// </summary>
        public int DataLength
        {
            get { return _DataLength; }
            internal set { _DataLength = value; }
        }
        private int _DataLength;
    }
}
