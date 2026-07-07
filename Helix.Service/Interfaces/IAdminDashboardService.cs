using Helix.Service.DTOs.AllergyDTOs;

namespace Helix.Service.Interfaces
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardDto> GetSystemOverviewAsync();
        Task<PatientsManagementDashboardDto> GetPatientsManagementAsync(string searchTerm = null);
        Task<DoctorsManagementDashboardDto> GetDoctorsManagementAsync(string searchTerm = null);
        Task<FacilitiesManagementDashboardDto> GetFacilitiesManagementAsync(string searchTerm = null);
        Task<DrugsManagementDashboardDto> GetDrugsManagementAsync(string searchTerm = null);
    }
}