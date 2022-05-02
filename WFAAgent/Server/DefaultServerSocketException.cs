using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Server
{
    public class DefaultServerSocketException : Exception
    {
        public DefaultServerSocketException(string message)
            : base(message)
        {

        }

        public DefaultServerSocketException(string message, Exception innerException) 
            : base(message, innerException)
        {

        }
    }
}
