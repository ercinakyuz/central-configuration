namespace CentralConfiguration.Cms.Data.Entity
{
    public interface IEntity<T>
    {
        T Id { get; set; }
    }
}
