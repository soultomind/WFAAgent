using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework.Net.Packets
{
    /// <summary>
    /// Text 패킷 클래스
    /// </summary>
    [Serializable]
    public class TextPacket : Packet
    {
        public string Data
        {
            get { return _data; }
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

                _data = value;
            }
        }
        private string _data = String.Empty;

        public TextPacket()
        {
            TransmissionData = TransmissionData.Text;
        }
    }
}
