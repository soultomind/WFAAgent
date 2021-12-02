using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Core
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

        public EventProcessorManager()
        {
            _EventProcessors = new ConcurrentDictionary<string, IEventProcessor>();
        }

        public bool TryCreate(string assemblyFile, Type abstractClassType, out object instance)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyFile);

            Type findType = null;
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsClass && type.IsSubclassOf(abstractClassType))
                {
                    findType = type;
                    break;
                }
            }

            if (findType != null)
            {
                instance = Activator.CreateInstance(findType);
                return true;
            }
            else
            {
                instance = null;
                return false;
            }
        }

        internal IEventProcessor AddStartsWithByEventNameEventProcessor(string eventName)
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
            return eventProcessor;
        }
    }
}
