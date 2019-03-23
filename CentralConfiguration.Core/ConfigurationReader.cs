using System;
using System.Linq;

namespace CentralConfiguration.Core
{
    public class ConfigurationReader : IConfigurationReader
    {
        public ConfigurationReader()
        {
        }

        public T GetValue<T>(string key)
        {
            var returnValue = default(T);
            try
            {
                var configuration = ConfigurationConsumerService.Context.CurrentConfigurations?.FirstOrDefault(x => x.Key == key);
                if (configuration != null)
                {
                    returnValue = Type.GetType(configuration.Type) == typeof(T)
                        ? (T)configuration.Value
                        : Convert.ChangeType(configuration.Value as dynamic, typeof(T));
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return returnValue;
        }

    }
}
