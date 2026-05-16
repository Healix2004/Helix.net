using AutoMapper;
using Helix.Data.Entities;
using Helix.Service.DTOs.LabTestResultDTOs;
using Helix.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
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
        public Task<bool> DeleteLabTestResultAsync(Guid id)
        {
            var labTestResult = _unitOfWork.Repository<LabTestResult>().Find(l => l.Id == id).Result.FirstOrDefault();
            if (labTestResult == null) return Task.FromResult(false);

            _unitOfWork.Repository<LabTestResult>().Delete(labTestResult);
            _unitOfWork.Complete();
            return Task.FromResult(true);
        }

        public Task<IEnumerable<LabTestResultDto>> GetAllLabTestResultsAsync()
        {
            var labTestResults = _unitOfWork.Repository<LabTestResult>().Find(l => true).Result
                .Include(l => l.TerminologyCode)
                .Include(l => l.Patient)
                    .ThenInclude(p => p.AppUser).ToList();
            return Task.FromResult(_mapper.Map<IEnumerable<LabTestResultDto>>(labTestResults));
        }

        public Task<IEnumerable<LabTestResultDto>> GetAllLabTestResultsAsync(Guid patientId)
        {
            var labTestResults= _unitOfWork.Repository<LabTestResult>().Find(l=>l.PatientId == patientId).Result
                .Include(l=>l.TerminologyCode)
                .Include(l=>l.Patient)
                    .ThenInclude(p=>p.AppUser).ToList();
            return Task.FromResult(_mapper.Map<IEnumerable<LabTestResultDto>>(labTestResults));
        }

        public Task<LabTestResultDto> GetLabTestResultByIdAsync(Guid id)
        {
            var labTestResult = _unitOfWork.Repository<LabTestResult>().Find(l => l.Id == id).Result.FirstOrDefault();
            return Task.FromResult(_mapper.Map<LabTestResultDto>(labTestResult));
        }

        public Task<LabTestResultDto> UpdateLabTestResultAsync(Guid id, UpdateLabTestResultDto updateLabTestResultDto)
        {
            var labTestResult = _unitOfWork.Repository<LabTestResult>().Find(l => l.Id == id).Result.FirstOrDefault();
            if (labTestResult == null) throw new Exception("LabTestResult not found");

            _mapper.Map(updateLabTestResultDto, labTestResult);
            _unitOfWork.Repository<LabTestResult>().Update(labTestResult);
            _unitOfWork.Complete();

            return Task.FromResult(_mapper.Map<LabTestResultDto>(labTestResult));
        }
    }
}
