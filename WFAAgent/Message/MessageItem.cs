using System;
using System.Globalization;
using WFAAgent.Core;

namespace WFAAgent.Message
{
    public class MessageItem
    {
        public LogLevel LogLevel { get; set; }
        public string Message { get; set; }
        public string NowDateTime { get; private set; }

        /*
        string className = new StackFrame(1).GetMethod().ReflectedType.Name;
        string methodName = new StackFrame(1, true).GetMethod().Name;
        */

        public MessageItem()
        {
            LogLevel = LogLevel.Info;
            Message = String.Empty;
            NowDateTime = NowToString();
        }

        public string NowToString(string format = "yyyy/MM/dd HH:mm:ss")
        {
            return System.DateTime.Now.ToString(format, CultureInfo.InvariantCulture);
        }

        public override string ToString()
        {
            return Message;
        }

        public string MakeMessage()
        {
            return String.Format("{0} {1} : {2}", LogLevel.ToString().ToUpper(), NowDateTime, Message);
        }
    }
}
