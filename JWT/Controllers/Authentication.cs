
using System.Collections.Generic;
using JWT.Server.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JWT.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class Authentication : ControllerBase
    {
        private readonly IAuthManager _authManager;

        public Authentication(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new[] {"Value1", "Value2"};
        }
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] UserCredentials userCredentials)
        {
            var token = _authManager.Authenticate(userCredentials.UserName, userCredentials.Password);
            if (token is null)
            {
                return Unauthorized();
            }
            return Ok(token);
        }
    }
}
