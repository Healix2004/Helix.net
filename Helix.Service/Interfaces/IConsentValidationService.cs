using System.Security.Claims;

namespace Helix.Service.Interfaces
{
    public interface IConsentValidationService
    {
        ClaimsPrincipal GetPrincipalFromConsentToken(string token);
    }
}
