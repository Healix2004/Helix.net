using Helix.Service.DTOs.PatientDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helix.Service.Interfaces
{
    public interface IPatientService
    {
        Task<PatientDto> GetPatientByIdAsync(Guid id);
        Task<PatientDto> GetPatientByUserIdAsync(string userId);
        Task<IEnumerable<PatientDto>> GetAllPatientsAsync();
        Task<PatientDto> CreatePatientAsync(CreatePatientDto createPatientDto);
        Task<PatientDto> UpdatePatientAsync(Guid id, UpdatePatientDto updatePatientDto);
        Task<bool> DeletePatientAsync(Guid id);
    }
}
