using Helix.Data.Enums;
using Helix.Service.DTOs.DrugDTOs;
using Helix.Service.DTOs.PrescriptionDtos;

namespace Helix.Service.Interfaces
{
    public interface IPrescriptionService
    {
        Task<CreatePrescriptionResultDto> CreatePrescriptionAsync(Guid doctorId, PrescriptionPayloadDto dto);
        Task<PatientClinicalSummaryDto> GetPatientClinicalSummaryAsync(Guid patientId);
        Task<bool> UpdatePrescriptionItemStatusAsync(Guid itemId, EnPrescriptionItemStatus newStatus);
        Task<PrescriptionSafetyResultDto> CheckCompletePrescriptionSafetyAsync(PrescriptionSafetyCheckDto request);
    }
}