using AutoMapper;
using Helix.Data.Entities;
using Helix.Service.DTOs.AllergyDTOs;
using Helix.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helix.Service.Services.AllergyService
{
    public class AllergyService(IUnitOfWork unitOfWork, IMapper mapper) : IAllergyService
    {
        public async Task<AllergyDto> CreateAllergyAsync(CreateAllergyDto createAllergyDto)
        {
            // 1. Truly await the database call using the new GetByIdAsync
            var patient = await unitOfWork.Repository<Patient>().GetByIdAsync(createAllergyDto.PatientId);

            if (patient == null)
                throw new Exception($"Patient with ID {createAllergyDto.PatientId} not found.");

            var allergy = mapper.Map<Allergy>(createAllergyDto);

            // Note: EF Core usually just needs the Foreign Key, but setting the entity is fine too
            allergy.PatientId = patient.Id;

            // 2. Await the repository and UnitOfWork operations
            await unitOfWork.Repository<Allergy>().AddAsync(allergy);
            await unitOfWork.CompleteAsync(); // Ensure your UnitOfWork has an async Complete method!

            return mapper.Map<AllergyDto>(allergy);
        }

        public async Task<bool> DeleteAllergyAsync(Guid id)
        {
            var allergy = await unitOfWork.Repository<Allergy>().GetByIdAsync(id);

            if (allergy == null)
                return false;

            await unitOfWork.Repository<Allergy>().DeleteAsync(allergy);
            await unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<IEnumerable<AllergyDto>> GetAllAllergiesAsync()
        {
            var allergies = await unitOfWork.Repository<Allergy>().GetAllAsync();
            return mapper.Map<IEnumerable<AllergyDto>>(allergies);
        }

        public async Task<IEnumerable<AllergyDto>> GetPatientAllergiesAsync(Guid patientId)
        {
            // FIX: Your original code checked "a => a.Id == PatiendId". 
            // That was comparing the Allergy's ID to the Patient's ID!
            // Changed to a.PatientId == patientId
            var allergies = await unitOfWork.Repository<Allergy>().FindAsync(a => a.PatientId == patientId);
            return mapper.Map<IEnumerable<AllergyDto>>(allergies);
        }

        public async Task<AllergyDto?> GetAllergyByIdAsync(Guid id)
        {
            // Used GetByIdAsync instead of Find().FirstOrDefault()
            var allergy = await unitOfWork.Repository<Allergy>().GetByIdAsync(id);

            return allergy == null ? null : mapper.Map<AllergyDto>(allergy);
        }

        public async Task<AllergyDto> UpdateAllergyAsync(Guid id, UpdateAllergyDto updateAllergyDto)
        {
            var allergy = await unitOfWork.Repository<Allergy>().GetByIdAsync(id);

            if (allergy == null)
                throw new Exception($"Allergy with ID {id} not found.");

            mapper.Map(updateAllergyDto, allergy);

            await unitOfWork.Repository<Allergy>().UpdateAsync(allergy);
            await unitOfWork.CompleteAsync();

            return mapper.Map<AllergyDto>(allergy);
        }
    }
}