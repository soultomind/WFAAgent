using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework.Net.Sockets
{
    public class DataPacket
    {
        public const int DataLengthPos = 4;
        public short Type
        {
            get { return _Type; }
            internal set { _Type = value; }
        }
        private short _Type;

        public TransmissionData TransmissionData
        {
            get { return _TransmissionData; }
            internal set { _TransmissionData = value; }
        }
        private TransmissionData _TransmissionData;

        public int DataLength
        {
            get { return _DataLength; }
            internal set { _DataLength = value; }
        }
        private int _DataLength;

        public byte[] Buffer
        {
            get { return _Buffer; }
        }
        private byte[] _Buffer;

        public DataPacket()
        {

        }
        public DataPacket(short type)
        {
            DataContext.CheckedType(type);
            this._Type = type;
        }

        public byte[] CreateData(string data)
        {
            _TransmissionData = TransmissionData.Text;
            _Buffer = Encoding.UTF8.GetBytes(data);
            _DataLength = _Buffer.Length;
            return CreateData();
        }

        public byte[] CreateData(byte[] data)
        {
            _TransmissionData = TransmissionData.Binary;
            _Buffer = data;
            _DataLength = _Buffer.Length;
            return CreateData();
        }
        
        private byte[] CreateData()
        {
            // 1. 4Byte = 2Byte [ Type(0-32767) ] + 1Byte [ TransmissionData(0-1) + 1Byte [ Length ]]
            // 2. 4Byte = 4Byte [ DataLength ]
            // .... Total 48Byte = DataContext.DataPacketHeaderLength
            // 3. Data ..

            // 시작 데이터 타입 + 데이터 전송 형식(Text,Binary) + 해당 패킷 헤더 길이
            int offset = 0;

            byte[] destHeaderBuffer1 = new byte[4];
            
            // 2Byte
            byte[] sourceTypeBuffer = BitConverter.GetBytes(_Type);
            Array.Copy(sourceTypeBuffer, 0, destHeaderBuffer1, 0, sourceTypeBuffer.Length);
            offset += sourceTypeBuffer.Length;

            // 1Byte
            destHeaderBuffer1[offset] = (byte)_TransmissionData;
            offset++;

            // 실제 전송되는 데이터 길이
            byte[] destHeaderBuffer2 = BitConverter.GetBytes(_DataLength);

            // 실제 전송되는 데이터 생성 [1. 시작 데이터 타입 + 2. 실제 전송되는 데이터 길이 + 3. 데이터 ]
            byte[] sourceCreateBuffer = new byte[destHeaderBuffer1.Length + destHeaderBuffer2.Length + _Buffer.Length];

            // 1. 시작 데이터 타입 복사
            Array.Copy(destHeaderBuffer1, 0, sourceCreateBuffer, 0, destHeaderBuffer1.Length);
            
            // 2. 실제 전송되는 데이터 길이 복사
            Array.Copy(destHeaderBuffer2, 0, sourceCreateBuffer, destHeaderBuffer1.Length, destHeaderBuffer2.Length);

            // 3. 실제 데이터 복사
            Array.Copy(_Buffer, 0, sourceCreateBuffer, destHeaderBuffer1.Length + destHeaderBuffer2.Length, _Buffer.Length);

            return sourceCreateBuffer;
        }

        public static DataPacket ToDataPacketHeader(byte[] dataPacketHeader)
        {
            DataPacket packet = new DataPacket();
            // 2Byte
            packet.Type = BitConverter.ToInt16(dataPacketHeader, 0);
            // 1Byte
            packet.TransmissionData = (TransmissionData)dataPacketHeader[2];

            // 1Byte 후에
            packet.DataLength = BitConverter.ToInt32(dataPacketHeader, DataPacket.DataLengthPos);
            return packet;
        }

        public static byte[] ToHeaderBytes(short type, byte[] data)
        {
            byte[] destBuffer = DataContext.NewDataPacketHeaderBuffer();
            int offset = 0;

            byte[] sourceTypeBuffer = BitConverter.GetBytes(type);
            Array.Copy(sourceTypeBuffer, offset, destBuffer, offset, sourceTypeBuffer.Length);
            offset += sourceTypeBuffer.Length;

            destBuffer[offset] = (byte)TransmissionData.Text;
            offset++;

            // 5번째부터 넣음
            offset++;
            byte[] sourceDataLengthBuffer = BitConverter.GetBytes(data.Length);
            Array.Copy(sourceDataLengthBuffer, 0, destBuffer, offset, sourceDataLengthBuffer.Length);

            return destBuffer;
        }

        public static byte[] ToHeaderBytes(short type, string data)
        {
            byte[] destBuffer = DataContext.NewDataPacketHeaderBuffer();
            int offset = 0;
            
            byte[] sourceTypeBuffer = BitConverter.GetBytes(type);
            Array.Copy(sourceTypeBuffer, offset, destBuffer, offset, sourceTypeBuffer.Length);
            offset += sourceTypeBuffer.Length;

            destBuffer[offset] = (byte)TransmissionData.Text;
            offset++;

            // 5번째부터 넣음
            offset++;
            byte[] sourceDataLengthBuffer = BitConverter.GetBytes(Encoding.UTF8.GetBytes(data).Length);
            Array.Copy(sourceDataLengthBuffer, 0, destBuffer, offset, sourceDataLengthBuffer.Length);

            return destBuffer;
        }
    }
}