using RabbitMQ.Client;

namespace CentralConfiguration.MessageBroker
{
    public interface IPublisher<in T>
    {
        void SendModelToQueue(T model, QueueDeclaration declaration, IBasicProperties properties = null);
    }
}
