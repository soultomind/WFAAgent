using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework.Net.NewFolder1
{
    [Serializable]
    /// <summary>
    /// 패킷 길이 정보 클래스
    /// <para>데이터 전송시에 가장 먼저 포함되는 정보입니다.</para>
    /// </summary>
    public class PacketLength
    {
        /// <summary>
        /// 길이
        /// </summary>
        public uint Length
        {
            get { return _length; }
            private set { _length = value; }
        }
        private uint _length;

        /// <summary>
        /// 패킷 길이를 받는 생성자입니다.
        /// </summary>
        /// <param name="length"></param>
        public PacketLength(uint length)
        {
            Length = length;
        }

        /// <summary>
        /// 바이트 배열로 반환합니다.
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            return BitConverter.GetBytes(_length);
        }
    }
}
