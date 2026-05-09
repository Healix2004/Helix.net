using AutoMapper;
using Helix.Data.Entities;
using Helix.Service.DTOs.FacilitieDTOs;
using Helix.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helix.Service.Services.FacilitieService
{
    public class FacilitieService : IFacilitieService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FacilitieService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<FacilitieDto> CreateFacilitieAsync(CreateFacilitieDto createFacilitieDto)
        {
            var facilitie = _mapper.Map<Facilitie>(createFacilitieDto);
            
            _unitOfWork.Repository<Facilitie>().Add(facilitie);
            _unitOfWork.Complete();

            return Task.FromResult(_mapper.Map<FacilitieDto>(facilitie));
        }

        public Task<bool> DeleteFacilitieAsync(Guid id)
        {
            var facilitie = _unitOfWork.Repository<Facilitie>().Find(f => f.Id == id).FirstOrDefault();
            if (facilitie == null) return Task.FromResult(false);

            _unitOfWork.Repository<Facilitie>().Delete(facilitie);
            _unitOfWork.Complete();
            return Task.FromResult(true);
        }

        public Task<IEnumerable<FacilitieDto>> GetAllFacilitiesAsync()
        {
            var facilities = _unitOfWork.Repository<Facilitie>().GetALL();
            return Task.FromResult(_mapper.Map<IEnumerable<FacilitieDto>>(facilities));
        }

        public Task<FacilitieDto> GetFacilitieByIdAsync(Guid id)
        {
            var facilitie = _unitOfWork.Repository<Facilitie>().Find(f => f.Id == id).FirstOrDefault();
            return Task.FromResult(_mapper.Map<FacilitieDto>(facilitie));
        }

        public Task<FacilitieDto> UpdateFacilitieAsync(Guid id, UpdateFacilitieDto updateFacilitieDto)
        {
            var facilitie = _unitOfWork.Repository<Facilitie>().Find(f => f.Id == id).FirstOrDefault();
            if (facilitie == null) throw new Exception("Facilitie not found");

            _mapper.Map(updateFacilitieDto, facilitie);
            _unitOfWork.Repository<Facilitie>().Update(facilitie);
            _unitOfWork.Complete();

            return Task.FromResult(_mapper.Map<FacilitieDto>(facilitie));
        }
    }
}
