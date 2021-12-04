using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework.Net.Sockets
{
    public class DataContext
    {
        public const int Max = short.MaxValue;
        public const int Min = short.MinValue;

        public const int DataPacketHeaderLength = byte.MaxValue;

        public const int UserData = 0x1;

        public static void CheckedType(short value)
        {
            if (!(Max >= value && Min <= value))
            {
                throw new ArgumentException("Usage value " +  Min + "-" + Max);
            }
        }

        public static byte[] NewDataPacketHeaderBuffer()
        {
            return new byte[DataPacketHeaderLength];
        }

        public static byte[] NewDataBuffer(int size)
        {
            return new byte[size];
        }
    }
}
