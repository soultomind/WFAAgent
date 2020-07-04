using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework
{
    public class DataType
    {
        public static readonly DataType UserData = new DataType(DataContext.UserData);
        private int value;
        private DataType(int value)
        {
            if (!(DataContext.Max >= value && DataContext.Min <= value))
            {
                throw new ArgumentException("Usage value 0 - 255");
            }
            this.value = value;
        }

        public int Value
        {
            get { return value; }
        }
    }
}
