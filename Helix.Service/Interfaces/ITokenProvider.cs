using Helix.Data.Entities;

namespace Helix.Service.Interfaces
{
    public interface ITokenProvider
    {
        Task<string> GenerateAccessTokenAsync(AppUser user);
    }
}
