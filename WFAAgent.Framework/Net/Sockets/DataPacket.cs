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
        public const int HeaderLength = 255;
        public ushort Type
        {
            get { return _Type; }
            internal set { _Type = value; }
        }
        private ushort _Type;
        
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
        public DataPacket(ushort type)
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
            // 1. 첫 번째 헤더 4Byte = 2Byte [ Type(0-65535) ] + 1Byte [ TransmissionData(0-1) ] + 1Byte [ 255 ]
            // 2. 두 번째 헤더 4Byte = 4Byte [ DataLength ]
            // == HeaderLength End 255 ==
            // 3. Data ..

            byte[] newHeaderBuffer = DataContext.NewDataPacketHeaderBuffer();

            int header1Index = 0;
            byte[] sourceHeaderBuffer1 = new byte[4];
            
            // 2Byte=Type
            byte[] typeBuffer = BitConverter.GetBytes(_Type);
            Array.Copy(typeBuffer, 0, sourceHeaderBuffer1, 0, typeBuffer.Length);
            header1Index += typeBuffer.Length;

            // 1Byte=TransmissionData
            sourceHeaderBuffer1[header1Index] = (byte)_TransmissionData;
            header1Index++;

            // 1Byte=HeaderLength [추후 변경이 필요시 3번째 요소에 값 수정]
            sourceHeaderBuffer1[header1Index] = HeaderLength;
            header1Index++;

            // 4Byte=DataLength
            byte[] sourceHeaderBuffer2 = BitConverter.GetBytes(_DataLength);




            ///// 1. 타입 + 헤더길이 + 전송타입(Text,Binary)
            Array.Copy(sourceHeaderBuffer1, 0, newHeaderBuffer, 0, sourceHeaderBuffer1.Length);

            ///// 2. 데이터 길이 복사
            Array.Copy(sourceHeaderBuffer2, 0, newHeaderBuffer, sourceHeaderBuffer1.Length, sourceHeaderBuffer2.Length);

            // 실제 전송되는 버퍼
            byte[] dataBuffer = new byte[newHeaderBuffer.Length + _Buffer.Length];

            ///// 3. 헤더 복사
            Array.Copy(newHeaderBuffer, 0, dataBuffer, 0, newHeaderBuffer.Length);
            ///// 4. 실제 데이터 복사
            Array.Copy(_Buffer, 0, dataBuffer, newHeaderBuffer.Length, _Buffer.Length);

            return dataBuffer;
        }

        public static DataPacket ToDataPacketHeader(byte[] dataPacketHeader)
        {
            DataPacket packet = new DataPacket();
            // 2Byte
            packet.Type = (ushort) BitConverter.ToInt16(dataPacketHeader, 0);
            // 1Byte
            packet.TransmissionData = (TransmissionData)dataPacketHeader[2];

            // 1Byte 후에
            packet.DataLength = BitConverter.ToInt32(dataPacketHeader, DataPacket.DataLengthPos);
            return packet;
        }

        public static byte[] ToHeaderBytes(ushort type, byte[] data)
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

        public static byte[] ToHeaderBytes(ushort type, string data)
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