using Helix.Service.DTOs.FacilitieDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helix.Service.Interfaces
{
    public interface IFacilitieService
    {
        Task<FacilitieDto> GetFacilitieByIdAsync(Guid id);
        Task<IEnumerable<FacilitieDto>> GetAllFacilitiesAsync();
        Task<FacilitieDto> CreateFacilitieAsync(CreateFacilitieDto createFacilitieDto);
        Task<FacilitieDto> UpdateFacilitieAsync(Guid id, UpdateFacilitieDto updateFacilitieDto);
        Task<bool> DeleteFacilitieAsync(Guid id);
    }
}
