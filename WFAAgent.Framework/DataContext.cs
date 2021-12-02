using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework
{
    public class DataContext
    {
        public const int Max = short.MaxValue;
        public const int Min = short.MinValue;

        public const int UserData = 0x1;

        public static void CheckedType(int value)
        {
            if (!(Max >= value && Min <= value))
            {
                throw new ArgumentException("Usage value " +  Min + "-" + Max);
            }
        }
    }
}
