using Helix.Service.DTOs.PatientDTOs;

namespace Helix.Service.Interfaces
{
    public interface IPatientDashboardService
    {
        Task<PatientPortalDashboardDto> GetDashboardDataAsync(Guid patientId);

        Task<PatientLabDashboardDto> GetPatientLabDashboardAsync(Guid patientId);
        Task<LabTestDetailsDto> GetLabTestDetailsAsync(Guid orderId, Guid requestingPatientId);

        Task<RadiologyDashboardDto> GetPatientDashboardAsync(Guid patientId);
        Task<RadiologyStudyDetailsDto> GetRadiologyStudyDetailsAsync(Guid orderId, Guid requestingPatientId);

        Task<PatientPrescriptionDashboardDto> GetPatientPrescriptionDashboardAsync(Guid patientId);
        Task<PrescriptionDetailsDto> GetPrescriptionDetailsAsync(Guid prescriptionId, Guid requestingPatientId);
    }
}