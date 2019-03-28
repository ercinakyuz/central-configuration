namespace CentralConfiguration.MessageBroker
{
    public interface IConsumer<out T>
    {
        string Host { get; set; }
        T GetModelInQueue(QueueDeclaration declaration);
    }
}
