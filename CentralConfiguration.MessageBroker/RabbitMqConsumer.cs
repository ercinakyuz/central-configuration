using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace CentralConfiguration.MessageBroker
{
    public class RabbitMqConsumer<T> : IConsumer<T>
    {
        private readonly IModel _channel;
        public string Host { get; set; }

        public RabbitMqConsumer()
        {
            var connection = RabbitMqConnectionManager.GetRabbitMqConnection(Host);
            _channel = connection.CreateModel();
        }

        public T GetModelInQueue(QueueDeclaration declaration)
        {
            var model = default(T);
            _channel.QueueDeclare(declaration.Name, declaration.IsDurable, declaration.IsExclusive, declaration.HasAutoDelete, declaration.Args);
            var result = _channel.BasicGet(declaration.Name, true);
            if (result != null)
            {
                string data = Encoding.UTF8.GetString(result.Body);
                model = JsonConvert.DeserializeObject<T>(data);
            }

            return model;
        }

    }
}
