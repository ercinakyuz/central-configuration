using CentralConfiguration.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace CentralConfiguration.ApiClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IConfigurationReader _configurationReader;

        public ValuesController(IOptionsSnapshot<AppSettings> appSettingsSnapshot, IConfiguration configuration)
        {
            _configurationReader = new ConfigurationReader(appSettingsSnapshot);
        }
        // GET api/values
        [HttpGet]
        public ActionResult<string> Get(string key)
        {
            return _configurationReader.GetValue<string>(key);
        }

    }
}
