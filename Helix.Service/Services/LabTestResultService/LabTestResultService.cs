using AutoMapper;
using Helix.Data.Entities;
using Helix.Service.DTOs.LabTestResultDTOs;
using Helix.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helix.Service.Services.LabTestResultService
{
    public class LabTestResultService(IUnitOfWork unitOfWork, IMapper mapper) : ILabTestResultService
    {
        public async Task<bool> DeleteLabTestResultAsync(Guid id)
        {
            var labTestResult = await unitOfWork.Repository<LabTestResult>().GetByIdAsync(id);
            if (labTestResult == null) return false;

            await unitOfWork.Repository<LabTestResult>().DeleteAsync(labTestResult);
            await unitOfWork.CompleteAsync(); // Asynchronous commit
            return true;
        }

        public async Task<IEnumerable<LabTestResultDto>> GetAllLabTestResultsAsync()
        {
            // 1. Safely await the Queryable to keep the thread alive
            var query = await unitOfWork.Repository<LabTestResult>().FindAsQueryable(l => true);

            // 2. Attach includes and execute asynchronously with ToListAsync()
            var labTestResults = await query
                .Include(l => l.MedicalConcept)
                .Include(l => l.Patient)
                    .ThenInclude(p => p.AppUser)
                .ToListAsync();

            return mapper.Map<IEnumerable<LabTestResultDto>>(labTestResults);
        }

        public async Task<IEnumerable<LabTestResultDto>> GetAllLabTestResultsAsync(Guid patientId)
        {
            // Filter directly at the database level inside FindAsQueryable
            var query = await unitOfWork.Repository<LabTestResult>().FindAsQueryable(l => l.PatientId == patientId);

            var labTestResults = await query
                .Include(l => l.MedicalConcept)
                .Include(l => l.Patient)
                    .ThenInclude(p => p.AppUser)
                .ToListAsync();

            return mapper.Map<IEnumerable<LabTestResultDto>>(labTestResults);
        }

        public async Task<LabTestResultDto?> GetLabTestResultByIdAsync(Guid id)
        {
            // Memory-optimized lookup
            var labTestResult = await unitOfWork.Repository<LabTestResult>().GetByIdAsync(id);
            return labTestResult == null ? null : mapper.Map<LabTestResultDto>(labTestResult);
        }

        public async Task<LabTestResultDto> UpdateLabTestResultAsync(Guid id, UpdateLabTestResultDto updateLabTestResultDto)
        {
            var labTestResult = await unitOfWork.Repository<LabTestResult>().GetByIdAsync(id);
            if (labTestResult == null)
                throw new Exception($"LabTestResult with ID {id} not found.");

            mapper.Map(updateLabTestResultDto, labTestResult);

            await unitOfWork.Repository<LabTestResult>().UpdateAsync(labTestResult);
            await unitOfWork.CompleteAsync();

            return mapper.Map<LabTestResultDto>(labTestResult);
        }
    }
}