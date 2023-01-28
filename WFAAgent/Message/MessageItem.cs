using System;
using System.Globalization;
using WFAAgent.Core;

namespace WFAAgent.Message
{
    public class MessageItem
    {
        public LogLevel LogLevel { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public string NowDateTime { get; private set; }

        public MessageItem()
        {
            LogLevel = LogLevel.Debug;
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
            if (Exception != null)
            {
                Message = Message + "\n" + Exception.ToString();
                return String.Format("{0} {1} : {2}", NowDateTime, LogLevel.ToString().ToUpper(), Message);
            }
            else
            {
                return String.Format("{0} {1} : {2}", NowDateTime, LogLevel.ToString().ToUpper(), Message);
            }
        }
    }
}
