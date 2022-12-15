using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework.Net
{
    /// <summary>
    /// 전송 데이터
    /// </summary>
    public enum TransmissionData
    {
        /// <summary>
        /// 텍스트 (TextPacket)
        /// </summary>
        Text = 0,

        /// <summary>
        /// 바이너리 (BinaryPacket)
        /// </summary>
        Binary = 1,

        /// <summary>
        /// 객체
        /// </summary>
        Object = 2
    }
}
