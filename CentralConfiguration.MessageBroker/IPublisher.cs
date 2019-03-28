using RabbitMQ.Client;

namespace CentralConfiguration.MessageBroker
{
    public interface IPublisher<in T>
    {
        string Host { get; set; }
        void SendModelToQueue(T model, QueueDeclaration declaration, IBasicProperties properties = null);
    }
}
