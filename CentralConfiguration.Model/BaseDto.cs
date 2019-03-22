namespace CentralConfiguration.Model
{
    public abstract class BaseDto
    {
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
