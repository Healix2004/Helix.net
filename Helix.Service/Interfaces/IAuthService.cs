using Helix.Data.Entities;
using Helix.Service.DTOs.AuthDTOs;
using System.Threading.Tasks;

namespace Helix.Service.Interfaces
{
    public interface IAuthService
    {
        Task<AuthDto> RegisterAsync(RegisterDto dto);
        Task<AuthDto> RegisterStep1Async(RegisterStep1Dto dto);
        Task<AuthDto> RegisterDoctorAsync(RegisterDoctorDto dto);
        Task<AuthDto> LoginAsync(LoginDto dto);
        Task<string> ConfirmEmailAsync(string Email, string code);
        Task<string> ForgetPasswordAsync(string email);
        Task<string> ResetPasswordAsync(ResetPasswordDto dto);
        Task<string> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<string> ResendConfirmationEmailAsync(AppUser user);
    }
}