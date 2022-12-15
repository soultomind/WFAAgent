using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFAAgent.Framework.Net.Sockets;

namespace WFAAgent.Framework.Net
{
    /// <summary>
    /// 패킷 베이스 클래스
    /// </summary>
    [Serializable]
    public abstract class Packet
    {
        /// <summary>
        /// 데이터 타입
        /// </summary>
        public ushort DataType
        {
            get { return _dataType; }
            private set { _dataType = value; }
        }
        private ushort _dataType;

        /// <summary>
        /// 전송 데이터
        /// </summary>
        public TransmissionData TransmissionData
        {
            get { return _transmissionData; }
            internal set { _transmissionData = value; }
        }
        private TransmissionData _transmissionData;

        /// <summary>
        /// 실제 데이터 길이
        /// </summary>
        public uint DataLength
        {
            get { return _dataLength; }
            private set { _dataLength = value; }
        }
        private uint _dataLength;
    }

    public enum PacketData : int
    {
        Text,
        Binary,
        Client,
        WebClient
    }
}
