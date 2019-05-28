using System;
using System.Threading;
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

            TryConnect();
            return _connection;
        }

        private static void TryConnect()
        {
            int attemptLimit = 3;

            for (int i = 0; i < attemptLimit; i++)
            {
                Thread.Sleep(1000);
                try
                {
                    if (_connection == null || !_connection.IsOpen)
                    {
                        _connection = _connectionFactory.CreateConnection();
                    }

                    if (_connection != null && _connection.IsOpen)
                    {
                        return;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    
                }
            }

        }
    }
}
