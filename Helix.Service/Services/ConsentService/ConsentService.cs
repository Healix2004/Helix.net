using AutoMapper;
using Helix.Data.Entities;
using Helix.Service.DTOs.ConsentDTOs;
using Helix.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helix.Service.Services.ConsentService
{
    // Using C# 12 Primary Constructor for cleaner Dependency Injection
    public class ConsentService(IUnitOfWork unitOfWork, IMapper mapper) : IConsentService
    {
        public async Task<ConsentDto> CreateConsentAsync(CreateConsentDto createConsentDto)
        {
            // 1. Optimize: Use GetByIdAsync instead of Find().FirstOrDefault()
            var patient = await unitOfWork.Repository<Patient>().GetByIdAsync(createConsentDto.PatientId);
            if (patient == null)
                throw new Exception($"Patient with ID {createConsentDto.PatientId} not found.");

            var doctor = await unitOfWork.Repository<Doctor>().GetByIdAsync(createConsentDto.DoctorId);
            if (doctor == null)
                throw new Exception($"Doctor with ID {createConsentDto.DoctorId} not found.");

            var consent = mapper.Map<Consent>(createConsentDto);

            consent.PatientId = patient.Id;
            consent.DoctorId = doctor.Id;

            await unitOfWork.Repository<Consent>().AddAsync(consent);
            await unitOfWork.CompleteAsync();

            return mapper.Map<ConsentDto>(consent);
        }

        public async Task<bool> DeleteConsentAsync(Guid id)
        {
            var consent = await unitOfWork.Repository<Consent>().GetByIdAsync(id);

            if (consent == null)
            {
                return false;
            }

            await unitOfWork.Repository<Consent>().DeleteAsync(consent);
            await unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<IEnumerable<ConsentDto>> GetAllConsentsAsync()
        {
            // 3. True async await instead of .Result and Task.FromResult
            var consents = await unitOfWork.Repository<Consent>().GetAllAsync();
            return mapper.Map<IEnumerable<ConsentDto>>(consents);
        }

        public async Task<ConsentDto?> GetConsentByIdAsync(Guid id)
        {
            var consent = await unitOfWork.Repository<Consent>().GetByIdAsync(id);

            return consent == null ? null : mapper.Map<ConsentDto>(consent);
        }

        public async Task<ConsentDto> UpdateConsentAsync(Guid id, UpdateConsentDto updateConsentDto)
        {
            var consent = await unitOfWork.Repository<Consent>().GetByIdAsync(id);

            if (consent == null)
            {
                throw new Exception($"Consent with ID {id} not found.");
            }

            // Maps the new values from the DTO directly onto the tracked DB entity
            mapper.Map(updateConsentDto, consent);

            await unitOfWork.Repository<Consent>().UpdateAsync(consent);
            await unitOfWork.CompleteAsync();

            return mapper.Map<ConsentDto>(consent);
        }
    }
}