using System;
using System.Globalization;

namespace WFAAgent.Message
{
    public class MessageItem
    {
        public string Message { get; set; }
        public string NowDateTime { get; private set; }
        public MessageItem()
        {
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
    }
}
