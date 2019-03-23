using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace CentralConfiguration.MessageBroker
{
    public class RabbitMqConnectionManager
    {
        private static readonly string HostName = "localhost";
        private static ConnectionFactory _connectionFactory;
        private static IConnection _connection;
        public static IConnection GetRabbitMqConnection(IConfiguration configuration)
        {
            if (_connectionFactory == null)
            {
                _connectionFactory = new ConnectionFactory
                {
                    HostName = configuration["RabbitMqConnection:Host"] ?? HostName
                };

            }
            return _connection ?? (_connection = _connectionFactory.CreateConnection());
        }
    }
}
