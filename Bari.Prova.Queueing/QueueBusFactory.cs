using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bari.Prova.Queueing
{
    public class QueueBusFactory
    {
        public static IQueueBus Create(string address, string userName, string password)
        {
            //TODO: Implementar outros tipos de QueueBus
            return new Rabbit.QueueBusRabbit(address, userName, password);
        }
    }
}
