using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace CentralConfiguration.MessageBroker
{
    public class RabbitMqPublisher<T> : IPublisher<T>
    {
        private readonly IConnection _connection;

        public RabbitMqPublisher()
        {
            _connection = RabbitMqConnectionManager.GetRabbitMqConnection();
        }

        public void SendModelToQueue(QueueDeclaration declaration, T model)
        {
            using (var channel = _connection.CreateModel())
            {
                channel.QueueDeclare(declaration.Name, declaration.IsDurable, declaration.IsExclusive, declaration.HasAutoDelete, declaration.Args);
                channel.BasicPublish(string.Empty, declaration.Name, false, null, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(model)));
            }
        }
    }
}
