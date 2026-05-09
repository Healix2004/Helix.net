using Helix.Service.DTOs.AllergyDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helix.Service.Interfaces
{
    public interface IAllergyService
    {
        Task<AllergyDto> GetAllergyByIdAsync(Guid id);
        Task<IEnumerable<AllergyDto>> GetAllAllergiesAsync();
        Task<AllergyDto> CreateAllergyAsync(CreateAllergyDto createAllergyDto);
        Task<AllergyDto> UpdateAllergyAsync(Guid id, UpdateAllergyDto updateAllergyDto);
        Task<bool> DeleteAllergyAsync(Guid id);
    }
}
