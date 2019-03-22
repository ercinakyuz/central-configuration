namespace CentralConfiguration.Model
{
    public class ConfigurationDto : BaseDto
    {
        public string Key { get; set; }
        public string Type { get; set; }
        public object Value { get; set; }
    }
}
