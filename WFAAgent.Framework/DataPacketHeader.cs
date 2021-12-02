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
            // 1. 4Byte = 2Byte [ Type(0-32767) ] + 1Byte [ TransmissionData(0-1) + 1Byte [ Length ]]
            // 2. 4Byte = 4Byte [ DataLength ]
            // 3. Data ..

            // 시작 데이터 타입 + 데이터 전송 형식(Text,Binary) + 해당 패킷 헤더 길이
            // 2Byte
            byte[] headerBuffer1 = BitConverter.GetBytes(_Type);
            // 1Byte
            headerBuffer1[2] = (byte)_TransmissionData;
            // 1Byte = HeaderBuffer1.Length
            headerBuffer1[3] = (byte)headerBuffer1.Length;




            // 실제 전송되는 데이터 길이
            byte[] headerBuffer2 = BitConverter.GetBytes(_DataLength);




            // 실제 전송되는 데이터 생성 [1. 시작 데이터 타입 + 2. 실제 전송되는 데이터 길이 + 3. 데이터 ]
            byte[] createBuffer = new byte[headerBuffer1.Length + headerBuffer2.Length + _Buffer.Length];
            // 1. 시작 데이터 타입 복사
            Array.Copy(headerBuffer1, 0, createBuffer, 0, headerBuffer1.Length);
            // 2. 실제 전송되는 데이터 길이 복사
            Array.Copy(headerBuffer2, 0, createBuffer, headerBuffer1.Length, headerBuffer2.Length);
            // 3. 실제 데이터 복사
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