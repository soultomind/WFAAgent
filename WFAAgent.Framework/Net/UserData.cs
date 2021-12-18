using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Framework.Net
{
    public class UserData : DefaultData
    {
        public string AppId { get; set; }
        public string AppData { get; set; }
        public UserData()
        {
            Type = GetType().Name;
        }
    }
}
