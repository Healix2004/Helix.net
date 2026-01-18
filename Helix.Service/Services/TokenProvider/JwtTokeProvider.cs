using Helix.Data.Entities;
using Helix.Service.Interfaces;
using Helix.Service.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Helix.Service.Services.TokenProvider
{
    internal class JwtTokeProvider : ITokenProvider
    {
        private readonly JwtSettings jwtSettings;
        private readonly UserManager<AppUser> userManager;

        public JwtTokeProvider(IOptions<JwtSettings> jwtSettings, UserManager<AppUser> userManager)
        {
            this.jwtSettings = jwtSettings.Value;
            this.userManager = userManager;
        }

        public async Task<string> GenerateAccessTokenAsync(AppUser user)
        {
            var userClimas = await userManager.GetClaimsAsync(user);
            var roles = await userManager.GetRolesAsync(user);
            var rolesClimas = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();
            var climas = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty)
            };
            climas.AddRange(userClimas);
            climas.AddRange(rolesClimas);

            var issuer = jwtSettings.Issuer ?? throw new InvalidOperationException("JWT issuer is missing.");
            var audience = jwtSettings.Audience ?? throw new InvalidOperationException("JWT audience is missing.");
            var key = jwtSettings.Secret ?? throw new InvalidOperationException("JWT key is missing.");

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);


            var jwtToken = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: climas,
                expires: DateTime.UtcNow.AddDays(jwtSettings.ExpirationInDay),
                signingCredentials: signingCredentials
            );
            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
            ;
        }
    }
}
