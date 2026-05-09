using Helix.Service.DTOs.ConsentDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helix.Service.Interfaces
{
    public interface IConsentService
    {
        Task<ConsentDto> GetConsentByIdAsync(Guid id);
        Task<IEnumerable<ConsentDto>> GetAllConsentsAsync();
        Task<ConsentDto> CreateConsentAsync(CreateConsentDto createConsentDto);
        Task<ConsentDto> UpdateConsentAsync(Guid id, UpdateConsentDto updateConsentDto);
        Task<bool> DeleteConsentAsync(Guid id);
    }
}
