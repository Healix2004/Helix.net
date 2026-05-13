using AutoMapper;
using Helix.Data.Entities;
using Helix.Service.DTOs.AllergyDTOs;
using Helix.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helix.Service.Services.AllergyService
{
    public class AllergyService : IAllergyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AllergyService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<AllergyDto> CreateAllergyAsync(CreateAllergyDto createAllergyDto)
        {
            var patient = _unitOfWork.Repository<Patient>().Find(p => p.Id == createAllergyDto.PatientId).Result.FirstOrDefault();
            if (patient == null) throw new Exception("Patient not found");

            var allergy = _mapper.Map<Allergy>(createAllergyDto);
            allergy.Patient = patient;

            _unitOfWork.Repository<Allergy>().Add(allergy);
            _unitOfWork.Complete();

            var result = _mapper.Map<AllergyDto>(allergy);
            result.PatientId = patient.Id;
            return Task.FromResult(result);
        }

        public Task<bool> DeleteAllergyAsync(Guid id)
        {
            var allergy = _unitOfWork.Repository<Allergy>().Find(a => a.Id == id).Result.FirstOrDefault();
            if (allergy == null) return Task.FromResult(false);

            _unitOfWork.Repository<Allergy>().Delete(allergy);
            _unitOfWork.Complete();
            return Task.FromResult(true);
        }

        public Task<IEnumerable<AllergyDto>> GetAllAllergiesAsync()
        {
            var allergies = _unitOfWork.Repository<Allergy>().GetALL().Result;
            return Task.FromResult(_mapper.Map<IEnumerable<AllergyDto>>(allergies));
        }

        public Task<AllergyDto> GetAllergyByIdAsync(Guid id)
        {
            var allergy = _unitOfWork.Repository<Allergy>().Find(a => a.Id == id).Result.FirstOrDefault();
            return Task.FromResult(_mapper.Map<AllergyDto>(allergy));
        }

        public Task<AllergyDto> UpdateAllergyAsync(Guid id, UpdateAllergyDto updateAllergyDto)
        {
            var allergy = _unitOfWork.Repository<Allergy>().Find(a => a.Id == id).Result.FirstOrDefault();
            if (allergy == null) throw new Exception("Allergy not found");

            _mapper.Map(updateAllergyDto, allergy);
            _unitOfWork.Repository<Allergy>().Update(allergy);
            _unitOfWork.Complete();

            return Task.FromResult(_mapper.Map<AllergyDto>(allergy));
        }
    }
}
