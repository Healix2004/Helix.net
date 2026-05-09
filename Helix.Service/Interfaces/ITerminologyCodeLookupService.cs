using Helix.Service.DTOs.TerminologyCodeLookupDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helix.Service.Interfaces
{
    public interface ITerminologyCodeLookupService
    {
        Task<TerminologyCodeLookupDto> GetTerminologyCodeLookupByIdAsync(Guid id);
        Task<IEnumerable<TerminologyCodeLookupDto>> GetAllTerminologyCodeLookupsAsync();
        Task<TerminologyCodeLookupDto> CreateTerminologyCodeLookupAsync(CreateTerminologyCodeLookupDto createDto);
        Task<TerminologyCodeLookupDto> UpdateTerminologyCodeLookupAsync(Guid id, UpdateTerminologyCodeLookupDto updateDto);
        Task<bool> DeleteTerminologyCodeLookupAsync(Guid id);
    }
}
