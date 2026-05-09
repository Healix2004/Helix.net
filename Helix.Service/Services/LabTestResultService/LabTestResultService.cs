using AutoMapper;
using Helix.Data.Entities;
using Helix.Service.DTOs.LabTestResultDTOs;
using Helix.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helix.Service.Services.LabTestResultService
{
    public class LabTestResultService : ILabTestResultService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LabTestResultService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<LabTestResultDto> CreateLabTestResultAsync(CreateLabTestResultDto createLabTestResultDto)
        {
            var patient = _unitOfWork.Repository<Patient>().Find(p => p.Id == createLabTestResultDto.PatientId).FirstOrDefault();
            if (patient == null) throw new Exception("Patient not found");

            var doctor = _unitOfWork.Repository<Doctor>().Find(d => d.Id == createLabTestResultDto.DoctorId).FirstOrDefault();
            if (doctor == null) throw new Exception("Doctor not found");

            var terminology = _unitOfWork.Repository<TerminologyCodeLookup>().Find(t => t.Id == createLabTestResultDto.TerminologyCodeId).FirstOrDefault();
            if (terminology == null) throw new Exception("Terminology Code not found");

            var encounter = _unitOfWork.Repository<Encounter>().Find(e => e.Id == createLabTestResultDto.EncounterId).FirstOrDefault();
            if (encounter == null) throw new Exception("Encounter not found");

            var labTestResult = _mapper.Map<LabTestResult>(createLabTestResultDto);
            labTestResult.Patient = patient;
            labTestResult.Doctor = doctor;
            labTestResult.TerminologyCode = terminology;
            labTestResult.Encounter = encounter;

            _unitOfWork.Repository<LabTestResult>().Add(labTestResult);
            _unitOfWork.Complete();

            var result = _mapper.Map<LabTestResultDto>(labTestResult);
            result.PatientId = patient.Id;
            result.DoctorId = doctor.Id;
            result.TerminologyCodeId = terminology.Id;
            result.EncounterId = encounter.Id;
            return Task.FromResult(result);
        }

        public Task<bool> DeleteLabTestResultAsync(Guid id)
        {
            var labTestResult = _unitOfWork.Repository<LabTestResult>().Find(l => l.Id == id).FirstOrDefault();
            if (labTestResult == null) return Task.FromResult(false);

            _unitOfWork.Repository<LabTestResult>().Delete(labTestResult);
            _unitOfWork.Complete();
            return Task.FromResult(true);
        }

        public Task<IEnumerable<LabTestResultDto>> GetAllLabTestResultsAsync()
        {
            var labTestResults = _unitOfWork.Repository<LabTestResult>().GetALL();
            return Task.FromResult(_mapper.Map<IEnumerable<LabTestResultDto>>(labTestResults));
        }

        public Task<LabTestResultDto> GetLabTestResultByIdAsync(Guid id)
        {
            var labTestResult = _unitOfWork.Repository<LabTestResult>().Find(l => l.Id == id).FirstOrDefault();
            return Task.FromResult(_mapper.Map<LabTestResultDto>(labTestResult));
        }

        public Task<LabTestResultDto> UpdateLabTestResultAsync(Guid id, UpdateLabTestResultDto updateLabTestResultDto)
        {
            var labTestResult = _unitOfWork.Repository<LabTestResult>().Find(l => l.Id == id).FirstOrDefault();
            if (labTestResult == null) throw new Exception("LabTestResult not found");

            _mapper.Map(updateLabTestResultDto, labTestResult);
            _unitOfWork.Repository<LabTestResult>().Update(labTestResult);
            _unitOfWork.Complete();

            return Task.FromResult(_mapper.Map<LabTestResultDto>(labTestResult));
        }
    }
}
