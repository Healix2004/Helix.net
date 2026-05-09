using AutoMapper;
using Helix.Data.Entities;
using Helix.Service.DTOs.ConsentDTOs;
using Helix.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helix.Service.Services.ConsentService
{
    public class ConsentService : IConsentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ConsentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<ConsentDto> CreateConsentAsync(CreateConsentDto createConsentDto)
        {
            var patient = _unitOfWork.Repository<Patient>().Find(p => p.Id == createConsentDto.PatientId).FirstOrDefault();
            if (patient == null) throw new Exception("Patient not found");

            var doctor = _unitOfWork.Repository<Doctor>().Find(d => d.Id == createConsentDto.DoctorId).FirstOrDefault();
            if (doctor == null) throw new Exception("Doctor not found");

            var consent = _mapper.Map<Consent>(createConsentDto);
            consent.Patient = patient;
            consent.Doctor = doctor;
            
            _unitOfWork.Repository<Consent>().Add(consent);
            _unitOfWork.Complete();

            var result = _mapper.Map<ConsentDto>(consent);
            // Manually map IDs just in case they aren't shadow-mapped correctly by EF immediately before saving.
            result.PatientId = consent.Patient.Id;
            result.DoctorId = consent.Doctor.Id;
            
            return Task.FromResult(result);
        }

        public Task<bool> DeleteConsentAsync(Guid id)
        {
            var consent = _unitOfWork.Repository<Consent>().Find(c => c.Id == id).FirstOrDefault();
            if (consent == null)
            {
                return Task.FromResult(false);
            }

            _unitOfWork.Repository<Consent>().Delete(consent);
            _unitOfWork.Complete();

            return Task.FromResult(true);
        }

        public Task<IEnumerable<ConsentDto>> GetAllConsentsAsync()
        {
            var consents = _unitOfWork.Repository<Consent>().GetALL();
            var consentsDto = _mapper.Map<IEnumerable<ConsentDto>>(consents);
            return Task.FromResult(consentsDto);
        }

        public Task<ConsentDto> GetConsentByIdAsync(Guid id)
        {
            var consent = _unitOfWork.Repository<Consent>().Find(c => c.Id == id).FirstOrDefault();
            if (consent == null)
            {
                return Task.FromResult<ConsentDto>(null);
            }

            var consentDto = _mapper.Map<ConsentDto>(consent);
            return Task.FromResult(consentDto);
        }

        public Task<ConsentDto> UpdateConsentAsync(Guid id, UpdateConsentDto updateConsentDto)
        {
            var consent = _unitOfWork.Repository<Consent>().Find(c => c.Id == id).FirstOrDefault();
            if (consent == null)
            {
                throw new Exception("Consent not found");
            }

            _mapper.Map(updateConsentDto, consent);
            
            _unitOfWork.Repository<Consent>().Update(consent);
            _unitOfWork.Complete();

            return Task.FromResult(_mapper.Map<ConsentDto>(consent));
        }
    }
}
