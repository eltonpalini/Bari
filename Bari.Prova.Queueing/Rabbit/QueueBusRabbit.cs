using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bari.Prova.Queueing.Rabbit
{
    public class QueueBusRabbit : IQueueBus, IDisposable
    {
        public string QueueName { get; set; }

        private string _exchange = null;
        public string Exchange
        {
            get
            {
                if (string.IsNullOrEmpty(_exchange))
                    _exchange = QueueName;

                return _exchange;
            }
            set
            {
                _exchange = value;
            }
        }

        private IConnection _connection = null;
        private IModel _model = null;
        private Dictionary<string, object> _defaultQueueArgs
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { "x-dead-letter-exchange", "" },
                    { "x-dead-letter-routing-key",  $"{QueueName}" }
                };
            }
        }
        private bool _isRejected = false;

        public QueueBusRabbit(string address, string userName, string password)
        {
            var connectionFactory = new ConnectionFactory() { HostName = address };

            if (!string.IsNullOrWhiteSpace(userName))
                connectionFactory.UserName = userName;
            if (!string.IsNullOrEmpty(password))
                connectionFactory.Password = password;

            string idConnection = string.Empty;
            if (Assembly.GetEntryAssembly() != null)
                idConnection = Assembly.GetEntryAssembly().GetName().Name;

            _connection = connectionFactory.CreateConnection(idConnection);
            _model = _connection.CreateModel();
        }

        public event EventHandler Listener;

        public void Publish<T>(T obj)
        {
            try
            {
                var message = JsonConvert.SerializeObject(obj);
                var messageBody = Encoding.UTF8.GetBytes(message);

                _model.ExchangeDeclare(Exchange, "direct", true);
                _model.QueueDeclare(QueueName, true, false, false, _defaultQueueArgs);
                _model.QueueBind(QueueName, Exchange, QueueName);

                var prop = _model.CreateBasicProperties();
                prop.Persistent = true;

                _model.BasicPublish(Exchange, QueueName, false, prop, messageBody);
            }
            catch (Exception e)
            {
                throw new Exception("Error to publish message", e);
            }            
        }

        public T ReceiveMessage<T>()
        {

            _model.QueueDeclare(QueueName, true, false, false, _defaultQueueArgs);
            _model.BasicQos(0, 1, false);

            var msg = _model.BasicGet(QueueName, false);
            try
            {
                if (msg != null)
                {
                    var body = Encoding.UTF8.GetString(msg.Body);
                    var obj = JsonConvert.DeserializeObject(body);

                    _model.BasicAck(msg.DeliveryTag, false);
                    return (T)obj;
                }
                else
                    return default;
            }
            catch (Exception ex)
            {
                _model.BasicNack(msg.DeliveryTag, false, false);
                throw new Exception("Error to get message", ex);
            }
        }

        public void StartListener()
        {
            _model.QueueDeclare(QueueName, true, false, false, _defaultQueueArgs);
            _model.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(_model);
            consumer.Received += (ch, ea) =>
            {
                try
                {
                    _isRejected = false;

                    var message = Encoding.UTF8.GetString(ea.Body);
                    var obj = message;

                    Listener.Invoke(obj, ea);

                    if (!_isRejected)
                        _model.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _model.BasicNack(ea.DeliveryTag, false, false);
                }
            };
            _model.BasicConsume(QueueName, false, consumer);
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
            }
        }

        public void RejectMessage(ulong deliveryTag, bool requeue)
        {
            _model.BasicNack(deliveryTag, false, requeue);
            _isRejected = true;
        }
    }
}
