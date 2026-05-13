using AutoMapper;
using Helix.Data.Entities;
using Helix.Service.DTOs.DiagnoseDTOs;
using Helix.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helix.Service.Services.DiagnoseService
{
    public class DiagnoseService : IDiagnoseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DiagnoseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<DiagnoseDto> CreateDiagnoseAsync(CreateDiagnoseDto createDiagnoseDto)
        {
            var patient = _unitOfWork.Repository<Patient>().Find(p => p.Id == createDiagnoseDto.PatientId).Result.FirstOrDefault();
            if (patient == null) throw new Exception("Patient not found");

            var doctor = _unitOfWork.Repository<Doctor>().Find(d => d.Id == createDiagnoseDto.DoctorId).Result.FirstOrDefault();
            if (doctor == null) throw new Exception("Doctor not found");

            var terminology = _unitOfWork.Repository<TerminologyCodeLookup>().Find(t => t.Id == createDiagnoseDto.TerminologyCodeId).Result.FirstOrDefault();
            if (terminology == null) throw new Exception("Terminology Code not found");

            var diagnose = _mapper.Map<Diagnose>(createDiagnoseDto);
            diagnose.Patient = patient;
            diagnose.doctor = doctor;
            diagnose.TerminologyCode = terminology;

            _unitOfWork.Repository<Diagnose>().Add(diagnose);
            _unitOfWork.Complete();

            var result = _mapper.Map<DiagnoseDto>(diagnose);
            result.PatientId = patient.Id;
            result.DoctorId = doctor.Id;
            result.TerminologyCodeId = terminology.Id;
            return Task.FromResult(result);
        }

        public Task<bool> DeleteDiagnoseAsync(Guid id)
        {
            var diagnose = _unitOfWork.Repository<Diagnose>().Find(d => d.Id == id).Result.FirstOrDefault();
            if (diagnose == null) return Task.FromResult(false);

            _unitOfWork.Repository<Diagnose>().Delete(diagnose);
            _unitOfWork.Complete();
            return Task.FromResult(true);
        }

        public Task<IEnumerable<DiagnoseDto>> GetAllDiagnosesAsync()
        {
            var diagnoses = _unitOfWork.Repository<Diagnose>().GetALL().Result;
            return Task.FromResult(_mapper.Map<IEnumerable<DiagnoseDto>>(diagnoses));
        }

        public Task<DiagnoseDto> GetDiagnoseByIdAsync(Guid id)
        {
            var diagnose = _unitOfWork.Repository<Diagnose>().Find(d => d.Id == id).Result.FirstOrDefault();
            return Task.FromResult(_mapper.Map<DiagnoseDto>(diagnose));
        }

        public Task<DiagnoseDto> UpdateDiagnoseAsync(Guid id, UpdateDiagnoseDto updateDiagnoseDto)
        {
            var diagnose = _unitOfWork.Repository<Diagnose>().Find(d => d.Id == id).Result.FirstOrDefault();
            if (diagnose == null) throw new Exception("Diagnose not found");

            _mapper.Map(updateDiagnoseDto, diagnose);
            _unitOfWork.Repository<Diagnose>().Update(diagnose);
            _unitOfWork.Complete();

            return Task.FromResult(_mapper.Map<DiagnoseDto>(diagnose));
        }
    }
}
