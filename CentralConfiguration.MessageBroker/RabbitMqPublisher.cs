using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace CentralConfiguration.MessageBroker
{
    public class RabbitMqPublisher<T> : IPublisher<T>
    {
        private IConnection _connection;

        public RabbitMqPublisher()
        {
        }

        public void Initialize(string connectionString)
        {
            _connection = RabbitMqConnectionManager.GetRabbitMqConnection(connectionString);
        }
        public void SendModelToQueue(T model, QueueDeclaration declaration, IBasicProperties properties = null)
        {
            using (var channel = _connection.CreateModel())
            {
                channel.QueueDeclare(declaration.Name, declaration.IsDurable, declaration.IsExclusive, declaration.HasAutoDelete, declaration.Args);
                channel.BasicPublish(string.Empty, declaration.Name, false, properties, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(model)));
            }
        }
    }
}
