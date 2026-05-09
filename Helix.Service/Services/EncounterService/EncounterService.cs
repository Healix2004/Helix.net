using AutoMapper;
using Helix.Data.Entities;
using Helix.Service.DTOs.EncounterDTOs;
using Helix.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helix.Service.Services.EncounterService
{
    public class EncounterService : IEncounterService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EncounterService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<EncounterDto> CreateEncounterAsync(CreateEncounterDto createEncounterDto)
        {
            var patient = _unitOfWork.Repository<Patient>().Find(p => p.Id == createEncounterDto.PatientId).FirstOrDefault();
            if (patient == null) throw new Exception("Patient not found");

            var doctor = _unitOfWork.Repository<Doctor>().Find(d => d.Id == createEncounterDto.DoctorId).FirstOrDefault();
            if (doctor == null) throw new Exception("Doctor not found");

            var encounter = _mapper.Map<Encounter>(createEncounterDto);
            encounter.patient = patient;
            encounter.Doctor = doctor;

            _unitOfWork.Repository<Encounter>().Add(encounter);
            _unitOfWork.Complete();

            var result = _mapper.Map<EncounterDto>(encounter);
            result.PatientId = patient.Id;
            result.DoctorId = doctor.Id;
            return Task.FromResult(result);
        }

        public Task<bool> DeleteEncounterAsync(Guid id)
        {
            var encounter = _unitOfWork.Repository<Encounter>().Find(e => e.Id == id).FirstOrDefault();
            if (encounter == null) return Task.FromResult(false);

            _unitOfWork.Repository<Encounter>().Delete(encounter);
            _unitOfWork.Complete();
            return Task.FromResult(true);
        }

        public Task<IEnumerable<EncounterDto>> GetAllEncountersAsync()
        {
            var encounters = _unitOfWork.Repository<Encounter>().GetALL();
            return Task.FromResult(_mapper.Map<IEnumerable<EncounterDto>>(encounters));
        }

        public Task<EncounterDto> GetEncounterByIdAsync(Guid id)
        {
            var encounter = _unitOfWork.Repository<Encounter>().Find(e => e.Id == id).FirstOrDefault();
            return Task.FromResult(_mapper.Map<EncounterDto>(encounter));
        }

        public Task<EncounterDto> UpdateEncounterAsync(Guid id, UpdateEncounterDto updateEncounterDto)
        {
            var encounter = _unitOfWork.Repository<Encounter>().Find(e => e.Id == id).FirstOrDefault();
            if (encounter == null) throw new Exception("Encounter not found");

            _mapper.Map(updateEncounterDto, encounter);
            _unitOfWork.Repository<Encounter>().Update(encounter);
            _unitOfWork.Complete();

            return Task.FromResult(_mapper.Map<EncounterDto>(encounter));
        }
    }
}
