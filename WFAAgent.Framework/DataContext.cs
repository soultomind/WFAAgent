using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework
{
    public class DataContext
    {
        public const int Max = 255;
        public const int Min = 0;

        public const int UserData = 0x1;

        public static void CheckedType(int value)
        {
            if (!(Max >= value && Min <= value))
            {
                throw new ArgumentException("Usage value 0 - 255");
            }
        }
    }
}
