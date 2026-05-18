using AutoMapper;
using Helix.Data.Entities;
using Helix.Service.DTOs.ObservationDTOs;
using Helix.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helix.Service.Services.ObservationService
{
    public class ObservationService(IUnitOfWork unitOfWork, IMapper mapper) : IObservationService
    {
        public async Task<ObservationDto> CreateObservationAsync(CreateObservationDto createObservationDto)
        {
            // 1. Optimize: Fast memory-cache lookup
            var encounter = await unitOfWork.Repository<Encounter>().GetByIdAsync(createObservationDto.EncounterId);
            if (encounter == null)
                throw new Exception($"Encounter with ID {createObservationDto.EncounterId} not found.");

            var observation = mapper.Map<Observation>(createObservationDto);

            // 2. Optimize: Set the Foreign Key directly to avoid unnecessary EF Core object tracking overhead
            observation.EncounterId = encounter.Id;

            await unitOfWork.Repository<Observation>().AddAsync(observation);
            await unitOfWork.CompleteAsync(); // Asynchronous database commit

            return mapper.Map<ObservationDto>(observation);
        }

        public async Task<bool> DeleteObservationAsync(Guid id)
        {
            var observation = await unitOfWork.Repository<Observation>().GetByIdAsync(id);
            if (observation == null) return false;

            await unitOfWork.Repository<Observation>().DeleteAsync(observation);
            await unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<IEnumerable<ObservationDto>> GetAllObservationsAsync()
        {
            var observations = await unitOfWork.Repository<Observation>().GetAllAsync();
            return mapper.Map<IEnumerable<ObservationDto>>(observations);
        }

        public async Task<ObservationDto?> GetObservationByIdAsync(Guid id)
        {
            var observation = await unitOfWork.Repository<Observation>().GetByIdAsync(id);
            return observation == null ? null : mapper.Map<ObservationDto>(observation);
        }

        public async Task<IEnumerable<ObservationDto>> GetPatientObservationsAsync(Guid patientId)
        {
            var observations = await unitOfWork.Repository<Observation>().FindAsync(o => o.PatientId == patientId);
            return mapper.Map<IEnumerable<ObservationDto>>(observations);
        }

        public async Task<ObservationDto> UpdateObservationAsync(Guid id, UpdateObservationDto updateObservationDto)
        {
            var observation = await unitOfWork.Repository<Observation>().GetByIdAsync(id);
            if (observation == null)
                throw new Exception($"Observation with ID {id} not found.");

            // Maps the new values from the DTO directly onto the tracked DB entity
            mapper.Map(updateObservationDto, observation);

            await unitOfWork.Repository<Observation>().UpdateAsync(observation);
            await unitOfWork.CompleteAsync();

            return mapper.Map<ObservationDto>(observation);
        }
    }
}