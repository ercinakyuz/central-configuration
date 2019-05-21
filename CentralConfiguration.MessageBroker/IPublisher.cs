using RabbitMQ.Client;

namespace CentralConfiguration.MessageBroker
{
    public interface IPublisher<in T>
    {
        void Initialize(string connectionString);
        void SendModelToQueue(T model, QueueDeclaration declaration, IBasicProperties properties = null);
    }
}
