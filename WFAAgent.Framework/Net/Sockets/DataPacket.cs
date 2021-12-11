using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework.Net.Sockets
{
    public class DataPacket
    {
        public static DataPacket AcceptClient = new DataPacket(DataContext.AcceptClient);
        public static DataPacket UserData = new DataPacket(DataContext.UserData);

        public const int DataLengthPos = 4;
        public const int HeaderLength = 255;
        public Header Header
        {
            get { return _Header; }
            internal set { _Header = value; }
        }
        private Header _Header = new Header();
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
            this.Header.Type = type;
        }

        public byte[] CreateData(string data)
        {
            this.Header.TransmissionData = TransmissionData.Text;
            _Buffer = Encoding.UTF8.GetBytes(data);
            this.Header.DataLength = _Buffer.Length;
            return CreateData();
        }

        public byte[] CreateData(byte[] data)
        {
            this.Header.TransmissionData = TransmissionData.Binary;
            _Buffer = data;
            this.Header.DataLength = _Buffer.Length;
            return CreateData();
        }
        
        private byte[] CreateData()
        {
            // 1. 첫 번째 헤더 4Byte = 2Byte [ Type(0-65535) ] + 1Byte [ TransmissionData(0-1) ] + 1Byte [ 255 ]
            // 2. 두 번째 헤더 4Byte = 4Byte [ DataLength ]
            // == HeaderLength End 255 ==
            // 3. Data ..

            byte[] newHeaderBuffer = DataContext.NewDefaultDataPacketHeaderBuffer();

            int header1Index = 0;
            byte[] sourceHeaderBuffer1 = new byte[4];
            
            // 2Byte=Type
            byte[] typeBuffer = BitConverter.GetBytes(Header.Type);
            Array.Copy(typeBuffer, 0, sourceHeaderBuffer1, 0, typeBuffer.Length);
            header1Index += typeBuffer.Length;

            // 1Byte=TransmissionData
            sourceHeaderBuffer1[header1Index] = (byte)Header.TransmissionData;
            header1Index++;

            // 1Byte=HeaderLength [추후 변경이 필요시 3번째 요소에 값 수정]
            sourceHeaderBuffer1[header1Index] = HeaderLength;
            header1Index++;

            // 4Byte=DataLength
            byte[] sourceHeaderBuffer2 = BitConverter.GetBytes(Header.DataLength);




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

        public static Header ToHeader(byte[] dataPacketHeader)
        {
            Header header = new Header();
            // 2Byte
            header.Type = (ushort) BitConverter.ToInt16(dataPacketHeader, 0);
            // 1Byte
            header.TransmissionData = (TransmissionData)dataPacketHeader[2];

            // 1Byte 후에
            header.DataLength = BitConverter.ToInt32(dataPacketHeader, DataPacket.DataLengthPos);
            return header;
        }

        public static byte[] ToHeaderBytes(ushort type, byte[] data)
        {
            byte[] destBuffer = DataContext.NewDefaultDataPacketHeaderBuffer();
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
            byte[] destBuffer = DataContext.NewDefaultDataPacketHeaderBuffer();
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