using AutoMapper;
using Helix.Data.Entities;
using Helix.Data.Enums;
using Helix.Service.DTOs.TerminologyCodeLookupDTOs;
using Helix.Service.Interfaces;

namespace Helix.Service.Services.TerminologyCodeLookupService
{
    public class TerminologyCodeLookupService(IUnitOfWork unitOfWork, IMapper mapper) : ITerminologyCodeLookupService
    {
        public async Task<TerminologyCodeLookupDto> CreateTerminologyCodeLookupAsync(CreateTerminologyCodeLookupDto createDto)
        {
            var terminologyCode = mapper.Map<TerminologyCodeLookup>(createDto);

            await unitOfWork.Repository<TerminologyCodeLookup>().AddAsync(terminologyCode);
            await unitOfWork.CompleteAsync(); // Asynchronous commit

            return mapper.Map<TerminologyCodeLookupDto>(terminologyCode);
        }

        public async Task<bool> DeleteTerminologyCodeLookupAsync(Guid id)
        {
            // Memory-optimized lookup
            var terminologyCode = await unitOfWork.Repository<TerminologyCodeLookup>().GetByIdAsync(id);
            if (terminologyCode == null)
                return false;

            await unitOfWork.Repository<TerminologyCodeLookup>().DeleteAsync(terminologyCode);
            await unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<IEnumerable<TerminologyCodeLookupDto>> GetAllTerminologyCodeLookupsAsync()
        {
            // True asynchronous execution
            var terminologyCodes = await unitOfWork.Repository<TerminologyCodeLookup>().GetAllAsync();
            return mapper.Map<IEnumerable<TerminologyCodeLookupDto>>(terminologyCodes);
        }

        public async Task<List<TerminologyCodeLookupDto>> GetOrFetchLoincCodeAsync(string searchTerm, EnTerminologyType category = EnTerminologyType.LabTest)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<TerminologyCodeLookupDto>();

            // Normalize search term once
            searchTerm = searchTerm.Trim().ToLower();

            // Safely await the asynchronous Find method
            var existingCodes = await unitOfWork.Repository<TerminologyCodeLookup>().FindAsync(t =>
                t.Display.ToLower().Contains(searchTerm) &&
                t.TerminologyType == category);

            // Consistency Fix: Use AutoMapper instead of manual LINQ selection
            return mapper.Map<List<TerminologyCodeLookupDto>>(existingCodes);
        }

        public async Task<TerminologyCodeLookupDto?> GetTerminologyCodeLooKupByCodeAsync(string code)
        {
            // FindAsync returns a read-only list, so we await it and take the first item
            var existingCodes = await unitOfWork.Repository<TerminologyCodeLookup>().FindAsync(t => t.Code == code);
            var terminologyCode = existingCodes.FirstOrDefault();

            return terminologyCode == null ? null : mapper.Map<TerminologyCodeLookupDto>(terminologyCode);
        }

        public async Task<TerminologyCodeLookupDto?> GetTerminologyCodeLookupByIdAsync(Guid id)
        {
            var terminologyCode = await unitOfWork.Repository<TerminologyCodeLookup>().GetByIdAsync(id);
            return terminologyCode == null ? null : mapper.Map<TerminologyCodeLookupDto>(terminologyCode);
        }

        public async Task<TerminologyCodeLookupDto> UpdateTerminologyCodeLookupAsync(Guid id, UpdateTerminologyCodeLookupDto updateDto)
        {
            var terminologyCode = await unitOfWork.Repository<TerminologyCodeLookup>().GetByIdAsync(id);
            if (terminologyCode == null)
                throw new Exception($"TerminologyCodeLookup with ID {id} not found.");

            mapper.Map(updateDto, terminologyCode);

            await unitOfWork.Repository<TerminologyCodeLookup>().UpdateAsync(terminologyCode);
            await unitOfWork.CompleteAsync();

            return mapper.Map<TerminologyCodeLookupDto>(terminologyCode);
        }
    }
}