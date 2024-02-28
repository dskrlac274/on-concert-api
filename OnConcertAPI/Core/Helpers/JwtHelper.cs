using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace OnConcert.Core.Helpers
{
    public class JwtHelper : IJwtHelper
    {
        private readonly IConfiguration _configuration;

        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateJwt(int userId, string role)
        {
            var jwtSecret = _configuration.GetSection("AppSettings:Jwt:Secret").Value!;
            var jwtDuration = _configuration.GetSection("AppSettings:Jwt:Duration").Value!;
            var jwtIssuer = _configuration.GetSection("AppSettings:Jwt:Issuer").Value!;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                    {
                        new(ClaimTypes.NameIdentifier, userId.ToString()),
                        new(ClaimTypes.Role, role)
                    }
                ),
                Issuer = jwtIssuer,
                IssuedAt = DateTime.Now,
                Expires = DateTime.Now.AddHours(int.Parse(jwtDuration)),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}