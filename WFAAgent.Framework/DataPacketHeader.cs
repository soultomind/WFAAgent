using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework
{
    public class DataPacketHeader
    {
        private int _Type;
        private byte[] _Buffer;
        private TransmissionData _TransmissionData;

        private int _DataLength;
        private int _DataOffset;

        public DataPacketHeader(int type, byte[] buffer)
            : this(type, buffer, TransmissionData.Text)
        {

        }

        public DataPacketHeader(int type, byte[] buffer, TransmissionData transmissionData)
        {
            DataContext.CheckedType(type);
            this._Type = type;
            this._Buffer = buffer;
            this._TransmissionData = transmissionData;

            this._DataLength = buffer.Length;

            this._DataOffset = 0;
        }

        public byte[] CreateBuffer()
        {
            // 1. 4Byte = 1Byte [ Type(0-255) ] 1Byte [ TransmissioNData(0-1) ]
            // 2. 4Byte = 4Byte [ DataLength ]
            // 3. Data ..
            byte[] headerBuffer1 = BitConverter.GetBytes(_Type);
            // 00000000
            headerBuffer1[1] = (byte)_TransmissionData;

            byte[] headerBuffer2 = BitConverter.GetBytes(_DataLength);

            byte[] createBuffer = new byte[headerBuffer1.Length + headerBuffer2.Length + _Buffer.Length];
            headerBuffer1[2] = (byte)headerBuffer1.Length;

            Array.Copy(headerBuffer1, 0, createBuffer, 0, headerBuffer1.Length);
            Array.Copy(headerBuffer2, 0, createBuffer, headerBuffer1.Length, headerBuffer2.Length);
            Array.Copy(_Buffer, 0, createBuffer, headerBuffer1.Length + headerBuffer2.Length, _Buffer.Length);

            return createBuffer;
        }

        public int Type
        {
            get { return _Type; }
        }

        public byte[] Buffer
        {
            get { return _Buffer; }
        }

        public TransmissionData TransmissionData
        {
            get { return _TransmissionData; }
        }

        public int DataOffset
        {
            get { return _DataOffset; }
        }

        public int DataLength
        {
            get { return _DataLength; }
        }
    }
}