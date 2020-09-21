using System;

namespace Bari.Prova.Queueing
{
    public interface IQueueBus : IDisposable
    {
        void Publish<T>(T obj);
        T ReceiveMessage<T>();
        event EventHandler Listener;
        string QueueName { get; set; }
        string Exchange { get; set; }
        void StartListener();
        new void Dispose();
        void RejectMessage(ulong deliveryTag, bool requeue);
    }
}
