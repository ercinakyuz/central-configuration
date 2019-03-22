namespace CentralConfiguration.MessageBroker
{
    public interface IPublisher<in T>
    {
        void SendModelToQueue(QueueDeclaration declaration, T model);
    }
}
