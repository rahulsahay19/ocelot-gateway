using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace JWT.Server.Model
{
    public class AuthManager : IAuthManager
    {
        private readonly string _key;

        public AuthManager(string key)
        {
            _key = key;
        }
        // This in-memory is just for test. This should come from db.
        private readonly IDictionary<string, string> users = new Dictionary<string, string>()
            {{"admin", "Password@1"}, {"user", "Password@2"}};
        public string Authenticate(string username, string password)
        {
            if (!users.Any(u => u.Key == username && u.Value == password))
            {
                return null;
            }
            // Create JWT Token, if it matches
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(_key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            // it will return token
            var token = tokenHandler.CreateToken(tokenDescriptor);
            // JWT will be returned from here
            return tokenHandler.WriteToken(token);
        }
    }
}
