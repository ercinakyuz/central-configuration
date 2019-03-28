using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CentralConfiguration.MessageBroker;
using CentralConfiguration.Model;
using Newtonsoft.Json;

namespace CentralConfiguration.Core
{
    public class ConfigurationReader : IConfigurationReader
    {
        private Timer _timer;
        private static readonly ConfigurationConsumerContext Context = new ConfigurationConsumerContext();
        private readonly ConsumerSettings _consumerSettings;
        private readonly IConsumer<IList<ConfigurationDto>> _consumer;
        private static Task _consumerTask;

        public ConfigurationReader(ConsumerSettings consumerSettings, IConsumer<IList<ConfigurationDto>> consumer)
        {
            _consumer = consumer;
            _consumer.Host = consumerSettings.ConsumerHost;
            _consumerSettings = consumerSettings;
            if (_consumerTask == null)
            {
                _consumerTask = Task.Factory.StartNew(StartAsync);
            }
            
        }
        public Task StartAsync()
        {
            _timer = new Timer(ConsumeQueue, null, TimeSpan.Zero, TimeSpan.FromSeconds(_consumerSettings.ConsumerInterval));
            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
        private void ConsumeQueue(object state)
        {
            var queueDeclaration = new QueueDeclaration
            {
                Name = _consumerSettings.ApplicationName,
            };
            try
            {
                var model = _consumer.GetModelInQueue(queueDeclaration);
                if (model != null && model.Any())
                {
                    Context.CurrentConfigurations = model.Select(x => x.Clone() as ConfigurationDto).ToList();
                    SerializeToFile(_consumerSettings.LocalSettingsPath, new LocalSettings { Configurations = model });
                }
                else
                {
                    var localSettings = DeserializeFileContent<LocalSettings>(_consumerSettings.LocalSettingsPath);
                    Context.CurrentConfigurations = localSettings?.Configurations?.Select(x => x.Clone() as ConfigurationDto).ToList();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public T GetValue<T>(string key)
        {
            var returnValue = default(T);
            try
            {
                var configuration = Context.CurrentConfigurations?.FirstOrDefault(x => x.Key == key);
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
            }

            return returnValue;
        }
        private T DeserializeFileContent<T>(string filePath)
        {
            var settingsString = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(settingsString);
        }

        private void SerializeToFile<T>(string filePath, T model)
        {
            var content = JsonConvert.SerializeObject(model);
            File.WriteAllText(filePath, content);
        }

    }
}
