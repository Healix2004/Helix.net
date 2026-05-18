using Helix.Service.DTOs.MedicationDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helix.Service.Interfaces
{
    public interface IMedicationService
    {
        Task<MedicationDto> GetMedicationByIdAsync(Guid id);
        Task<IEnumerable<MedicationDto>> GetAllMedicationsAsync();
        Task<IEnumerable<MedicationDto>> GetPatientMedicationsAsync(Guid patientId);
        Task<MedicationDto> CreateMedicationAsync(CreateMedicationDto createMedicationDto);
        Task<MedicationDto> UpdateMedicationAsync(Guid id, UpdateMedicationDto updateMedicationDto);
        Task<bool> DeleteMedicationAsync(Guid id);
    }
}
