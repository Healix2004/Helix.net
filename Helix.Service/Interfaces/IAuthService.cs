using Helix.Data.Entities;
using Helix.Service.DTOs.AuthDTOs;
using System.Threading.Tasks;

namespace Helix.Service.Interfaces
{
    public interface IAuthService
    {
        Task<AuthDto> RegisterAsync(RegisterDto dto);
        Task<AuthDto> LoginAsync(LoginDto dto);
        Task<string> ConfirmEmailAsync(Guid userId, string token);
        Task<string> GenerateConfirmLink(AppUser user);
        Task<string> ForgetPasswordAsync(string email);
        Task<string> ResetPasswordAsync(ResetPasswordDto dto);
        Task<string> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
    }
}