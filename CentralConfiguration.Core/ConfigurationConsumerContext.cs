using System.Collections.Generic;
using CentralConfiguration.Model;

namespace CentralConfiguration.Core
{
    public class ConfigurationConsumerContext
    {
        public IList<ConfigurationDto> CurrentConfigurations { get; set; }
    }
}
