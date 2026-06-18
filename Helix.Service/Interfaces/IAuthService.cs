using Helix.Data.Entities;
using Helix.Service.DTOs.AuthDTOs;
using Helix.Service.DTOs.DoctorDTOs;
using Helix.Service.DTOs.PatientDTOs;
using System.Threading.Tasks;

namespace Helix.Service.Interfaces
{
    public interface IAuthService
    {
        Task<AuthDto> RegisterAsync(RegisterDto dto);
        Task<AuthDto> RegisterDoctorAsync(DoctorRegistrationPayloadDto payload);
        Task<AuthDto> RegisterPatientAsync(PatientRegistrationPayloadDto payload);

        Task<AuthDto> LoginAsync(LoginDto dto);
        Task<string> ConfirmEmailAsync(string Email, string code);
        Task<string> ForgetPasswordAsync(string email);
        Task<string> ResetPasswordAsync(ResetPasswordDto dto);
        Task<string> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<string> ResendConfirmationEmailAsync(AppUser user);
    }
}