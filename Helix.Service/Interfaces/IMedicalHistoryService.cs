using Helix.Service.DTOs.MedicalHistoryDtos;

namespace Helix.Service.Interfaces
{
    public interface IMedicalHistoryService
    {
        Task<List<TimelineEventDto>> GetPatientTimelineAsync(Guid patientId);
    }
}