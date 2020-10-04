using System;
using Microsoft.AspNetCore.Mvc;

namespace SeriesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeriesController : ControllerBase
    {
        private static readonly string[] Series = new[]
        {
            "Dark Tourist", "Love is Blind", "Dracula", "Troy", "Bandish Bandits"
        };

    
        [HttpGet]
        public ActionResult Get()
        {
            var rng = new Random();
            
            return Ok(Series[rng.Next(Series.Length)]);
        }
    }
}