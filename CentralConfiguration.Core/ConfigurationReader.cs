using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CentralConfiguration.MessageBroker;
using CentralConfiguration.Model;
using Microsoft.Extensions.Options;

namespace CentralConfiguration.Core
{
    public class ConfigurationReader : IConfigurationReader
    {
        private IList<ConfigurationDto> _currentConfigurations;
        private IList<ConfigurationDto> _configurationsSnapshot;

        public ConfigurationReader(IOptionsSnapshot<BaseAppSettings> options)
        {
            _configurationsSnapshot = options.Value.DynamicSettings;
            var staticSettings = options.Value.StaticSettings;
            //Task.Factory.StartNew(() => HandleConfiguration(staticSettings.ApplicationName, staticSettings.ConsumerInterval * 1000));
        }

        private async Task HandleConfiguration(string applicationName, int consumerInterval)
        {
            var consumer = new RabbitMqConsumer<List<ConfigurationDto>>(applicationName);
            while (true)
            {
                var model = consumer.GetModelInQueue(applicationName);
                if (model != null && model.Any())
                {
                    _configurationsSnapshot = model;
                    _currentConfigurations = model;
                }
                else
                {
                    _currentConfigurations = _configurationsSnapshot.Select(x => x.Clone() as ConfigurationDto).ToList();
                }
                await Task.Delay(consumerInterval);
            }
        }

        public T GetValue<T>(string key)
        {
            var returnValue = default(T);
            try
            {
                var configuration = ConfigurationConsumerService.CurrentConfigurations?.FirstOrDefault(x => x.Key == key);
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
