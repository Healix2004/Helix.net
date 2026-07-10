using Helix.Data.Entities;
using Helix.Data.Enums;
using Helix.Service.DTOs.LabSpecialistDto;
using Helix.Service.Interfaces;
using Hl7.Fhir.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Service.Services.LabSpecialistService
{
    public class LabSpecialistService(IUnitOfWork unitOfWork, IFileService fileService, UserManager<AppUser> userManager) : ILabSpecialistService
    {
        public async Task<LabSpecialist> RegisterLabSpecialistAsync(RegisterLabSpecialistDto dto, string appUserId)
        {
            // 1. Guard Clauses & Existence Checks
            var user = await userManager.FindByIdAsync(appUserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User account not found.");
            }

            // Check if the user already has a Lab Specialist profile
            // NOTE: Ensure FindAsQueryable returns an IQueryable<LabSpecialist>.
            // Remove the unnecessary await when obtaining a queryable and use EF Core's AnyAsync.
            var baseQuery =  await unitOfWork.Repository<LabSpecialist>().FindAsQueryable(l => l.AppUserId == appUserId);
            bool profileExists = await baseQuery.AnyAsync();

            if (profileExists)
            {
                throw new InvalidOperationException("A Lab Specialist profile is already associated with this account.");
            }

            // 2. Parallel File Uploads (Optimized Performance)
            var licenseTask = fileService.UploadFileAsync(dto.LabSpecialistLicenseFile, "lab/licenses");
            var nationalIdTask = fileService.UploadFileAsync(dto.NationalIdFile, "lab/national-ids");

            Task<string> profileImageTask = dto.ProfileImageFile != null
                ? fileService.UploadFileAsync(dto.ProfileImageFile, "lab/profiles")
                : Task.FromResult<string>(null);

            // Await all file uploads concurrently
            await Task.WhenAll(licenseTask, nationalIdTask, profileImageTask);

            // 3. Map DTO to Entity
            var labSpecialist = new LabSpecialist
            {
                AppUserId = appUserId,
                FullName = dto.FullName,
                NationalId = dto.NationalId,
                LabName = dto.LabName,
                Address = dto.Address,
                LicenseNumber = dto.LicenseNumber,
                IsVerified = false, // Helix admins must review this manually

                // Extract the generated URLs from the completed tasks
                LabSpecialistLicenseUrl = licenseTask.Result,
                NationalIdUrl = nationalIdTask.Result,
                ProfileImageUrl = profileImageTask.Result
            };

            // 4. Save to Database FIRST
            await unitOfWork.Repository<LabSpecialist>().AddAsync(labSpecialist);

            // If saving fails, it throws an exception and Identity roles remain untouched
            await unitOfWork.CompleteAsync();

            // 5. Update Identity Roles AFTER successful DB save
            // Adjust the enum names below based on exactly how you named them in your EnRoles enum
            var labSpecialistRole = EnRoles.LabSpecialist.ToString();
            var registerRole = EnRoles.RegisterAsLabSpecialist.ToString();

            if (!await userManager.IsInRoleAsync(user, labSpecialistRole))
            {
                await userManager.AddToRoleAsync(user, labSpecialistRole);
            }

            // Remove the temporary registration role if they have it
            if (await userManager.IsInRoleAsync(user, registerRole))
            {
                await userManager.RemoveFromRoleAsync(user, registerRole);
            }

            return labSpecialist;
        }
    }
}
