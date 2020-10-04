using System;
using Microsoft.AspNetCore.Mvc;

namespace MoviesAPI.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private static readonly string[] Movies = new[]
        {
            "Die Another Day", "Top Gun", "Grease", "Dil Bechara", "Jurassic Park"
        };

    
        [HttpGet]
        public ActionResult Get()
        {
            var rng = new Random();
            
            return Ok(Movies[rng.Next(Movies.Length)]);
        }
    }
}