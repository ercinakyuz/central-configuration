using System;
using RabbitMQ.Client;

namespace CentralConfiguration.MessageBroker
{
    public class RabbitMqConnectionManager
    {
        private static ConnectionFactory _connectionFactory;
        private static IConnection _connection;

        public static IConnection GetRabbitMqConnection(string connnectionString)
        {
            if (_connectionFactory == null)
            {
                _connectionFactory = new ConnectionFactory
                {
                    Uri = new Uri(connnectionString)
                };
            }
            return _connection ?? (_connection = _connectionFactory.CreateConnection());
        }
    }
}
