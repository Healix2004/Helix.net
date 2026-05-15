using Helix.Service.DTOs.LabOrderDTOs;
using Helix.Service.DTOs.LabTestResultDTOs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Helix.Service.Interfaces
{
    public interface ILabOrderService
    {
        // ==========================================
        // PATIENT & LAB SPECIALIST WORKFLOW (QR)
        // ==========================================
        Task<List<PendingLabOrderDto>> GetPendingOrdersAsync(Guid patientId);
        Task<LabOrderDto> ScanLabOrderAsync(string qrToken);
        Task<bool> UploadLabResultAsync(LabTestResultDto dto);

        // ==========================================
        // DOCTOR WORKFLOW
        // ==========================================
        Task<Guid> CreateLabOrderAsync(CreateLabOrderDto dto);
        Task<List<LabOrderDto>> GetOrdersByDoctorAsync(Guid doctorId);
        Task<bool> UpdateLabOrderAsync(Guid id, UpdateLabOrderDto dto);

        // ==========================================
        // ADMIN & MANAGEMENT WORKFLOW (CRUD)
        // ==========================================
        Task<LabOrderDto> GetLabOrderByIdAsync(Guid id);
        Task<List<LabOrderDto>> GetAllLabOrdersAsync();
        Task<bool> UpdateOrderStatusAsync(Guid id, string newStatus);
        Task<bool> DeleteLabOrderAsync(Guid id);
    }
}