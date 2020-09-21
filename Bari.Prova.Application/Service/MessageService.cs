using Bari.Prova.Application.Interface;
using Bari.Prova.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using Bari.Prova.Queueing;
using System.Runtime.Serialization;

namespace Bari.Prova.Application.Service
{
    public class MessageService : IMessageService
    {
        private readonly IQueueBus _queueBus;
        public event EventHandler Listener;

        public MessageService()
        {
            _queueBus = QueueBusFactory.Create("localhost", null, null);
            _queueBus.QueueName = "Bari.Prova";
            _queueBus.Exchange = "Bari";
        }

        public Message ReceiveMessage()
        {
            return _queueBus.ReceiveMessage<Message>();
        }

        public void SendMessage(Message message)
        {
            _queueBus.Publish(message);
        }

        public void StartListener()
        {
            _queueBus.Listener += (obj, e) => Listener.Invoke(obj, e);
            _queueBus.StartListener();
        }
    }
}
