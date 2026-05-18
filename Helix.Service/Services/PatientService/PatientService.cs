using AutoMapper;
using Helix.Data.Entities;
using Helix.Service.DTOs.PatientDTOs;
using Helix.Service.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Helix.Service.Services.PatientService
{
    public class PatientService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<AppUser> userManager) : IPatientService
    {
        public async Task<PatientDto> CreatePatientAsync(CreatePatientDto createPatientDto)
        {
            // 1. Verify the Identity user exists without attaching the massive object to EF Core
            var user = await userManager.FindByIdAsync(createPatientDto.AppUserId);
            if (user == null)
            {
                throw new Exception($"Identity User with ID {createPatientDto.AppUserId} not found.");
            }

            var patient = mapper.Map<Patient>(createPatientDto);

            // 2. EF Core Optimization: Just set the Foreign Key string
            patient.AppUserId = createPatientDto.AppUserId;

            await unitOfWork.Repository<Patient>().AddAsync(patient);
            await unitOfWork.CompleteAsync(); // Asynchronous database commit

            return mapper.Map<PatientDto>(patient);
        }

        public async Task<bool> DeletePatientAsync(Guid id)
        {
            // 3. Memory-optimized lookup using GetByIdAsync
            var patient = await unitOfWork.Repository<Patient>().GetByIdAsync(id);
            if (patient == null)
            {
                return false;
            }

            await unitOfWork.Repository<Patient>().DeleteAsync(patient);
            await unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<IEnumerable<PatientDto>> GetAllPatientsAsync()
        {
            // 4. True asynchronous execution
            var patients = await unitOfWork.Repository<Patient>().GetAllAsync();
            return mapper.Map<IEnumerable<PatientDto>>(patients);
        }

        public async Task<PatientDto?> GetPatientByIdAsync(Guid id)
        {
            var patient = await unitOfWork.Repository<Patient>().GetByIdAsync(id);
            return patient == null ? null : mapper.Map<PatientDto>(patient);
        }

        public async Task<PatientDto?> GetPatientByUserIdAsync(string userId)
        {
            // FindAsync returns an IReadOnlyList, so we await it, then take the FirstOrDefault
            var patients = await unitOfWork.Repository<Patient>().FindAsync(p => p.AppUserId == userId);
            var patient = patients.FirstOrDefault();

            return patient == null ? null : mapper.Map<PatientDto>(patient);
        }

        public async Task<PatientDto> UpdatePatientAsync(Guid id, UpdatePatientDto updatePatientDto)
        {
            var patient = await unitOfWork.Repository<Patient>().GetByIdAsync(id);
            if (patient == null)
            {
                throw new Exception($"Patient with ID {id} not found.");
            }

            mapper.Map(updatePatientDto, patient);

            await unitOfWork.Repository<Patient>().UpdateAsync(patient);
            await unitOfWork.CompleteAsync();

            return mapper.Map<PatientDto>(patient);
        }
    }
}