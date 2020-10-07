using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc;

namespace MoviesAPI.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private static int _count = 0;
        private static readonly string[] Movies = new[]
        {
            "Die Another Day", "Top Gun", "Grease", "Dil Bechara", "Jurassic Park"
        };

    
        [HttpGet]
        public ActionResult Get()
        {
            _count++;
            Console.WriteLine($"get...{_count}");
            if (_count <= 5)
            {
                Thread.Sleep(5000);
            }
            var rng = new Random();
            
            return Ok(Movies[rng.Next(Movies.Length)]);
        }
    }
}