using Helix.Data.Entities;
using Helix.Service.DTOs.LabSpecialistDto;

namespace Helix.Service.Interfaces
{
    public interface ILabSpecialistService
    {
        Task<LabSpecialist> RegisterLabSpecialistAsync(RegisterLabSpecialistDto dto, string appUserId);
    }
}