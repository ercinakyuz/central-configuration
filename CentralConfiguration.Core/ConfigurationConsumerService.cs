using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CentralConfiguration.MessageBroker;
using CentralConfiguration.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace CentralConfiguration.Core
{
    public class ConfigurationConsumerService : IHostedService, IDisposable
    {
        public static ConfigurationConsumerContext Context = new ConfigurationConsumerContext();
        private readonly int _consumerInterval;
        private readonly string _applicationName;
        private readonly string _localSettingsPath;
        private Timer _timer;
        private readonly IConsumer<IList<ConfigurationDto>> _consumer;

        public ConfigurationConsumerService(IConfiguration configuration, IConsumer<IList<ConfigurationDto>> consumer)
        {
            _consumer = consumer;
            _applicationName = configuration["AppSettings:ApplicationName"];
            _localSettingsPath = configuration["AppSettings:LocalSettingsPath"];
            if (!int.TryParse(configuration["RabbitMqConnection:ConsumerInterval"], out _consumerInterval))
            {
                _consumerInterval = 10;
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(ConsumeQueue, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(_consumerInterval));
            return Task.CompletedTask;
        }

        private void ConsumeQueue(object state)
        {
            var queueDeclaration = new QueueDeclaration
            {
                Name = _applicationName,
            };
            var model = _consumer.GetModelInQueue(queueDeclaration);
            if (model != null && model.Any())
            {
                Context.CurrentConfigurations = model.Select(x => x.Clone() as ConfigurationDto).ToList();
                SerializeToFile(_localSettingsPath, new LocalSettings { Configurations = model });
            }
            else
            {
                var localSettings = DeserializeFileContent<LocalSettings>(_localSettingsPath);
                Context.CurrentConfigurations = localSettings?.Configurations?.Select(x => x.Clone() as ConfigurationDto).ToList();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
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
