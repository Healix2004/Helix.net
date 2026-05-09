using AutoMapper;
using Helix.Data.Entities;
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
            var terminologyCode = _unitOfWork.Repository<TerminologyCodeLookup>().Find(t => t.Id == id).FirstOrDefault();
            if (terminologyCode == null) return Task.FromResult(false);

            _unitOfWork.Repository<TerminologyCodeLookup>().Delete(terminologyCode);
            _unitOfWork.Complete();
            return Task.FromResult(true);
        }

        public Task<IEnumerable<TerminologyCodeLookupDto>> GetAllTerminologyCodeLookupsAsync()
        {
            var terminologyCodes = _unitOfWork.Repository<TerminologyCodeLookup>().GetALL();
            return Task.FromResult(_mapper.Map<IEnumerable<TerminologyCodeLookupDto>>(terminologyCodes));
        }

        public Task<TerminologyCodeLookupDto> GetTerminologyCodeLookupByIdAsync(Guid id)
        {
            var terminologyCode = _unitOfWork.Repository<TerminologyCodeLookup>().Find(t => t.Id == id).FirstOrDefault();
            return Task.FromResult(_mapper.Map<TerminologyCodeLookupDto>(terminologyCode));
        }

        public Task<TerminologyCodeLookupDto> UpdateTerminologyCodeLookupAsync(Guid id, UpdateTerminologyCodeLookupDto updateDto)
        {
            var terminologyCode = _unitOfWork.Repository<TerminologyCodeLookup>().Find(t => t.Id == id).FirstOrDefault();
            if (terminologyCode == null) throw new Exception("TerminologyCodeLookup not found");

            _mapper.Map(updateDto, terminologyCode);
            _unitOfWork.Repository<TerminologyCodeLookup>().Update(terminologyCode);
            _unitOfWork.Complete();

            return Task.FromResult(_mapper.Map<TerminologyCodeLookupDto>(terminologyCode));
        }
    }
}
