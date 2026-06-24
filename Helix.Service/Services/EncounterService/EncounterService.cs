using AutoMapper;
using Helix.Data.Entities;
using Helix.Service.DTOs.EncounterDTOs;
using Helix.Service.Interfaces;

namespace Helix.Service.Services.EncounterService
{
    public class EncounterService(IUnitOfWork unitOfWork, IMapper mapper) : IEncounterService
    {
        public async Task<EncounterDto> CreateEncounterAsync(CreateEncounterDto createEncounterDto)
        {
            // 1. Optimize: Use GetByIdAsync for faster validation leveraging EF Core's local cache
            var patient = await unitOfWork.Repository<Patient>().GetByIdAsync(createEncounterDto.PatientId);
            if (patient == null)
                throw new Exception($"Patient with ID {createEncounterDto.PatientId} not found.");

            var doctor = await unitOfWork.Repository<Doctor>().GetByIdAsync(createEncounterDto.DoctorId);
            if (doctor == null)
                throw new Exception($"Doctor with ID {createEncounterDto.DoctorId} not found.");

            var encounter = mapper.Map<Encounter>(createEncounterDto);

            // 2. Optimize: You only need to set the Foreign Keys! 
            // Attaching the full objects is unnecessary overhead.
            encounter.PatientId = patient.Id;

            await unitOfWork.Repository<Encounter>().AddAsync(encounter);
            await unitOfWork.CompleteAsync(); // Using the async commit method!

            return mapper.Map<EncounterDto>(encounter);
        }

        public async Task<bool> DeleteEncounterAsync(Guid id)
        {
            var encounter = await unitOfWork.Repository<Encounter>().GetByIdAsync(id);

            if (encounter == null)
                return false;

            await unitOfWork.Repository<Encounter>().DeleteAsync(encounter);
            await unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<IEnumerable<EncounterDto>> GetAllEncountersAsync()
        {
            // 3. True asynchronous await instead of .Result and Task.FromResult
            var encounters = await unitOfWork.Repository<Encounter>().GetAllAsync();
            return mapper.Map<IEnumerable<EncounterDto>>(encounters);
        }

        public async Task<EncounterDto?> GetEncounterByIdAsync(Guid id)
        {
            var encounter = await unitOfWork.Repository<Encounter>().GetByIdAsync(id);

            return encounter == null ? null : mapper.Map<EncounterDto>(encounter);
        }

        public async Task<EncounterDto> UpdateEncounterAsync(Guid id, UpdateEncounterDto updateEncounterDto)
        {
            var encounter = await unitOfWork.Repository<Encounter>().GetByIdAsync(id);

            if (encounter == null)
                throw new Exception($"Encounter with ID {id} not found.");

            // Maps the new values from the DTO directly onto the tracked DB entity
            mapper.Map(updateEncounterDto, encounter);

            await unitOfWork.Repository<Encounter>().UpdateAsync(encounter);
            await unitOfWork.CompleteAsync();

            return mapper.Map<EncounterDto>(encounter);
        }
    }
}