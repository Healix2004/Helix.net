using AutoMapper;
using Helix.Data.Entities;
using Helix.Data.Enums;
using Helix.Infrastructure.Context;
using Helix.Service.DTOs.AuthDTOs;
using Helix.Service.DTOs.DoctorDTOs;
using Helix.Service.DTOs.FileDto;
using Helix.Service.DTOs.PatientDTOs;
using Helix.Service.Interfaces;
using Helix.Service.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helix.Service.Services.AuthServices
{
    public class AuthService(UserManager<AppUser> userManager, ITokenProvider tokenProvider, IFileService fileService, IMapper mapper, IHttpContextAccessor httpContextAccessor, IEmailService emailService, IUnitOfWork unitOfWork) : IAuthService
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
                // Return null token to indicate authentication failure. Don't throw exception to avoid revealing if user exists
                return new AuthDto { AccessToken = null };
            }
            if (user.EmailConfirmed == false)
            {
                // resend email confirmation code 
                await sendConfirmationEmailAsync(user.Id);
                throw new InvalidOperationException("Email not confirmed. Please confirm your email before logging in.");
            }

            var token = await tokenProvider.GenerateAccessTokenAsync(user);
            var roles = await userManager.GetRolesAsync(user);
            return new AuthDto { UserId = user.Id, AccessToken = token, Roles = roles.ToList() };
        }

        public async Task<AuthDto> RegisterAsync(RegisterDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto), "Registration data is required.");
            }

            await ValidateUserUniquenessAsync(dto.Email, dto.Username);

            // Map RegisterDto to AppUser using AutoMapper
            var user = mapper.Map<AppUser>(dto);
            var result = await userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"User creation failed: {errors}");
            }

            // Send email confirmation code
            await sendConfirmationEmailAsync(user.Id);

            string roleName = dto.RegisterAs.ToString();

            if (roleName.Contains("Register") && !await userManager.IsInRoleAsync(user, roleName))
            {
                await userManager.AddToRoleAsync(user, roleName);
            }
            else
            {
                var errors = "invlaid User Role selection";
                throw new InvalidOperationException($"User creation failed: {errors}");
            }
            var token = await tokenProvider.GenerateAccessTokenAsync(user);

            return new AuthDto { UserId = user.Id, AccessToken = token};
        }
        public async Task<string> ConfirmEmailAsync(string email, string code)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new ArgumentException("Invalid user Email");
            }

            // Try to decode Base64Url encoded tokens (safe for URL). If decode fails, use original code.
            var decodedCode = TryDecodeBase64Url(code);

            var result = await userManager.ConfirmEmailAsync(user, decodedCode);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Email confirmation failed: {errors}");
            }

            return "Email confirmed successfully.";
        }

        public async Task<string> ForgetPasswordAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email address is required.");
            }

            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return "If the email exists, a password reset code has been sent.";
            }

            var sixDigitCode = await userManager.GeneratePasswordResetTokenAsync(user);

            var emailBody = $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; text-align: center; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 8px; }}
                    .code {{ font-size: 32px; font-weight: bold; letter-spacing: 5px; color: #007bff; margin: 20px 0; }}
                </style>
            </head>
            <body>
                <div class=""container"">
                    <h2>Password Reset Request</h2>
                    <p>You requested to reset your password for Healix. Please use the following 6-digit code to complete the process:</p>
                    <div class=""code"">{sixDigitCode}</div>
                    <p>If you did not request this, please ignore this email.</p>
                    <p><strong>Note: This code will expire shortly.</strong></p>
                </div>
            </body>
            </html>";

            try
            {
                await emailService.SendEmail(email, emailBody, "Your Password Reset Code - Healix");
            }
            catch (Exception)
            {
                // Log error, but don't expose it to the user
            }

            return "If the email exists, a password reset code has been sent.";
        }

        public async Task<string> ResetPasswordAsync(ResetPasswordDto dto)
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

        public async Task<string> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
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
        
        public async Task<string> ResendConfirmationEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email address is required.");
            }
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new ArgumentException("Invalid email address.");
            }
            if (user.EmailConfirmed)
            {
                return "Email is already confirmed.";
            }
            return await sendConfirmationEmailAsync(user.Id);
        }
        private async Task<string> sendConfirmationEmailAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("Invalid user ID.");
            }

            try
            {
                var confirmationCode = await userManager.GenerateEmailConfirmationTokenAsync(user);

                var emailBody = $@"
                    <html>
                    <head>
                        <style>
                            body {{ font-family: 'Segoe UI', Arial, sans-serif; line-height: 1.6; color: #333; background-color: #f4f4f4; padding: 20px; }}
                            .container {{ max-width: 600px; margin: 0 auto; background: #ffffff; padding: 40px; border-radius: 8px; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }}
                            .header {{ text-align: center; border-bottom: 1px solid #eee; padding-bottom: 20px; }}
                            .code-container {{ text-align: center; margin: 30px 0; padding: 20px; background-color: #f8f9fa; border: 2px dashed #28a745; border-radius: 10px; }}
                            .confirmation-code {{ font-family: 'Courier New', Courier, monospace; font-size: 32px; font-weight: bold; letter-spacing: 10px; color: #28a745; }}
                            .footer {{ margin-top: 30px; font-size: 12px; color: #888; text-align: center; }}
                        </style>
                    </head>
                    <body>
                        <div class=""container"">
                            <div class=""header"">
                                <h2 style=""color: #222;"">Welcome to Helix!</h2>
                            </div>
                            <p>Thank you for registering. To complete your sign-up, please enter the following verification code in the application:</p>
        
                            <div class=""code-container"">
                                <div class=""confirmation-code"">{confirmationCode}</div>
                            </div>

                            <p>This code will expire in 2 hours. If you did not create an account, you can safely ignore this email.</p>
        
                            <div class=""footer"">
                                <p>Helix Security Team<br>This is an automated message, please do not reply.</p>
                            </div>
                        </div>
                    </body>
                    </html>";

                await emailService.SendEmail(user.Email, emailBody, "Confirm Your Email - Helix");
                return "Confirmation email sent successfully.";
            }
            catch (Exception)
            {
                // Log error but continue - user can request resend confirmation later
                return "Failed to send confirmation email. Please try again later.";
            }
        }

        // --- Helper Methods ---

        private async Task ValidateUserUniquenessAsync(string email, string username)
        {
            var existingEmail = await userManager.FindByEmailAsync(email);
            if (existingEmail != null)
            {
                throw new InvalidOperationException("Email address is already registered.");
            }

            var existingUsername = await userManager.FindByNameAsync(username);
            if (existingUsername != null)
            {
                throw new InvalidOperationException("Username is already taken.");
            }
        }

        private static string TryDecodeBase64Url(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            try
            {
                var bytes = WebEncoders.Base64UrlDecode(input);
                return Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                // Not a Base64Url-encoded string — return original value.
                return input;
            }
        }
    }
}