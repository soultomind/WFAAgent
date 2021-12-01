using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Core
{
    public class EventProcessorManager
    {
        public ConcurrentDictionary<string, IEventProcessor> EventProcessors
        {
            get { return _EventProcessors; }
        }
        private ConcurrentDictionary<string, IEventProcessor> _EventProcessors;

        public EventProcessorManager()
        {
            _EventProcessors = new ConcurrentDictionary<string, IEventProcessor>();
        }
    }
}
