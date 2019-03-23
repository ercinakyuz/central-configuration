namespace CentralConfiguration.Model
{
    public abstract class BaseDto
    {
        public BaseDto Clone()
        {
            return MemberwiseClone() as BaseDto;
        }
    }
}
