using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework
{
    public class DataHeader
    {
        private int type;
        private byte[] buffer;
        private TransmissionData transmissionData;

        private int dataLength;
        private int dataOffset;
        
        public DataHeader(int type, byte[] buffer)
            : this(type, buffer, TransmissionData.Text)
        {
            
        }

        public DataHeader(int type, byte[] buffer, TransmissionData transmissionData)
        {
            DataContext.CheckedType(type);
            this.type = type;
            this.buffer = buffer;
            this.transmissionData = transmissionData;

            this.dataLength = buffer.Length;

            this.dataOffset = 0;
        }

        public byte[] CreateBuffer()
        {
            // 1. 4Byte = 1Byte [ Type(0-255) ] 1Byte [ TransmissioNData(0-1) ]
            // 2. 4Byte = 4Byte [ DataLength ]
            // 3. Data ..
            byte[] headerBuffer1 = BitConverter.GetBytes(type);
            // 00000000
            headerBuffer1[1] = (byte)transmissionData;

            byte[] headerBuffer2 = BitConverter.GetBytes(dataLength);

            byte[] createBuffer = new byte[headerBuffer1.Length + headerBuffer2.Length + buffer.Length];
            headerBuffer1[2] = (byte)headerBuffer1.Length;

            Array.Copy(headerBuffer1, 0, createBuffer, 0, headerBuffer1.Length);
            Array.Copy(headerBuffer2, 0, createBuffer, headerBuffer1.Length, headerBuffer2.Length);
            Array.Copy(buffer, 0, createBuffer, headerBuffer1.Length + headerBuffer2.Length, buffer.Length);

            return createBuffer;
        }

        public int Type
        {
            get { return type; }
        }

        public byte[] Buffer
        {
            get { return buffer; }
        }

        public TransmissionData TransmissionData
        {
            get { return transmissionData; }
        }

        public int DataOffset
        {
            get { return dataOffset; }
        }

        public int DataLength
        {
            get { return dataLength; }
        }
    }

    public enum TransmissionData
    {
        Text = 0,
        Binary = 1
    }
}
