using RabbitMQ.Client;

namespace CentralConfiguration.MessageBroker
{
    public class RabbitMqConnectionManager
    {
        private static readonly string _hostName = "localhost";
        private static ConnectionFactory _connectionFactory;
        private static IConnection _connection;
        public static IConnection GetRabbitMqConnection()
        {
            if (_connectionFactory == null)
            {
                _connectionFactory = new ConnectionFactory()
                {
                    HostName = _hostName
                };

            }
            return _connection ?? (_connection = _connectionFactory.CreateConnection());
        }
    }
}
