using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace RetailTrack.Controllers
{
    [Route("api/test")]
    public class TestController : Controller
    {
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Hello()
        {
            _logger.LogInformation("Hello endpoint was reached.");
            return Content("Hello World!");
        }
    }
}
