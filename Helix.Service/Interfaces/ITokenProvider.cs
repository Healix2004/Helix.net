using Helix.Data.Entities;
using Helix.Service.DTOs.ConsentDTOs;

namespace Helix.Service.Interfaces
{
    public interface ITokenProvider
    {
        Task<string> GenerateAccessTokenAsync(AppUser user);
        Task<string> GenerateConsentToken(AppUser user, Guid patientId, ConsentRequestDto dto);
    }
}
