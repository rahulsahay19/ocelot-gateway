using System;
using Microsoft.AspNetCore.Mvc;

namespace OcelotGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private static readonly string[] Test = new[]
        {
            "Test 1", "Test 2", "Test 3", "Test 4", "Test 5"
        };


        [HttpGet]
        public ActionResult Get()
        {
            var rng = new Random();

            return Ok(Test[rng.Next(Test.Length)]);
        }
    }
}
