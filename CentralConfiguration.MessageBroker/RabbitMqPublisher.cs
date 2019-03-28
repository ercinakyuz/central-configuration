using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing;

namespace CentralConfiguration.MessageBroker
{
    public class RabbitMqPublisher<T> : IPublisher<T>
    {
        public string Host { get; set; }
        private readonly IConnection _connection;

        public RabbitMqPublisher()
        {
            _connection = RabbitMqConnectionManager.GetRabbitMqConnection(Host);
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
