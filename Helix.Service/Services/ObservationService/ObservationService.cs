using AutoMapper;
using Helix.Data.Entities;
using Helix.Service.DTOs.ObservationDTOs;
using Helix.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helix.Service.Services.ObservationService
{
    public class ObservationService : IObservationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ObservationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<ObservationDto> CreateObservationAsync(CreateObservationDto createObservationDto)
        {
            var encounter = _unitOfWork.Repository<Encounter>().Find(e => e.Id == createObservationDto.EncounterId).Result.FirstOrDefault();
            if (encounter == null) throw new Exception("Encounter not found");

            var observation = _mapper.Map<Observation>(createObservationDto);
            observation.Encounter = encounter;

            _unitOfWork.Repository<Observation>().Add(observation);
            _unitOfWork.Complete();

            var result = _mapper.Map<ObservationDto>(observation);
            result.EncounterId = encounter.Id;
            return Task.FromResult(result);
        }

        public Task<bool> DeleteObservationAsync(Guid id)
        {
            var observation = _unitOfWork.Repository<Observation>().Find(o => o.Id == id).Result.FirstOrDefault();
            if (observation == null) return Task.FromResult(false);

            _unitOfWork.Repository<Observation>().Delete(observation);
            _unitOfWork.Complete();
            return Task.FromResult(true);
        }

        public Task<IEnumerable<ObservationDto>> GetAllObservationsAsync()
        {
            var observations = _unitOfWork.Repository<Observation>().GetALL().Result;
            return Task.FromResult(_mapper.Map<IEnumerable<ObservationDto>>(observations));
        }

        public Task<ObservationDto> GetObservationByIdAsync(Guid id)
        {
            var observation = _unitOfWork.Repository<Observation>().Find(o => o.Id == id).Result.FirstOrDefault();
            return Task.FromResult(_mapper.Map<ObservationDto>(observation));
        }

        public Task<ObservationDto> UpdateObservationAsync(Guid id, UpdateObservationDto updateObservationDto)
        {
            var observation = _unitOfWork.Repository<Observation>().Find(o => o.Id == id).Result.FirstOrDefault();
            if (observation == null) throw new Exception("Observation not found");

            _mapper.Map(updateObservationDto, observation);
            _unitOfWork.Repository<Observation>().Update(observation);
            _unitOfWork.Complete();

            return Task.FromResult(_mapper.Map<ObservationDto>(observation));
        }
    }
}
