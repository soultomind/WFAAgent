using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework.Net.Packets
{
    /// <summary>
    /// Binary 패킷 클래스
    /// </summary>
    [Serializable]
    public class BinaryPacket : Packet
    {
        public byte[] RawData
        {
            get { return _rawData; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                if (value.Length == 0)
                {
                    throw new ArgumentException();
                }

                _rawData = value;
            }
        }
        private byte[] _rawData;

        public BinaryPacket()
        {
            TransmissionData = TransmissionData.Binary;
        }
    }
}
