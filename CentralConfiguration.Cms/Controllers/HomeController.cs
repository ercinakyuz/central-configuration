using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CentralConfiguration.Cms.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(IConfiguration config)
        {
        }

        [Route("")]
        public IActionResult Index()
        {
            return View();
        }


    }
}
