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
using Microsoft.Extensions.Logging;

namespace CentralConfiguration.Cms.Service
{
    public class ConfigurationPublisherService : IHostedService, IDisposable
    {
        private readonly IPublisher<List<ConfigurationDto>> _publisher;
        private readonly IRepository<Configuration, string> _configurationRepository;
        private readonly int _publisherInterval;
        private Timer _timer;
        private static readonly Dictionary<string, IList<ConfigurationDto>> ConfigurationsSnapShot = new Dictionary<string, IList<ConfigurationDto>>();

        public ConfigurationPublisherService(IRepository<Configuration, string> configurationRepository, IPublisher<List<ConfigurationDto>> publisher, IConfiguration configuration)
        {
            _publisher = publisher;
            _configurationRepository = configurationRepository;
            if (!int.TryParse(configuration["RabbitMqConnection:PublisherInterval"], out _publisherInterval))
            {
                _publisherInterval = 10;
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(PublishQueue, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(_publisherInterval));
            return Task.CompletedTask;
        }

        private void PublishQueue(object state)
        {
            try
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
                    //PublisherProperties properties = new PublisherProperties
                    //{
                    //    Expiration = $"{1000 * _publisherInterval}"
                    //};

                    if (ConfigurationsSnapShot.ContainsKey(queueDeclaration.Name) &&
                        !configurationsToQueue.SequenceEqual(ConfigurationsSnapShot[queueDeclaration.Name]))
                    {
                        _publisher.SendModelToQueue(configurationsToQueue, queueDeclaration);
                    }
                    else if (!ConfigurationsSnapShot.ContainsKey(queueDeclaration.Name))
                    {
                        _publisher.SendModelToQueue(configurationsToQueue, queueDeclaration);
                    }

                    ConfigurationsSnapShot[queueDeclaration.Name] = configurationsToQueue;

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
