using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFAAgent.Framework.Net.Sockets;

namespace WFAAgent.Framework.Net.Sockets
{
    public class DataType
    {
        public static readonly DataType AgentData = new DataType(DataContext.AgentStringData);
        private ushort _Type;
        private DataType(ushort type)
        {
            DataContext.CheckedType(type);
            this._Type = type;
        }

        public ushort Type
        {
            get { return _Type; }
        }
    }
}
