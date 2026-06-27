using Helix.Service.DTOs.RadiologyOrderDto;
using Helix.Service.DTOs.RadiologyTestResultDto;

namespace Helix.Service.Interfaces
{
    public interface IRadiologyOrderService
    {
        // ==========================================
        // PATIENT & RADIOLOGY SPECIALIST WORKFLOW (QR)
        // ==========================================
        Task<List<PendingRadiologyOrderDto>> GetPendingOrdersAsync(Guid patientId);

        // Added to sync with the service implementation
        Task<List<RadiologyOrderDto>> GetOrdersByPatientAsync(Guid patientId);

        Task<RadiologyOrderDto> ScanRadiologyOrderAsync(string qrToken);
        Task<bool> UploadResultAsync(CreateRadiologyTestResultDto dto);

        // ==========================================
        // DOCTOR WORKFLOW
        // ==========================================
        Task<Guid> CreateRadiologyOrderAsync(CreateRadiologyOrderDto dto);
        Task<List<RadiologyOrderDto>> GetOrdersByDoctorAsync(Guid doctorId);
        Task<bool> UpdateRadiologyOrderAsync(Guid id, UpdateRadiologyOrderDto dto);

        // ==========================================
        // ADMIN & MANAGEMENT WORKFLOW (CRUD)
        // ==========================================
        Task<RadiologyOrderDto> GetRadiologyOrderByIdAsync(Guid id);
        Task<List<RadiologyOrderDto>> GetAllRadiologyOrdersAsync();
        Task<bool> UpdateOrderStatusAsync(Guid id, string newStatus);
        Task<bool> DeleteRadiologyOrderAsync(Guid id);
    }
}