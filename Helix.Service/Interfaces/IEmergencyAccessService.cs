namespace Helix.Service.Interfaces
{
    public interface IEmergencyAccessService
    {
        Task<bool> ActivateEmergencyOverrideAsync(Guid doctorId, Guid patientId, string reason);
        Task<bool> HasActiveEmergencyAccessAsync(Guid doctorId, Guid patientId);
    }
}