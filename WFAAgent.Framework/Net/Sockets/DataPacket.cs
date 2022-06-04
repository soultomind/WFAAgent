using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework.Net.Sockets
{
    /// <summary>
    /// TCP/IP 소켓 데이터 패킷 클래스
    /// <para>1. AppId = 36Byte</para>
    /// <para>2. ProcessId = 4Byte</para>
    /// <para></para>
    /// </summary>
    public class DataPacket
    {
        public static DataPacket AcceptClient = new DataPacket(DataContext.AcceptClient);
        public static DataPacket AgentStringData = new DataPacket(DataContext.AgentStringData);
        public static DataPacket AgentBinaryData = new DataPacket(DataContext.AgentBinaryData);
        
        public static DataPacket[] Packets = new DataPacket[]
        {
            AcceptClient, AgentStringData, AgentBinaryData
        };

        /// <summary>
        /// AppId 속성 값 길이
        /// </summary>
        public const int AppIdBufferLength = 36;

        /// <summary>
        /// ProcessId 속성 값 길이
        /// </summary>
        public const int ProcessIdBufferLength = 4;
        
        /// <summary>
        /// 패킷 헤더
        /// </summary>
        public Header Header
        {
            get { return _Header; }
            internal set { _Header = value; }
        }
        private Header _Header = new Header();

        /// <summary>
        /// 실제 전송 데이터
        /// </summary>
        public byte[] Buffer
        {
            get { return _Buffer; }
            set { _Buffer = value; }
        }
        private byte[] _Buffer;

        #region Constructor

        private DataPacket(Header header)
        {
            DataContext.CheckedType(header.Type);
            this.Header = header;
        }

        public DataPacket(ushort type)
        {
            DataContext.CheckedType(type);
            this.Header.Type = type;
        }
        public DataPacket(ushort type, string appId)
            : this(type)
        {
            this.Header.AppId = appId;
            this.Header.ProcessId = new Random().Next(1000, 10000);
        }

        public DataPacket(ushort type, string appId, int processId)
            : this(type)
        {
            this.Header.AppId = appId;
            this.Header.ProcessId = processId;
        }

        #endregion

        #region Method

        #region Private

        private byte[] CreatePacket(string data)
        {
            return CreatePacket(Header.AppId, Header.ProcessId, Header.Type, data);
        }

        private byte[] CreatePacket(byte[] data)
        {
            return CreatePacket(Header.AppId, Header.ProcessId, Header.Type, data);
        }

        private byte[] CreatePacket(string data, out int dataLength)
        {
            return CreatePacket(Header.AppId, Header.ProcessId, Header.Type, data, out dataLength);
        }

        private byte[] CreatePacket(byte[] data, out int dataLength)
        {
            return CreatePacket(Header.AppId, Header.ProcessId, Header.Type, data, out dataLength);
        }

        private byte[] CreatePacket(string appId, int processId, ushort type, string data)
        {
            byte[] packetBytes = new byte[DataContext.DefaultDataPacketHeaderLength + data.Length];

            int offset = 0;

            // AppId
            int appIdLength = appId.Length;
            byte[] appIdBytes = Encoding.UTF8.GetBytes(appId);
            System.Buffer.BlockCopy(appIdBytes, 0, packetBytes, offset, appIdBytes.Length);
            offset += appIdBytes.Length;

            // ProcessId
            byte[] processIdBytes = BitConverter.GetBytes(processId);
            offset += processIdBytes.Length;

            // Type
            byte[] typeBytes = BitConverter.GetBytes(type);
            System.Buffer.BlockCopy(typeBytes, 0, packetBytes, offset, typeBytes.Length);
            offset += typeBytes.Length;

            // TransmissionData
            packetBytes[offset] = (byte)TransmissionData.Text;
            offset++;

            // Length
            byte[] dataLengthBytes = BitConverter.GetBytes(Encoding.UTF8.GetBytes(data).Length);
            System.Buffer.BlockCopy(dataLengthBytes, 0, packetBytes, offset, dataLengthBytes.Length);
            offset += dataLengthBytes.Length;

            // Data 
            _Buffer = Encoding.UTF8.GetBytes(data);
            System.Buffer.BlockCopy(_Buffer, 0, packetBytes, DataContext.DefaultDataPacketHeaderLength, _Buffer.Length);

            Header.DataLength = _Buffer.Length;
            return packetBytes;
        }

        private byte[] CreatePacket(string appId, int processId, ushort type, byte[] data)
        {
            byte[] packetBytes = new byte[DataContext.DefaultDataPacketHeaderLength + data.Length];

            int offset = 0;

            // AppId
            int appIdLength = appId.Length;
            byte[] appIdBytes = Encoding.UTF8.GetBytes(appId);
            System.Buffer.BlockCopy(appIdBytes, 0, packetBytes, offset, appIdBytes.Length);
            offset += appIdBytes.Length;

            // ProcessId
            byte[] processIdBytes = BitConverter.GetBytes(processId);
            System.Buffer.BlockCopy(processIdBytes, 0, packetBytes, offset, processIdBytes.Length);
            offset += processIdBytes.Length;

            // Type
            byte[] typeBytes = BitConverter.GetBytes(type);
            System.Buffer.BlockCopy(typeBytes, 0, packetBytes, offset, typeBytes.Length);
            offset += typeBytes.Length;

            // TransmissionData
            packetBytes[offset] = (byte)TransmissionData.Binary;
            offset++;

            // Length
            byte[] dataLengthBytes = BitConverter.GetBytes(data.Length);
            System.Buffer.BlockCopy(dataLengthBytes, 0, packetBytes, offset, dataLengthBytes.Length);
            offset += dataLengthBytes.Length;

            // Data 
            _Buffer = data;
            System.Buffer.BlockCopy(_Buffer, 0, packetBytes, DataContext.DefaultDataPacketHeaderLength, _Buffer.Length);

            Header.DataLength = _Buffer.Length;
            return packetBytes;
        }

        private byte[] CreatePacket(string appId, int processId, ushort type, string data, out int dataLength)
        {
            // Data 
            _Buffer = Encoding.UTF8.GetBytes(data);

            byte[] packetBytes = new byte[DataContext.DefaultDataPacketHeaderLength + _Buffer.Length];

            int offset = 0;

            // AppId
            int appIdLength = appId.Length;
            byte[] appIdBytes = Encoding.UTF8.GetBytes(appId);
            System.Buffer.BlockCopy(appIdBytes, 0, packetBytes, offset, appIdBytes.Length);
            offset += appIdBytes.Length;

            // ProcessId
            byte[] processIdBytes = BitConverter.GetBytes(processId);
            System.Buffer.BlockCopy(processIdBytes, 0, packetBytes, offset, processIdBytes.Length);
            offset += processIdBytes.Length;

            // Type
            byte[] typeBytes = BitConverter.GetBytes(type);
            System.Buffer.BlockCopy(typeBytes, 0, packetBytes, offset, typeBytes.Length);
            offset += typeBytes.Length;

            // TransmissionData
            packetBytes[offset] = (byte)TransmissionData.Text;
            offset++;

            // Length
            byte[] dataLengthBytes = BitConverter.GetBytes(Encoding.UTF8.GetBytes(data).Length);
            System.Buffer.BlockCopy(dataLengthBytes, 0, packetBytes, offset, dataLengthBytes.Length);
            offset += dataLengthBytes.Length;

            // Data
            System.Buffer.BlockCopy(_Buffer, 0, packetBytes, DataContext.DefaultDataPacketHeaderLength, _Buffer.Length);

            dataLength = _Buffer.Length;
            Header.DataLength = dataLength;
            return packetBytes;
        }

        private byte[] CreatePacket(string appId, int processId, ushort type, byte[] data, out int dataLength)
        {
            byte[] packetBytes = new byte[DataContext.DefaultDataPacketHeaderLength + data.Length];

            int offset = 0;

            // AppId
            int appIdLength = appId.Length;
            byte[] appIdBytes = Encoding.UTF8.GetBytes(appId);
            System.Buffer.BlockCopy(appIdBytes, 0, packetBytes, offset, appIdBytes.Length);
            offset += appIdBytes.Length;

            // ProcessId
            byte[] processIdBytes = BitConverter.GetBytes(processId);
            System.Buffer.BlockCopy(processIdBytes, 0, packetBytes, offset, processIdBytes.Length);
            offset += processIdBytes.Length;

            // Type
            byte[] typeBytes = BitConverter.GetBytes(type);
            System.Buffer.BlockCopy(typeBytes, 0, packetBytes, offset, typeBytes.Length);
            offset += typeBytes.Length;

            // TransmissionData
            packetBytes[offset] = (byte)TransmissionData.Binary;
            offset++;

            // Length
            byte[] dataLengthBytes = BitConverter.GetBytes(data.Length);
            System.Buffer.BlockCopy(dataLengthBytes, 0, packetBytes, offset, dataLengthBytes.Length);
            offset += dataLengthBytes.Length;

            // Data 
            _Buffer = data;
            System.Buffer.BlockCopy(_Buffer, 0, packetBytes, DataContext.DefaultDataPacketHeaderLength, _Buffer.Length);

            dataLength = _Buffer.Length;
            Header.DataLength = dataLength;
            return packetBytes;
        }
        #endregion

        public byte[] ToPacketBytes(string data)
        {
            this.Header.TransmissionData = TransmissionData.Text;

            int dataLength = 0;
            byte[] dataPacketBytes = CreatePacket(Header.AppId, Header.ProcessId, Header.Type, data, out dataLength);
            this.Header.DataLength = dataLength;

            return dataPacketBytes;
        }

        public byte[] ToPacketBytes(byte[] data)
        {
            this.Header.TransmissionData = TransmissionData.Binary;

            int dataLength = 0;
            byte[] dataPacketBytes = CreatePacket(Header.AppId, Header.ProcessId, Header.Type, data, out dataLength);
            this.Header.DataLength = dataLength;

            return dataPacketBytes;
        }

        #region Static

        private static byte[] ToAppIdBytes(byte[] dataPacketHeader, int startIndex, int endIndex)
        {
            int length = endIndex - startIndex;
            byte[] buffer = new byte[length];

            for (int i = startIndex; i < buffer.Length; i++)
            {
                buffer[i] = dataPacketHeader[i];
            }
            return buffer;
        }

        private static byte[] ToProcessIdBytes(byte[] dataPacketHeader, int startIndex, int endIndex)
        {
            // 프로세스 ID 값 범위는 1 - 99999 추정됨

            int length = endIndex - startIndex;
            if (length != 4)
            {
                throw new InvalidOperationException("Length != 4");
            }

            byte[] buffer = new byte[length];

            int bufferIndex = 0;
            for (int i = startIndex; i < endIndex; i++)
            {
                buffer[bufferIndex] = dataPacketHeader[i];
                bufferIndex++;
            }
            return buffer;
        }

        public static Header ToHeader(byte[] dataPakcetBytes)
        {
            byte[] appIdBytes = ToAppIdBytes(dataPakcetBytes, 
                0, 
                AppIdBufferLength);

            byte[] processIdBytes = ToProcessIdBytes(dataPakcetBytes,
                appIdBytes.Length,
                appIdBytes.Length + ProcessIdBufferLength);

            Header header = new Header();
            // 35Byte
            header.AppId = Encoding.UTF8.GetString(appIdBytes);
            int startIndex = appIdBytes.Length;

            header.ProcessId = BitConverter.ToInt32(processIdBytes, 0);
            startIndex += processIdBytes.Length;

            // 2Byte
            header.Type = (ushort)BitConverter.ToInt16(dataPakcetBytes, startIndex);
            startIndex += 2;

            // 1Byte
            header.TransmissionData = (TransmissionData)dataPakcetBytes[startIndex];
            startIndex += 1;

            // 4Byte
            header.DataLength = BitConverter.ToInt32(dataPakcetBytes, startIndex);
            startIndex += 4;

            return header;
        }

        public static Header ToHeader(byte[] dataPakcetBytes, out int startIndex)
        {
            byte[] appIdBytes = ToAppIdBytes(dataPakcetBytes, 0, AppIdBufferLength);
            byte[] processIdBytes = ToProcessIdBytes(dataPakcetBytes, AppIdBufferLength, AppIdBufferLength + ProcessIdBufferLength);

            Header header = new Header();
            // 35Byte
            header.AppId = Encoding.UTF8.GetString(appIdBytes);
            startIndex = appIdBytes.Length;

            header.ProcessId = BitConverter.ToInt32(processIdBytes, 0);
            startIndex += processIdBytes.Length;

            // 2Byte
            header.Type = (ushort)BitConverter.ToInt16(dataPakcetBytes, startIndex);
            startIndex += 2;

            // 1Byte
            header.TransmissionData = (TransmissionData)dataPakcetBytes[startIndex];
            startIndex += 1;

            // 4Byte
            header.DataLength = BitConverter.ToInt32(dataPakcetBytes, startIndex);
            startIndex += 4;

            return header;
        }

        public static DataPacket ToPacket(byte[] dataPakcetBytes)
        {
            Header header = ToHeader(dataPakcetBytes);
            DataPacket dp = new DataPacket(header);

            int size = dataPakcetBytes.Length - DataContext.DefaultDataPacketHeaderLength;
            byte[] buffer = new byte[size];

            int bufferIndex = 0;
            for (int dataIndex = DataContext.DefaultDataPacketHeaderLength; dataIndex < dataPakcetBytes.Length; dataIndex++)
            {
                buffer[bufferIndex] = dataPakcetBytes[dataIndex];
                bufferIndex++;
            }
            dp.Buffer = buffer;
            return dp;
        }

        public static byte[] ToDataPacketBytes(string appId, ushort type, byte[] data)
        {
            DataPacket dp = new DataPacket(type, appId);
            int dataLength = 0;
            byte[] packet = dp.CreatePacket(data, out dataLength);
            return packet;
        }

        public static byte[] ToDataPacketBytes(string appId, ushort type, string data)
        {
            DataPacket dp = new DataPacket(type, appId);
            int dataLength = 0;
            byte[] packet = dp.CreatePacket(data, out dataLength);
            return packet;
        }

        #endregion

        #endregion
    }
}