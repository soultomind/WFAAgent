using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Core
{
    public class ItemQueue<T>
    {
        public Queue<T> Messages
        {
            get { return _Messages; }
        }
        private Queue<T> _Messages;

        public ItemQueue()
        {
            _Messages = new Queue<T>();
        }

        public void Enqueue(T message)
        {
            _Messages.Enqueue(message);
        }

        public T Dequeue()
        {
            return _Messages.Dequeue();
        }
    }
}
