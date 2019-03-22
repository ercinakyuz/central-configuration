using System.Threading.Tasks;
using CentralConfiguration.Cms.Data.Entity;
using CentralConfiguration.Cms.Data.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CentralConfiguration.Cms.Controllers
{

    public class ConfigurationController : Controller
    {
        private readonly IRepository<Configuration, string> _configurationRepository;
        public ConfigurationController(IConfiguration config, IRepository<Configuration, string> configurationRepository)
        {
            _configurationRepository = configurationRepository;
        }

        public async Task<IActionResult> List()
        {
            var configurations = await _configurationRepository.GetListAsync(x => true);
            return View(configurations);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Configuration model)
        {
            var configuration = await _configurationRepository.InsertOneAsync(model);
            return RedirectToAction("Edit", new { id = configuration.Id });
        }
        public async Task<IActionResult> Edit(string id)
        {
            var configuration = await _configurationRepository.GetOneAsync(x => x.Id == id);
            return configuration == null ? (IActionResult) NotFound() : View(configuration);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Configuration model)
        {
            var configuration = await _configurationRepository.UpdateOneAsync(model);
            return RedirectToAction("Edit", new { id = model.Id });
        }

        public async Task<IActionResult> Delete(string id)
        {
            await _configurationRepository.DeleteAsync(id);
            return RedirectToAction("List");
        }
    }

}