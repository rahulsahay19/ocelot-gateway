using Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MoviesAPI.Controllers
{
    [Route("")]
    public class HomeController : ControllerBase
    {
        private readonly IOptions<AppOptions> _appOptions;

        public HomeController(IOptions<AppOptions> appOptions)
        {
            _appOptions = appOptions;
        }
        [HttpGet]
        public IActionResult Get() => Ok(_appOptions.Value.Name);

        [HttpGet("ping")]
        public IActionResult Ping() => Ok("");
    }
}
