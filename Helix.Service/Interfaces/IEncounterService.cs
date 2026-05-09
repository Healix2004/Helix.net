using Helix.Service.DTOs.EncounterDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helix.Service.Interfaces
{
    public interface IEncounterService
    {
        Task<EncounterDto> GetEncounterByIdAsync(Guid id);
        Task<IEnumerable<EncounterDto>> GetAllEncountersAsync();
        Task<EncounterDto> CreateEncounterAsync(CreateEncounterDto createEncounterDto);
        Task<EncounterDto> UpdateEncounterAsync(Guid id, UpdateEncounterDto updateEncounterDto);
        Task<bool> DeleteEncounterAsync(Guid id);
    }
}
