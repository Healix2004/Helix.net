using Helix.Service.DTOs.PatientDTOs;

namespace Helix.Service.Interfaces
{
    public interface IPatientDashboardService
    {
        Task<PatientPortalDashboardDto> GetDashboardDataAsync(Guid patientId);
        Task<PatientLabDashboardDto> GetPatientLabDashboardAsync(Guid patientId);
    }
}