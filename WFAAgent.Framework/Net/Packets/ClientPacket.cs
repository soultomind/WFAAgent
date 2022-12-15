using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework.Net.Packets
{
    [Serializable]
    public class ClientPacket : Packet
    {
        /// <summary>
        /// 클라이언트 프로세스 AppId (고유의 아이디)
        /// <para></para>
        /// <para></para>
        /// </summary>
        public string AppId
        {
            get { return _appId; }
            set { _appId = value; }
        }
        private string _appId = String.Empty;

        /// <summary>
        /// 프로세스 아이디
        /// </summary>
        public int ProcessId
        {
            get { return _processId; }
            set { _processId = value; }
        }
        private int _processId;

        public ClientPacket()
        {
            TransmissionData = TransmissionData.Object;
        }
    }
}
