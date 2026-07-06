using Helix.Data.Entities;
using Helix.Service.DTOs.Pharmacy;

namespace Helix.Service.Interfaces
{
    public interface IPharmacyService
    {
        Task<Pharmacy> RegisterPharmacyAsync(RegisterPharmacyDto dto, Guid appUserId);
        Task<PharmacyDashboardDto> GetDashboardAsync(Guid appUserId);
        Task<PharmacyPrescriptionDetailsDto> GetPrescriptionDetailsForPharmacyAsync(Guid prescriptionId, Guid appUserId);
        Task<PharmacyPrescriptionDetailsDto> GetPrescriptionDetailsForPharmacyAsync(string QrToken, Guid appUserId);
        Task<bool> DispensePrescriptionAsync(Guid prescriptionId, Guid appUserId);
        Task<bool> FlagPrescriptionAsync(Guid prescriptionId, FlagPrescriptionDto dto, Guid appUserId);
    }
}