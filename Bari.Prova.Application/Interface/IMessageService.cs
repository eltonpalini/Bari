using Bari.Prova.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bari.Prova.Application.Interface
{
    public interface IMessageService
    {
        void SendMessage(Message message);
        Message ReceiveMessage();
        void StartListener();
        event EventHandler Listener;
    }
}
