using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework.Net.Sockets
{
    public class Header
    {
        public ushort Type
        {
            get { return _Type; }
            internal set { _Type = value; }
        }
        private ushort _Type = DataContext.AgentData;

        public TransmissionData TransmissionData
        {
            get { return _TransmissionData; }
            internal set { _TransmissionData = value; }
        }
        private TransmissionData _TransmissionData = TransmissionData.Text;

        public int DataLength
        {
            get { return _DataLength; }
            internal set { _DataLength = value; }
        }
        private int _DataLength;
    }
}
