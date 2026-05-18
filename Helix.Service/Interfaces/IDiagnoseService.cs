using Helix.Service.DTOs.DiagnoseDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helix.Service.Interfaces
{
    public interface IDiagnoseService
    {
        Task<DiagnoseDto> GetDiagnoseByIdAsync(Guid id);
        Task<IEnumerable<DiagnoseDto>> GetAllDiagnosesAsync();
        Task<IEnumerable<DiagnoseDto>> GetPatientDiagnosesAsync(Guid patientId);
        Task<DiagnoseDto> CreateDiagnoseAsync(CreateDiagnoseDto createDiagnoseDto);
        Task<DiagnoseDto> UpdateDiagnoseAsync(Guid id, UpdateDiagnoseDto updateDiagnoseDto);
        Task<bool> DeleteDiagnoseAsync(Guid id);

    }
}
