using AutoMapper;
using Helix.Data.Entities;
using Helix.Service.DTOs.DoctorDTOs;
using Helix.Service.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Helix.Service.Services.DoctorService
{
    public class DoctorService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<AppUser> userManager) : IDoctorService
    {
        public async Task<DoctorDto> CreateDoctorAsync(CreateDoctorDto createDoctorDto)
        {
            // 1. Verify the Identity user exists without attaching the massive object to EF Core
            var user = await userManager.FindByIdAsync(createDoctorDto.AppUserId);
            if (user == null)
            {
                throw new Exception($"Identity User with ID {createDoctorDto.AppUserId} not found.");
            }

            var doctor = mapper.Map<Doctor>(createDoctorDto);

            // 2. EF Core Optimization: Just set the Foreign Key string
            doctor.AppUserId = createDoctorDto.AppUserId;

            await unitOfWork.Repository<Doctor>().AddAsync(doctor);
            await unitOfWork.CompleteAsync(); // Using our new asynchronous commit!

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
            var doctor = await unitOfWork.Repository<Doctor>().GetByIdAsync(id);

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
    }
}