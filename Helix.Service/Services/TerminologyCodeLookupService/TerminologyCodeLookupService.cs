using AutoMapper;
using Helix.Data.Entities;
using Helix.Data.Enums;
using Helix.Service.DTOs.TerminologyCodeLookupDTOs;
using Helix.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helix.Service.Services.TerminologyCodeLookupService
{
    public class TerminologyCodeLookupService : ITerminologyCodeLookupService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TerminologyCodeLookupService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<TerminologyCodeLookupDto> CreateTerminologyCodeLookupAsync(CreateTerminologyCodeLookupDto createDto)
        {
            var terminologyCode = _mapper.Map<TerminologyCodeLookup>(createDto);
            
            _unitOfWork.Repository<TerminologyCodeLookup>().Add(terminologyCode);
            _unitOfWork.Complete();

            return Task.FromResult(_mapper.Map<TerminologyCodeLookupDto>(terminologyCode));
        }

        public Task<bool> DeleteTerminologyCodeLookupAsync(Guid id)
        {
            var terminologyCode = _unitOfWork.Repository<TerminologyCodeLookup>().Find(t => t.Id == id).Result.FirstOrDefault();
            if (terminologyCode == null) return Task.FromResult(false);

            _unitOfWork.Repository<TerminologyCodeLookup>().Delete(terminologyCode);
            _unitOfWork.Complete();
            return Task.FromResult(true);
        }

        public Task<IEnumerable<TerminologyCodeLookupDto>> GetAllTerminologyCodeLookupsAsync()
        {
            var terminologyCodes = _unitOfWork.Repository<TerminologyCodeLookup>().GetALL().Result;
            return Task.FromResult(_mapper.Map<IEnumerable<TerminologyCodeLookupDto>>(terminologyCodes));
        }
        public async Task<List<TerminologyCodeLookupDto>> GetOrFetchLoincCodeAsync(string searchTerm, EnTerminologyType category = EnTerminologyType.LabTest)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return new List<TerminologyCodeLookupDto>();

            var repository = _unitOfWork.Repository<TerminologyCodeLookup>();
            searchTerm = searchTerm.Trim();

            // 1. Get the data. We use ToLower() to bypass case-sensitivity issues.
            var existingCodes = await repository.Find(t =>
                t.Display.ToLower().Contains(searchTerm.ToLower()) &&
                t.TerminologyType == category);

            // 2. Map directly to DTO using LINQ (Cleaner and more performant)
            var result = existingCodes.Select(terminology => new TerminologyCodeLookupDto
            {
                Id = terminology.Id,
                Code = terminology.Code,
                Display = terminology.Display,
                SystemUrl = terminology.SystemUrl,
                TerminologyType = terminology.TerminologyType,
                UsageCount = terminology.UsageCount
            }).ToList();

            return result;
        }

        public Task<TerminologyCodeLookupDto> GetTerminologyCodeLookupByIdAsync(Guid id)
        {
            var terminologyCode = _unitOfWork.Repository<TerminologyCodeLookup>().Find(t => t.Id == id).Result.FirstOrDefault();
            return Task.FromResult(_mapper.Map<TerminologyCodeLookupDto>(terminologyCode));
        }

        public Task<TerminologyCodeLookupDto> UpdateTerminologyCodeLookupAsync(Guid id, UpdateTerminologyCodeLookupDto updateDto)
        {
            var terminologyCode = _unitOfWork.Repository<TerminologyCodeLookup>().Find(t => t.Id == id).Result.FirstOrDefault();
            if (terminologyCode == null) throw new Exception("TerminologyCodeLookup not found");

            _mapper.Map(updateDto, terminologyCode);
            _unitOfWork.Repository<TerminologyCodeLookup>().Update(terminologyCode);
            _unitOfWork.Complete();

            return Task.FromResult(_mapper.Map<TerminologyCodeLookupDto>(terminologyCode));
        }
    }
}
