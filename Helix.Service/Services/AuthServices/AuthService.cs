using AutoMapper;
using Helix.Data.Entities;
using Helix.Data.Enums;
using Helix.Infrastructure.Context;
using Helix.Service.DTOs.AuthDTOs;
using Helix.Service.DTOs.FileDto;
using Helix.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Helix.Service.Services.AuthServices
{
    public class AuthService(UserManager<AppUser> userManager, ITokenProvider tokenProvider, IFileService fileService, IMapper mapper, IHttpContextAccessor httpContextAccessor, IEmailService emailService,ApplicationDbContext dbContext) : IAuthService
    {
        public async Task<AuthDto> LoginAsync(LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.EmailAddress) || string.IsNullOrWhiteSpace(dto.Password))
            {
                throw new ArgumentException("Email and password are required.");
            }

            var user = await userManager.FindByEmailAsync(dto.EmailAddress);
            if (user == null || !await userManager.CheckPasswordAsync(user, dto.Password))
            {
                // Return null token to indicate authentication failure
                // Don't throw exception to avoid revealing if user exists
                return new AuthDto { AccessToken = null };
            }
            //if(user.EmailConfirmed == false)
            //{
            //    throw new InvalidOperationException("Email not confirmed. Please confirm your email before logging in.");
            //}
            var token = await tokenProvider.GenerateAccessTokenAsync(user);
            return new AuthDto { AccessToken = token };
        }
        public async Task<AuthDto> RegisterStep1Async(RegisterStep1Dto dto)
        {
            var user = await CreateUser(dto);

            await userManager.AddToRoleAsync(user, EnRoles.Patient.ToString());
            var patient = new Patient
            {
                AppUser = user,
                PatientCategory = EnPatientCategories.Outpatient
            };
            dbContext.Patients.Add(patient);
            dbContext.SaveChanges();

            var token = await tokenProvider.GenerateAccessTokenAsync(user);
            return new AuthDto { AccessToken = token };
        }
        public async Task<AuthDto> RegisterDoctorAsync(RegisterDoctorDto dto)
        {
            var user = await CreateUser(dto);

            await userManager.AddToRoleAsync(user, EnRoles.Doctor.ToString());
            var doctor = new Doctor
            {
                AppUser = user,
                Specialization = dto.Specialization
            };
            dbContext.Doctors.Add(doctor);
            dbContext.SaveChanges();

            var token = await tokenProvider.GenerateAccessTokenAsync(user);
            return new AuthDto { AccessToken = token };
        }

        public async Task<AuthDto> RegisterAsync(RegisterDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto), "Registration data is required.");
            }

            // Validate email uniqueness
            var existingUser = await userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Email address is already registered.");
            }

            // Validate username uniqueness
            existingUser = await userManager.FindByNameAsync(dto.Username);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Username is already taken.");
            }


            // Map RegisterDto to AppUser using AutoMapper
            var user = mapper.Map<AppUser>(dto);

            var result = await userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                // Log errors for debugging
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"User creation failed: {errors}");
            }

            // Generate email confirmation token and send confirmation email
            try
            {
                var confirmationLink = await ((IAuthService)this).GenerateConfirmLink(user);
                var emailBody = $@"
                    <html>
                    <head>
                        <style>
                            body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                            .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                            .button {{ display: inline-block; padding: 10px 20px; background-color: #28a745; color: white; text-decoration: none; border-radius: 5px; margin: 10px 0; }}
                            .button:hover {{ background-color: #218838; }}
                            .footer {{ margin-top: 20px; font-size: 12px; color: #666; }}
                        </style>
                    </head>
                    <body>
                        <div class=""container"">
                            <h2>Welcome to Helix!</h2>
                            <p>Thank you for registering. Please confirm your email address by clicking the button below:</p>
                            <p><a href=""{confirmationLink}"" class=""button"">Confirm Email</a></p>
                            <p>Or copy and paste this link into your browser:</p>
                            <p style=""word-break: break-all;"">{confirmationLink}</p>
                            <p>If you did not create an account, please ignore this email.</p>
                            <div class=""footer"">
                                <p>This is an automated message, please do not reply to this email.</p>
                            </div>
                        </div>
                    </body>
                    </html>";
                
                await emailService.SendEmail(user.Email, emailBody, "Confirm Your Email - Helix");
            }
            catch (Exception ex)
            {
                // Log error but continue - user can request resend confirmation later
                // Don't fail registration if email sending fails
            }

            var token = await tokenProvider.GenerateAccessTokenAsync(user);
            return new AuthDto { AccessToken = token };
        }

        async Task<string> IAuthService.ConfirmEmailAsync(Guid userId, string token)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new ArgumentException("Invalid user ID.");
            }

            var result = await userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Email confirmation failed: {errors}");
            }

            return "Email confirmed successfully.";
        }

        async Task<string> IAuthService.GenerateConfirmLink(AppUser user)
        {
            var scheme = httpContextAccessor.HttpContext.Request.Scheme;
            var host = httpContextAccessor.HttpContext.Request.Host.Value;
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"{scheme}://{host}/api/auth/confirmemail?userid={user.Id}&token={Uri.EscapeDataString(token)}";

            return confirmationLink;
        }

        async Task<string> IAuthService.ForgetPasswordAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email address is required.");
            }

            var user = await userManager.FindByEmailAsync(email);
            // Don't reveal if user exists or not for security reasons
            if (user == null)
            {
                // Still return success message to prevent email enumeration
                return "If the email exists, a password reset link has been sent.";
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var scheme = httpContextAccessor.HttpContext.Request.Scheme;
            var host = httpContextAccessor.HttpContext.Request.Host.Value;
            // Note: The reset password endpoint expects the data in the request body (POST), not query parameters
            // For email links, we provide a link to a frontend page that will call the API with the token
            var resetLink = $"{scheme}://{host}/api/auth/reset-password";

            var emailBody = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .button {{ display: inline-block; padding: 10px 20px; background-color: #007bff; color: white; text-decoration: none; border-radius: 5px; margin: 10px 0; }}
                        .button:hover {{ background-color: #0056b3; }}
                        .footer {{ margin-top: 20px; font-size: 12px; color: #666; }}
                    </style>
                </head>
                <body>
                    <div class=""container"">
                        <h2>Password Reset Request</h2>
                        <p>You have requested to reset your password. Click the button below to reset your password:</p>
                        <p><a href=""{resetLink}"" class=""button"">Reset Password</a></p>
                        <p>Or copy and paste this link into your browser:</p>
                        <p style=""word-break: break-all;"">{resetLink}</p>
                        <p>If you did not request this, please ignore this email and your password will remain unchanged.</p>
                        <p><strong>This link will expire in 1 hour.</strong></p>
                        <div class=""footer"">
                            <p>This is an automated message, please do not reply to this email.</p>
                        </div>
                    </div>
                </body>
                </html>";

            try
            {
                await emailService.SendEmail(email, emailBody, "Password Reset Request - Helix");
            }
            catch (Exception ex)
            {
                // Log error but don't reveal email sending failure to user (security best practice)
                // Still return success message to prevent email enumeration
            }

            return "If the email exists, a password reset link has been sent.";
        }

        async Task<string> IAuthService.ResetPasswordAsync(ResetPasswordDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto), "Reset password data is required.");
            }

            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                throw new ArgumentException("Invalid email address.");
            }

            var result = await userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Password reset failed: {errors}");
            }

            return "Password has been reset successfully.";
        }

        async Task<string> IAuthService.ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("User ID is required.");
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("Invalid user ID.");
            }

            // Verify current password
            var isCurrentPasswordValid = await userManager.CheckPasswordAsync(user, currentPassword);
            if (!isCurrentPasswordValid)
            {
                throw new InvalidOperationException("Current password is incorrect.");
            }

            var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Password change failed: {errors}");
            }

            return "Password has been changed successfully.";
        }
        private async Task<AppUser> CreateUser(RegisterUserDto dto)
        {
            // Validate the input DTO
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }
            // Validate email uniqueness
            var existingUser = await userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Email address is already registered.");
            }

            // Validate username uniqueness
            existingUser = await userManager.FindByNameAsync(dto.Username);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Username is already taken.");
            }

            // Map RegisterStep1Dto to AppUser using AutoMapper
            var user = mapper.Map<AppUser>(dto);

            if (user.Address == null)
            {
                user.Address = "Mansoura, Egypt";
                user.FirstName = "Defualt1";
                user.LastName = "Defualt2";
                user.MiddleName = "Defualt3";
                user.Gender = EnGenders.None.ToString();
            }

            var result = await userManager.CreateAsync(user, dto.Password);

            // For demonstration, let's assume registration is successful and return a new AuthDto
            var authDto = new AuthDto
            {
                AccessToken = "sample_access_token", // Replace with actual token generation logic
                                                     // Populate other properties as needed
            };

            if (!result.Succeeded)
            {
                // Log errors for debugging
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"User creation failed: {errors}");
            }
            return user;
        }
    }
}
