using Helix.Service.DTOs.LabTestResultDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helix.Service.Interfaces
{
    public interface ILabTestResultService
    {
        Task<LabTestResultDto> GetLabTestResultByIdAsync(Guid id);
        Task<IEnumerable<LabTestResultDto>> GetAllLabTestResultsAsync();
        Task<IEnumerable<LabTestResultDto>> GetAllLabTestResultsAsync(Guid patientId);
        Task<LabTestResultDto> UpdateLabTestResultAsync(Guid id, UpdateLabTestResultDto updateLabTestResultDto);
        Task<bool> DeleteLabTestResultAsync(Guid id);
    }
}
