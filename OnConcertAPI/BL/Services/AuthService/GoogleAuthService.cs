using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OnConcert.BL.Models;
using OnConcert.Core.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text.Json;

namespace OnConcert.BL.Services.AuthService
{
    public class GoogleAuthService : IExternalAuthService
    {
        private readonly IConfiguration _configuration;

        public GoogleAuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ServiceResponse<object>> Authenticate(string token)
        {
            var response = new ServiceResponse<object>();

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = await GetValidationParametersAsync();
                tokenHandler.ValidateToken(token, validationParameters, out _);
                return response;
            }
            catch
            {
                return ServiceResponseBuilder.CreateErrorResponse<object>("Access token not valid.");
            }
        }

        private async Task<TokenValidationParameters> GetValidationParametersAsync()
        {
            var fetchedJson = await WebContent.FetchJson(_configuration.GetSection("AppSettings:Google:Cert").Value!);

            using JsonDocument document = JsonDocument.Parse(fetchedJson);

            var rsaKeys = document.RootElement.GetProperty("keys").EnumerateArray().Select(key =>
            {
                var rsaParameters = new RSAParameters
                {
                    Exponent = Base64UrlEncoder.DecodeBytes(key.GetProperty("e").GetString()),
                    Modulus = Base64UrlEncoder.DecodeBytes(key.GetProperty("n").GetString())
                };

                return new RsaSecurityKey(rsaParameters);
            }).ToArray();

            return new TokenValidationParameters
            {
                ValidIssuer = _configuration.GetSection("AppSettings:Google:Issuer").Value!,
                IssuerSigningKeys = rsaKeys,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudiences = new[] { _configuration.GetSection("AppSettings:Google:ClientId").Value! },
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        }
    }
}