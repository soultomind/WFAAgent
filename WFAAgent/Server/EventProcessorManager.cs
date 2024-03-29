﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Server
{
    public class EventProcessorManager
    {
        private static Type[] _sTypes = null;
        static EventProcessorManager()
        {
            _sTypes = Assembly.GetAssembly(typeof(EventProcessorManager)).GetTypes();
        }
        public ConcurrentDictionary<string, IEventProcessor> EventProcessors
        {
            get { return _EventProcessors; }
        }
        private ConcurrentDictionary<string, IEventProcessor> _EventProcessors;

        public ProcessStartDataEventProcessor ProcessStartDataEventProcessor { get; private set; }
        public ProcessEventDataEventProcessor ProcessEventDataEventProcessor { get; private set; }

        public EventProcessorManager()
        {
            _EventProcessors = new ConcurrentDictionary<string, IEventProcessor>();
        }

        internal IEventProcessor CreateInstanceStartsWithByEventName(string eventName)
        {
            IEventProcessor eventProcessor = null;
            foreach (Type type in _sTypes)
            {
                if (type.IsClass && type.IsSubclassOf(typeof(EventProcessor)))
                {
                    if (type.Name.StartsWith(eventName))
                    {
                        eventProcessor = Activator.CreateInstance(type) as IEventProcessor;
                        _EventProcessors[eventName] = eventProcessor;
                        break;
                    }
                }
            }

            if (eventProcessor.GetType() == typeof(ProcessStartDataEventProcessor))
            {
                ProcessStartDataEventProcessor = eventProcessor as ProcessStartDataEventProcessor;
            }

            else if (eventProcessor.GetType() == typeof(ProcessEventDataEventProcessor))
            {
                ProcessEventDataEventProcessor = eventProcessor as ProcessEventDataEventProcessor;
            }

            return eventProcessor;
        }
    }
}
