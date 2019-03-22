using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CentralConfiguration.MessageBroker;
using CentralConfiguration.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace CentralConfiguration.Core
{
    public class ConfigurationConsumerService : IHostedService, IDisposable
    {
        private readonly int _consumerInterval;
        private readonly string _applicationName;
        public static IList<ConfigurationDto> CurrentConfigurations;
        private IList<ConfigurationDto> _configurationsSnapshot;
        private Timer _timer;

        public ConfigurationConsumerService(IConfiguration configuration,IOptionsSnapshot<BaseAppSettings> options)
        {
            _applicationName = configuration["StaticSettings:ApplicationName"];
            if (!int.TryParse(configuration["StaticSettings:ConsumerInterval"], out _consumerInterval))
            {
                _consumerInterval = 10;
            }
            //var staticSettings = options.Value.StaticSettings;
            //_consumerInterval = staticSettings.ConsumerInterval;
            //_applicationName = staticSettings.ApplicationName;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(ConsumeQueue, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(_consumerInterval));
            return Task.CompletedTask;
        }

        private void ConsumeQueue(object state)
        {
            var consumer = new RabbitMqConsumer<List<ConfigurationDto>>(_applicationName);
            var model = consumer.GetModelInQueue(_applicationName);
            if (model != null && model.Any())
            {
                _configurationsSnapshot = model;
                CurrentConfigurations = model;
            }
            else
            {
                CurrentConfigurations = _configurationsSnapshot?.Select(x => x.Clone() as ConfigurationDto).ToList();
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
    }
}
