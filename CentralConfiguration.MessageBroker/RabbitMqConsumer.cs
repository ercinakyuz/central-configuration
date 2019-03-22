using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace CentralConfiguration.MessageBroker
{
    public class RabbitMqConsumer<T> : IConsumer<T>
    {
        private IModel _channel;
        private IConnection _connection;

        public RabbitMqConsumer(string queueName)
        {
            Initialize(queueName);
        }

        private void Initialize(string queueName)
        {
            _connection = RabbitMqConnectionManager.GetRabbitMqConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: queueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

        }

        public T GetModelInQueue(string queueName)
        {
            var model = default(T);
            var result = _channel.BasicGet(queueName, true);
            if (result != null)
            {
                string data = Encoding.UTF8.GetString(result.Body);
                model = JsonConvert.DeserializeObject<T>(data);
            }

            return model;
        }

    }
}
