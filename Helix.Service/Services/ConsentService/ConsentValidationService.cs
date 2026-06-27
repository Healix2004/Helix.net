using Helix.Service.Interfaces;
using Helix.Service.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Helix.Service.Services.ConsentService
{
    public class ConsentValidationService : IConsentValidationService
    {
        private readonly JwtSettings _jwtSettings;

        // Inject your JwtSettings so you aren't hardcoding keys
        public ConsentValidationService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public ClaimsPrincipal GetPrincipalFromConsentToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true, // Rejects expired tokens automatically
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                // Returns the validated claims if the signature and expiration are good
                return tokenHandler.ValidateToken(token, validationParameters, out _);
            }
            catch
            {
                // Token was tampered with, expired, or invalid
                return null;
            }
        }
    }
}