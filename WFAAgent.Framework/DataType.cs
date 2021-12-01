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
        private int _Type;
        private DataType(int type)
        {
            DataContext.CheckedType(type);
            this._Type = type;
        }

        public int Type
        {
            get { return _Type; }
        }
    }
}
