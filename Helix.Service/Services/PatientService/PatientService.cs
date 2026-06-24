using AutoMapper;
using Helix.Data.Entities;
using Helix.Service.DTOs.DoctorDTOs;
using Helix.Service.DTOs.PatientDTOs;
using Helix.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Helix.Service.Services.PatientService
{
    public class PatientService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<AppUser> userManager, IFileService fileService) : IPatientService
    {
        public async Task<PatientDto> CreatePatientAsync(CreatePatientDto createPatientDto)
        {
            // 1. Verify the Identity user exists without attaching the massive object to EF Core
            var user = await userManager.FindByIdAsync(createPatientDto.AppUserId);
            if (user == null)
            {
                throw new Exception($"Identity User with ID {createPatientDto.AppUserId} not found.");
            }
            if(await unitOfWork.Repository<Patient>().FindAsync(p => p.AppUserId == createPatientDto.AppUserId) is { Count: > 0 })
            {
                throw new Exception($"A patient already exists for the Identity User with ID {createPatientDto.AppUserId}.");
            }
            var patient = mapper.Map<Patient>(createPatientDto);

            // 2. EF Core Optimization: Just set the Foreign Key string
            patient.AppUserId = createPatientDto.AppUserId;

            patient.EmergencyContacts = new List<EmergencyContact>{
                    new EmergencyContact
                    {
                        Name = createPatientDto.EmergencyContactName,
                        PhoneNumber = createPatientDto.EmergencyContactPhoneNumber
                    }};
            patient.Allergies = createPatientDto.AllergiesCode.Select(a => new Allergy() { AllergenCatalogCode = a }).ToList();
            patient.ChronicDiseases = createPatientDto.ChronicDiseasesCode.Select(c => new ChronicDisease() { ChronicDiseaseCatalogCode = c }).ToList();
            patient.Surgeries = createPatientDto.SurgeriesCode.Select(s => new Surgery() { ProcedureCatalogCode = s }).ToList();
            patient.Medications = createPatientDto.CurrentMedicationsCode.Select(m => new Medication() { medicationCatalogRxcui = m }).ToList();
            patient.ProfileImageUrl = await fileService.UploadFileAsync(createPatientDto.ProfileImage);

            await unitOfWork.Repository<Patient>().AddAsync(patient);

            await unitOfWork.CompleteAsync(); // Asynchronous database commit

            patient = await unitOfWork.Repository<Patient>().GetByIdAsync(patient.Id); 

            user.PhoneNumber = createPatientDto.PhoneNumber;
            await userManager.UpdateAsync(user);
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
            var quary = await unitOfWork.Repository<Patient>().FindAsQueryable(p=>true);
            var patients = await quary.Include(p=>p.AppUser).ToListAsync();
            return mapper.Map<IEnumerable<PatientDto>>(patients);
        }

        public async Task<PatientDto?> GetPatientByIdAsync(Guid id)
        {
            var quary = await unitOfWork.Repository<Patient>().FindAsQueryable(p => p.Id == id);
            var patient = quary.Include(p => p.AppUser)
                .Include(p => p.Surgeries).ThenInclude(s => s.ProcedureCatalog)
                .Include(p => p.Medications).ThenInclude(m => m.medicationCatalog)
                .Include(p => p.Allergies).ThenInclude(a => a.AllergenCatalog)
                .Include(p => p.ChronicDiseases).ThenInclude(c => c.ChronicDiseaseCatalog)
                .FirstOrDefault();

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