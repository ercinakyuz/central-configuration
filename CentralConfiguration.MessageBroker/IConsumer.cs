namespace CentralConfiguration.MessageBroker
{
    public interface IConsumer<out T>
    {
        T GetModelInQueue(QueueDeclaration declaration);
    }
}
