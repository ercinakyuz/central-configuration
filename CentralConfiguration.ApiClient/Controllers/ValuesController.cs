using CentralConfiguration.Core;
using Microsoft.AspNetCore.Mvc;

namespace CentralConfiguration.ApiClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IConfigurationReader _configurationReader;

        public ValuesController()
        {
            _configurationReader = new ConfigurationReader();
        }
        // GET api/values
        [HttpGet]
        public IActionResult Get(string key)
        {
            return Content(_configurationReader.GetValue<string>(key));
        }

    }
}
