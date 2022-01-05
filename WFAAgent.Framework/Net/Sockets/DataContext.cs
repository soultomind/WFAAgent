using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework.Net.Sockets
{
    public class DataContext
    {
        public const ushort Max = ushort.MaxValue - 1;
        public const ushort Min = ushort.MinValue;

        public const int DefaultDataPacketHeaderLength = byte.MaxValue;
        public const int FirstHeaderLength = 8;

        public const ushort AcceptClient = 1;
        public const ushort AgentData = 255;
        public const ushort UnknownData = ushort.MaxValue;

        public const int TempReceiveBufferSize = 8192;
        public static void CheckedType(ushort value)
        {
            if (!(Max >= value && Min <= value))
            {
                throw new ArgumentException("Usage value " +  Min + "-" + Max);
            }
        }

        public static byte[] NewDefaultDataPacketHeaderBuffer()
        {
            return NewDataBuffer(DefaultDataPacketHeaderLength);
        }

        public static byte[] NewFirstHeaderBuffer()
        {
            return NewDataBuffer(FirstHeaderLength);
        }

        public static byte[] NewDataBuffer(int size)
        {
            return new byte[size];
        }

        public static byte[] NewTempReceiveBuffer()
        {
            return NewDataBuffer(TempReceiveBufferSize);
        }

        
    }
}
