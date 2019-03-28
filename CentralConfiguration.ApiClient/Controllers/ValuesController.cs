using CentralConfiguration.Core;
using CentralConfiguration.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CentralConfiguration.ApiClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IConfigurationReader _configurationReader;

        public ValuesController(IConfigurationReader configurationReader)
        {
            _configurationReader = configurationReader;
        }
        [HttpGet]
        public ActionResult Get(string key, string type)
        {
            dynamic value = null;
            if (type == "string")
            {
                value = _configurationReader.GetValue<string>(key);
            }
            else if (type == "bool")
            {
                value = _configurationReader.GetValue<bool>(key);
            }
            else if (type == "int")
            {
                value = _configurationReader.GetValue<int>(key);
            }

            return Ok(value);
        }

    }
}
