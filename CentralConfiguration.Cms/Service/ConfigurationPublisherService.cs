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
    public class ConfigurationPublisherService : IHostedService, IDisposable
    {
        private readonly IPublisher<IList<ConfigurationDto>> _publisher;
        private readonly IRepository<Configuration, string> _configurationRepository;
        private readonly int _publisherInterval;
        private Timer _timer;
        public static IDictionary<string, IList<ConfigurationDto>> ConfigurationsSnapShot { get; set; }

        public ConfigurationPublisherService(IRepository<Configuration, string> configurationRepository, IPublisher<IList<ConfigurationDto>> publisher, IConfiguration configuration)
        {
            _publisher = publisher;
            _publisher.Host = configuration["RabbitMqConnection:Host"];
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
                    if (ConfigurationsSnapShot != null)
                    {
                        if ((ConfigurationsSnapShot.ContainsKey(queueDeclaration.Name) && !configurationsToQueue.SequenceEqual(ConfigurationsSnapShot[queueDeclaration.Name]))
                            || !ConfigurationsSnapShot.ContainsKey(queueDeclaration.Name))
                        {
                            ConfigurationsSnapShot[queueDeclaration.Name] = configurationsToQueue.Select(x => x.Clone() as ConfigurationDto).ToList();
                            _publisher.SendModelToQueue(configurationsToQueue, queueDeclaration);
                        }
                        
                    }
                    else
                    {
                        ConfigurationsSnapShot = new Dictionary<string, IList<ConfigurationDto>>();
                    }


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
