using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace CentralConfiguration.MessageBroker
{
    public class RabbitMqConsumer<T> : IConsumer<T>
    {
        private IModel _channel;

        public RabbitMqConsumer()
        {
        }
        public void Initialize(string connectionString)
        {
            var connection = RabbitMqConnectionManager.GetRabbitMqConnection(connectionString);
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
