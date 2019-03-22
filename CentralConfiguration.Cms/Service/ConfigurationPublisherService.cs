using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CentralConfiguration.Cms.Data.Entity;
using CentralConfiguration.Cms.Data.Repository;
using CentralConfiguration.MessageBroker;
using CentralConfiguration.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace CentralConfiguration.Cms.Service
{
    internal class ConfigurationPublisherService : IHostedService, IDisposable
    {
        private readonly IPublisher<List<ConfigurationDto>> _publisher;
        private readonly IRepository<Configuration, string> _configurationRepository;
        private readonly int _refreshTimeIntervalInSeconds;
        private Timer _timer;

        public ConfigurationPublisherService(IRepository<Configuration, string> configurationRepository, IPublisher<List<ConfigurationDto>> publisher, IConfiguration configuration)
        {
            _publisher = publisher;
            _configurationRepository = configurationRepository;
            if (!int.TryParse(configuration["ConfigurationPublisherService:RefreshTimeIntervalInSeconds"], out _refreshTimeIntervalInSeconds))
            {
                _refreshTimeIntervalInSeconds = 10;
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(PublishQueue, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(_refreshTimeIntervalInSeconds));
            return Task.CompletedTask;
        }

        private void PublishQueue(object state)
        {
            var getConfigurationsTask = _configurationRepository.GetListAsync(x => x.IsActive);
            getConfigurationsTask.Wait();
            var groupedConfigurations = getConfigurationsTask.Result.GroupBy(x => x.ApplicationName);
            foreach (var configurationGroup in groupedConfigurations)
            {
                var configurationsToQueue = configurationGroup.Select(config => new ConfigurationDto
                {
                    Key = config.Key,
                    Value = config.Value,
                    Type = config.Type,
                }).ToList();
                var queueDeclaration = new QueueDeclaration
                {
                    Name = configurationGroup.Key,
                };
                //if (!queueDeclaration.Args.ContainsKey("x-expires"))
                //{
                //    queueDeclaration.Args.Add("x-expires", 1000 * _refreshTimeIntervalInSeconds);
                //}               
                _publisher.SendModelToQueue(queueDeclaration, configurationsToQueue);
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
