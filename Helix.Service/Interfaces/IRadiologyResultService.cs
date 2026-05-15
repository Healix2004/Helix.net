using Helix.Service.DTOs.RadiologyTestResultDto;

// Note: You will likely need to add a using statement for your LabOrder DTOs
// using Helix.Service.DTOs.LabOrderDTOs;

namespace Helix.Service.Interfaces
{
    public interface IRadiologyResultService
    {
        //===========================================
        // PATIENT WORKFLOW
        //===========================================
        Task<RadiologyTestResultDto> GetRadiologyResultByOrderIdAsync(Guid orderId);
        Task<List<RadiologyTestResultDto>> GetRadiologyResultsByPatientIdAsync(Guid patientId);

        //===========================================
        // ADMIN / RADIOLOGIST WORKFLOW
        //===========================================
        Task<bool> DeleteRadiologyResultAsync(Guid id);

        // TWEAKED: Now uses the specific Update DTO for better security and performance
        Task UpdateRadiologyResultAsync(UpdateRadiologyTestResultDto dto);

        Task<List<RadiologyTestResultDto>> GetAllRadiologyResultsAsync();
    }
}