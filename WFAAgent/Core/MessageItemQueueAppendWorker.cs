using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WFAAgent.Core
{
    public delegate void MessageItemReceivedEventHandler<T>(T item);
    
    public class MessageItemQueueAppendWorker<T>
    {
        public ItemQueue<T> ItemQueue
        {
            get { return _ItemQueue; }
        }
        private ItemQueue<T> _ItemQueue;

        public bool IsStart
        {
            get { return _Start; }
        }
        private volatile bool _Start;
        private Thread _Thread;
        public event MessageItemReceivedEventHandler<T> MessageItemReceived;
        public MessageItemQueueAppendWorker()
        {
            _ItemQueue = new ItemQueue<T>();
        }

        #region ItemQueue
        public void Enqueue(T message)
        {
            _ItemQueue.Messages.Enqueue(message);
        }

        public T Dequeue()
        {
            return _ItemQueue.Messages.Dequeue();
        }

        public void Clear()
        {
            _ItemQueue.Messages.Clear();
        }
        #endregion

        #region Thread

        public void Start()
        {
            _Thread = new Thread(StartWorker);
            _Thread.IsBackground = true;
            _Thread.Start();
        }

        public void Stop()
        {
            StopWorker();
        }

        private void StartWorker()
        {
            _Start = true;
            while (_Start)
            {
                if (_ItemQueue.Messages.Count > 0)
                {
                    T message = _ItemQueue.Messages.Dequeue();
                    MessageItemReceived?.Invoke(message);
                }
            }
        }

        private void StopWorker()
        {
            _Start = false;
            if (_Thread != null)
            {
                _Thread.Join();
                _Thread = null;
            }
        }

        #endregion
    }
}
