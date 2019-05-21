namespace CentralConfiguration.MessageBroker
{
    public interface IConsumer<out T>
    {
        void Initialize(string connectionString);
        T GetModelInQueue(QueueDeclaration declaration);
    }
}
