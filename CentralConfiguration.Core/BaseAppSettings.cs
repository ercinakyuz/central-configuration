using System.Collections.Generic;
using CentralConfiguration.Model;

namespace CentralConfiguration.Core
{
    public class BaseAppSettings
    {
        public StaticSettings StaticSettings { get; set; }
        public IList<ConfigurationDto> DynamicSettings { get; set; }
    }
}
