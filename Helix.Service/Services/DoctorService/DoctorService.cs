using AutoMapper;
using Helix.Data.Entities;
using Helix.Data.Enums;
using Helix.Service.DTOs.DoctorDTOs;
using Helix.Service.DTOs.PatientDTOs;
using Helix.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Helix.Service.Services.DoctorService
{
    public class DoctorService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<AppUser> userManager, IFileService fileService , RoleManager<IdentityRole>  roleManager) : IDoctorService
    {
        public async Task<DoctorDto> CreateDoctorAsync(CreateDoctorDto createDoctorDto)
        {
            // 1. Verify the Identity user exists without attaching the massive object to EF Core
            var user = await userManager.FindByIdAsync(createDoctorDto.AppUserId);
            if (user == null)
            {
                throw new Exception($"Identity User with ID {createDoctorDto.AppUserId} not found.");
            }
            if (await unitOfWork.Repository<Doctor>().FindAsync(p => p.AppUserId == createDoctorDto.AppUserId) is { Count: > 0 })
            {
                throw new Exception($"A patient already exists for the Identity User with ID {createDoctorDto.AppUserId}.");
            }

            var doctor = mapper.Map<Doctor>(createDoctorDto);

            // 2. EF Core Optimization: Just set the Foreign Key string
            doctor.AppUserId = createDoctorDto.AppUserId;

            doctor.MedicalLicenseDocumentUrl = await fileService.UploadFileAsync(createDoctorDto.MedicalLicenseDocument);
            doctor.NationalIdDocumentUrl = await fileService.UploadFileAsync(createDoctorDto.NationalIdDocument);
            doctor.ProfileImageUrl = await fileService.UploadFileAsync(createDoctorDto.ProfileImage);

            await unitOfWork.Repository<Doctor>().AddAsync(doctor);
            await unitOfWork.CompleteAsync(); // Using our new asynchronous commit!

            
            string roleName = EnRoles.Doctor.ToString();
            if (!await userManager.IsInRoleAsync(user, roleName))
                await userManager.AddToRoleAsync(user, roleName);

            roleName= EnRoles.RegisterAsDoctor.ToString();
            if (await userManager.IsInRoleAsync(user, roleName))
                await userManager.RemoveFromRoleAsync(user,roleName);

            user.PhoneNumber = createDoctorDto.PhoneNumber;
            await userManager.UpdateAsync(user);
            return mapper.Map<DoctorDto>(doctor);
        }

        public async Task<bool> DeleteDoctorAsync(Guid id)
        {
            var doctor = await unitOfWork.Repository<Doctor>().GetByIdAsync(id);
            if (doctor == null)
            {
                return false;
            }

            await unitOfWork.Repository<Doctor>().DeleteAsync(doctor);
            await unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync()
        {
            // 3. True asynchronous execution
            var doctors = await unitOfWork.Repository<Doctor>().GetAllAsync();
            return mapper.Map<IEnumerable<DoctorDto>>(doctors);
        }

        public async Task<DoctorDto?> GetDoctorByIdAsync(Guid id)
        {
            // 4. Memory-optimized lookup using GetByIdAsync instead of Find().FirstOrDefault()
            var quary = await unitOfWork.Repository<Doctor>().FindAsQueryable(d => d.Id == id);
            var doctor = await quary.Include(d => d.SpecialtyCatalog).Include(d=> d.AppUser).FirstOrDefaultAsync();

            return doctor == null ? null : mapper.Map<DoctorDto>(doctor);
        }

        public async Task<DoctorDto?> GetDoctorByUserIdAsync(string userId)
        {
            // FindAsync returns an IReadOnlyList, so we await it, then take the FirstOrDefault
            var doctors = await unitOfWork.Repository<Doctor>().FindAsync(d => d.AppUserId == userId);
            var doctor = doctors.FirstOrDefault();

            return doctor == null ? null : mapper.Map<DoctorDto>(doctor);
        }

        public async Task<DoctorDto> UpdateDoctorAsync(Guid id, UpdateDoctorDto updateDoctorDto)
        {
            var doctor = await unitOfWork.Repository<Doctor>().GetByIdAsync(id);
            if (doctor == null)
            {
                throw new Exception($"Doctor with ID {id} not found.");
            }

            mapper.Map(updateDoctorDto, doctor);

            await unitOfWork.Repository<Doctor>().UpdateAsync(doctor);
            await unitOfWork.CompleteAsync();

            return mapper.Map<DoctorDto>(doctor);
        }

        public async Task<IEnumerable<DoctorDto>> SearchDoctorsByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new List<DoctorDto>();
            }

            var query = await unitOfWork.Repository<Doctor>()
                .FindAsQueryable(d => d.FullName.ToLower().Contains(name.ToLower()));

            var doctors = await query
                .Include(d => d.SpecialtyCatalog)
                .ToListAsync();

            // Map to your DoctorDto (assuming you have AutoMapper or manual mapping)
            return mapper.Map<IEnumerable<DoctorDto>>(doctors);
        }

        public async Task<IEnumerable<DoctorDto>> SearchDoctorsBySpecialtyAsync(string specialty)
        {
            if (string.IsNullOrWhiteSpace(specialty))
            {
                return new List<DoctorDto>();
            }

            // This searches both the code (e.g., "CARDIO") and the display name (e.g., "Cardiology")
            var query = await unitOfWork.Repository<Doctor>()
                .FindAsQueryable(d =>
                    d.SpecialtyCatalogCode.ToLower().Contains(specialty.ToLower()) ||
                    (d.SpecialtyCatalog != null && d.SpecialtyCatalog.DisplayName.ToLower().Contains(specialty.ToLower())));

            var doctors = await query
                .Include(d => d.SpecialtyCatalog)
                .ToListAsync();

            return mapper.Map<IEnumerable<DoctorDto>>(doctors);
        }
    }
}